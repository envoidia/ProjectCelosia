using System;
using System.Collections.Generic;
using System.Linq;
using API.Debug;
using API.Graphics;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Menu.Widget;

/* todo:
    - internal label alignment setting
    - max height (creates scrollbar)
    - maybe make every option an IComponent and make them hold Labels?
*/
public sealed class ListWidget : ILayoutWidget, IInputWidget, IActor
{
    public List<Label> LabelsL { get; private set; } = null!;

    private const int _GapBeforeRight = 400;

    /// <summary>
    /// Whether this has a right-side component
    /// </summary>
    public bool UseRight
    {
        get
        {
            return this.LabelsR is not null;
        }
    }

    public List<Label> LabelsR { get; private set; } = null!;

    /// <summary>
    /// Set width for this. 0 means dynamic width.
    /// Call <c>CalcLayout</c> after changing
    /// </summary>
    public int FixedWidth = 0;

    /// <summary>
    /// Amount to add to X per option. You probably want <c>NormalSlant</c>
    /// Call <c>CalcLayout</c> after changing
    /// </summary>
    public int Slant = 0;

    public const int NormalSlant = -15;

    public Padding ItemPadding
    {
        get;
        init
        {
            field = value;

            foreach (Label l in this.LabelsL)
            {
                l.Padding = value;
            }
        }
    } = new(40, 20, 10, 10);

    public SelectionType PrefDir
    {
        get
        {
            return SelectionType.Vert;
        }
    }

    public SelectionType CurDir { get; set; } = SelectionType.None;

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
    public List<Progress> Progs { get; private set; } = null!;

    public ActorData Data { get; private set; } = null!;

    /// <inheritdoc cref="ActorData.AnimFromDir" />
    public Dir AnimFromDir
    {
        get
        {
            return this.Data.AnimFromDir;
        }
        set
        {
            this.Data.AnimFromDir = value;

            foreach (Label l in this.LabelsL)
            {
                l.AnimFromDir = value;
            }
        }
    }

    public ListWidget(Vector2 pos, bool useRight, int capacity)
    {
        this._Setup(pos, capacity, useRight);

        this.Data.OnCreate = this.OnCreate;
        this.Data.OnDestroy = this.OnDestroy;
    }

    public ListWidget(Vector2 pos, bool useRight, params ReadOnlySpan<string> textL)
    {
        this._Setup(pos, textL.Length, useRight);

        for (int i = 0; i < this.OptCount; i++)
        {
            this.LabelsL[i].Text = textL[i];
        }

        this.CalcLayout();

        this.Data.OnCreate = this.OnCreate;
        this.Data.OnDestroy = this.OnDestroy;
    }

    private void _Setup(Vector2 pos, int capacity, bool useRight)
    {
        this.Data = new ActorData(this, RenderPriority.B2Med);

        this.Position = pos;
        this.Size = Point.Zero;

        this.LabelsL = new List<Label>(capacity);

        for (int i = 0; i < capacity; i++)
        {
            this.LabelsL.Add(new Label()
            {
                Padding = this.ItemPadding
            });
        }

        if (useRight)
        {
            this.LabelsR = new List<Label>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                this.LabelsR.Add(new Label()
                {
                    Alignment = Alignment.TopRight
                });
            }
        }

