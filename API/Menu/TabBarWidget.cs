using System;
using System.Collections.Generic;
using System.Linq;
using API.Graphics;
using API.Input;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Menu;

/// <summary>
/// A set of tabs
/// Expected to have static lifetime -- otherwise, make sure to manually unsubscribe from <c>InputLib.DeviceChange</c>
/// </summary>
public sealed class TabBarWidget : ILayoutWidget, IInputWidget, IActor
{
    public List<Label> Labels { get; private set; }

    public SelectionType PrefDir => SelectionType.Horiz;
    public SelectionType CurDir
    {
        get;
        set
        {
            field = value;
            this._UpdateInputPrompt();
        }
    } = SelectionType.None;

    private const int _DefaultLabelPaddingLR = 30;
    private const int _DefaultLabelPaddingTB = 20;

    public Label PromptL { get; } = new()
    {
        Padding = new(0, _DefaultLabelPaddingTB),
        Alignment = Alignment.Controlled
    };
    public Label PromptR { get; } = new()
    {
        Padding = new(0, _DefaultLabelPaddingTB),
        Alignment = Alignment.Controlled
    };

    public bool CheckInput { get; set; } = true;

    public int Index { get; set; } = 0;

    public int OptCount
    {
        get;
        private set
        {
            field = value;
            this.Index = Math.Clamp(this.Index, 0, Math.Max(value - 1, 0));
        }
    }

    public Action<int>? OnChangeIndex { get; set; }

    /// <summary>
    /// Animation progress per-item
    /// </summary>
    public List<Progress> Progs { get; private set; }

    public ActorData Data { get; private set; }

    /// <inheritdoc cref="ActorData.AnimFromDir" />
    public Dir AnimFromDir
    {
        get => this.Data.AnimFromDir;
        set
        {
            this.Data.AnimFromDir = value;

            foreach (Label l in this.Labels)
            {
                l.AnimFromDir = value;
            }

            this.PromptL.AnimFromDir = value;
            this.PromptR.AnimFromDir = value;
        }
    }

    private const int _OutlineWidth = 10;
    private const int _YOffset = 9;

    public TabBarWidget(Vector2 pos, int capacity)
    {
        this.Data = new ActorData(this, RenderPriority.B2Med);

        this.Position = pos;
        this.Alignment = Alignment.Center;
        this.Size = Point.Zero;
        this.Padding = new Padding(30, 0);

        this.Labels = new List<Label>(capacity);

        this.Progs = [.. Enumerable.Repeat(Progress.Zero, capacity)];
        this.OptCount = capacity;

        InputLib.OnDeviceChange += this._UpdateInputPrompt;
    }

    public TabBarWidget(Vector2 pos, params string[] optionText) : this(pos, optionText.Length)
    {
        for (int i = 0; i < optionText.Length; i++)
        {
            this.Labels.Add(new Label()
            {
                Text = optionText[i],
                Padding = new(_DefaultLabelPaddingLR, _DefaultLabelPaddingTB),
            });
        }

        this.CalcLayout();
    }

    public void SetText(params string[] optionText)
    {
        int i = 0;
        for (; i < optionText.Length && i < this.Labels.Count; i++)
        {
            this.Labels[i].IsVisible = true;
            this.Labels[i].Text = optionText[i];
        }

        // New list shorter, blank out remaining Labels and progs
        for (; i < this.Labels.Count; i++)
        {
            this.Labels[i].IsVisible = false;
            this.Progs[i] = new();
        }

        // New list longer, add more Labels and progs
        for (; i < optionText.Length; i++)
        {
            this.Labels.Add(new Label()
            {
                Text = optionText[i],
                Padding = new(_DefaultLabelPaddingLR, _DefaultLabelPaddingTB),
                Alignment = Alignment.Controlled
            });

            this.Progs.Add(Progress.Zero);
        }

        this.OptCount = optionText.Length;

        this.CalcLayout();
    }

    public void CalcLayout()
    {
        this.Size = Point.Zero;

        foreach (Label l in this.Labels)
        {
            this.Width += l.Padding.L;
            l.Position = new(this.X + this.Width, this.Y + l.Padding.T);
            this.Width += l.Width + l.Padding.R;

            if (l.Height + l.Padding.TB > this.Height)
            {
                this.Height = l.Height + l.Padding.TB;
            }
        }

        this.Origin = this.Data.CalcOrigin();

        if (this.Origin != Point.Zero)
        {
            foreach (Label l in this.Labels)
            {
                l.Origin = this.Origin;
            }
            this.PromptL.Origin = this.Origin;
            this.PromptR.Origin = this.Origin;
        }

        this.PromptL.Position = new(this.X - this.PromptL.Width - this.Padding.L, this.Y + this.PromptL.Padding.T);
        this.PromptR.Position = new(this.X + this.Width + this.Padding.R, this.Y + this.PromptR.Padding.T);
    }

    public void Input(GameTime gt)
    {
        this.Index = this.CheckInput();
    }

    public void OnCreate()
    {
        // todo why dont prompts appear to animate
        this.PromptL.Create();
        this.PromptR.Create();

        foreach (Label l in this.Labels)
        {
            l.Create();
        }
    }

    public void OnDestroy()
    {
        this.PromptL.Destroy();
        this.PromptR.Destroy();

        foreach (Label l in this.Labels)
        {
            l.Destroy();
        }
    }

    public void Draw(GameTime gt)
    {
        if (this.OptCount != 0)
        {
            this.PromptL.IsVisible = this.CheckInput;
            this.PromptR.IsVisible = this.CheckInput;

            int w = 0;

            for (int i = 0; i < this.OptCount; i++)
            {
                this.Progs[i] = RenderLib.UpdateProg(this.Progs[i], this.Speed, gt,
                    i == this.Index ? AnimDirs.In : AnimDirs.Out);

                float yOff = (float) this.Progs[i] * _YOffset;

                Vector2 pos = new(MathHelper.SmoothStep(this.AnimFrom.X, this.Position.X,
                    (float) this.Prog) + w, this.Position.Y - this.Padding.T - yOff);

                Point size = new(this.Labels[i].Width + this.Labels[i].Padding.LR - _OutlineWidth,
                    (int) (this.Height + this.Padding.TB + yOff * 2));

                RenderLib.DrawParallelogram(pos, size, this.Origin, Settings.Theme.Bg,
                    Settings.Theme.Fg, _OutlineWidth, RenderLib.DefaultSlant,
                    RenderLib.DefaultSlant, Progress.One);

                if (this.Progs[i] != 0)
                {

                    // Cursor
                    RenderLib.DrawParallelogram(pos, size, this.Origin, Settings.Theme.Accent,
                        Color.Red, 0f, 6, 6,
                        Progress.Min(this.Prog, this.Progs[i]));
                }

                w += this.Labels[i].Width + this.Labels[i].Padding.LR;
            }
        }

        foreach (Label l in this.Labels)
        {
            l.Data.Act(gt);

            if (DebugUtil.DrawActorOutlines)
            {
                l.Data.DrawDebug(false);
            }
        }

        this.PromptL.Data.Act(gt);
        this.PromptR.Data.Act(gt);

        if (DebugUtil.DrawActorOutlines)
        {
            this.PromptL.Data.DrawDebug(false);
            this.PromptR.Data.DrawDebug(false);

            // Disabled input overlay
            if (!this.CheckInput)
            {
                this.Data.DrawBackground(Color.ActorDisabledInput);
            }
        }
    }

    private void _UpdateInputPrompt()
    {
        this.PromptL.Text = this.CurDir.GetDec()?.GetCurrentGlyph() ?? "";
        this.PromptR.Text = this.CurDir.GetInc()?.GetCurrentGlyph() ?? "";

        this.CalcLayout();
    }
}