        this.Progs = [.. Enumerable.Repeat(Progress.Zero, capacity)];
        this.OptCount = 0;
    }

    /// <summary>
    /// Call <c>CalcLayout</c> after changing
    /// </summary>
    public void SetTextL(params ReadOnlySpan<string> textL)
    {
        this.OptCount = textL.Length;

        int i = 0;

        // Set existing labels until reaching the end of the new text
        for (; i < textL.Length && i < this.LabelsL.Count; i++)
        {
            this.LabelsL[i].IsVisible = true;
            this.LabelsL[i].Padding = this.ItemPadding;
            this.LabelsL[i].Text = textL[i];
        }

        // New list shorter, blank out remaining Labels and progs
        for (; i < this.LabelsL.Count; i++)
        {
            this.LabelsL[i].IsVisible = false;
            this.LabelsL[i].Padding = Padding.Zero;
            this.Progs[i] = Progress.Zero;
        }

        // New list longer, add more Labels and progs
        for (; i < textL.Length; i++)
        {
            this.LabelsL.Add(new Label()
            {
                Text = textL[i],
                Padding = this.ItemPadding
            });

            this.Progs.Add(Progress.Zero);
        }
    }

    // todo deduplicate
    /// <inheritdoc cref="SetTextL" />
    public void SetTextR(params ReadOnlySpan<string> textR)
    {
        Assert.NotNull(this.LabelsR);

        this.OptCount = textR.Length;

        int i = 0;
        for (; i < textR.Length && i < this.LabelsL.Count; i++)
        {
            this.LabelsR[i].IsVisible = true;
            this.LabelsR[i].Padding = this.ItemPadding;
            this.LabelsR[i].Text = textR[i];
        }

        for (; i < this.LabelsL.Count; i++)
        {
            this.LabelsR[i].IsVisible = false;
            this.LabelsR[i].Padding = Padding.Zero;
        }

        for (; i < textR.Length; i++)
        {
            this.LabelsR.Add(new Label()
            {
                Text = textR[i],
                Alignment = Alignment.TopRight,
                Padding = this.ItemPadding
            });
        }
    }

    public void CalcLayout()
    {
        this.Size = Point.Zero;

        // todo i dont think non-fixed width works correctly (this must be set after all L calcs)
        int prevW = this.FixedWidth != 0 ? this.FixedWidth : this.Width;

        for (int i = 0; i < this.OptCount; i++)
        {
            Label l = this.LabelsL[i];

            this.Height += l.Padding.T;
            l.Position = this.Position + new Vector2(l.Padding.L + (this.Slant * i), this.Height);
            this.Height += l.Height + l.Padding.B;

            int w = l.Width + l.Padding.LR;

            if (this.UseRight)
            {
                Label lr = this.LabelsR[i];

                lr.Position = new(this.X + prevW - lr.Padding.R + (this.Slant * i), l.Y);

                w += lr.Width + lr.Padding.LR + _GapBeforeRight;
            }

            if (w > this.Width)
            {
                this.Width = w;
            }
        }

        if (this.FixedWidth != 0)
        {
            this.Width = this.FixedWidth;
        }

        this.Origin = this.Data.CalcOrigin();

        if (UseRight)
        {
            Assert.Eq(this.LabelsL.Count, this.LabelsR.Count);
        }
    }

    public void Input(GameTime gt)
    {
        this.Index = this.CheckInput();
    }

    public void OnCreate()
    {
        for (int i = 0; i < this.OptCount; i++)
        {
            this.LabelsL[i].Create();

            if (this.UseRight)
            {
                this.LabelsR[i].Create();
            }
        }
    }

    public void OnDestroy()
    {
        for (int i = 0; i < this.OptCount; i++)
        {
            this.LabelsL[i].Destroy();

            if (this.UseRight)
            {
                this.LabelsR[i].Destroy();
            }
        }
    }

    public void Draw(GameTime gt)
    {
        // todo cleanup + move input out of draw
        // + dont have the cursor immediately vanish when optcount goes from n>0 to =0
        if (this.OptCount != 0)
        {
            int h = 0;
            for (int i = 0; i < this.OptCount; i++)
            {
                this.Progs[i] = RenderLib.UpdateProg(this.Progs[i], IActor.DefaultSpeed, gt,
                    i == this.Index ? AnimDirs.In : AnimDirs.Out);

                if (this.Progs[i] != 0)
                {
                    // Cursor
                    RenderLib.DrawParallelogram(new(MathHelper.SmoothStep(this.AnimFrom.X,
                        this.Position.X - this.Padding.L, (float) this.Prog) + (this.Slant * i),
                        this.Position.Y + h - this.Padding.T),
                        new(this.Width + this.Padding.LR, this.LabelsL[i].Height +
                        this.LabelsL[i].Padding.TB + this.Padding.TB),
                        this.Origin, Settings.Theme.Accent, Color.Red,
                        0f, RenderLib.DefaultSlant, RenderLib.DefaultSlant,
                        Progress.Min(this.Prog, this.Progs[i]));
                }

                h += this.LabelsL[i].Height + this.LabelsL[i].Padding.TB;
            }
        }

        for (int i = 0; i < this.OptCount; i++)
        {
            Label l = this.LabelsL[i];
            l.Data.Act(gt);

            if (DebugUtil.DrawActorOutlines)
            {
                l.Data.DrawDebug(false);
            }

            if (this.UseRight)
            {
                Label lr = this.LabelsR[i];
                lr.Data.Act(gt);

                if (DebugUtil.DrawActorOutlines)
                {
                    lr.Data.DrawDebug(false);
                }
            }
        }

        // Disabled input overlay
        if (DebugUtil.DrawActorOutlines && !this.CheckInput)
        {
            this.Data.DrawBackground(Color.ActorDisabledInput);
        }
    }
}
