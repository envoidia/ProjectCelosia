using System;
using System.Collections.Generic;
using System.Linq;
using API.Graphics;
using API.Input;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Menu;

public sealed class TabBarWidget : ILayoutWidget, IInputWidget, IActor, IAnimated {
    public List<Label> Labels { get; private set; }

    public SelectionType PrefDir => SelectionType.Horiz;
    public SelectionType CurDir { get; set; } = SelectionType.None;

    public bool CheckInput { get; set; } = true;

    public int Index { get; set; } = 0;
    public int OptCount { get; private set; }

    public Action? OnSelect { get; set; }

    public ActorData Data { get; }

    public Progress Prog { get; set; } = new();

    /// <summary>
    /// Animation progress per-item
    /// </summary>
    public List<Progress> Progs { get; }

    public float Speed { get; private set; } = 4f;

    private const int _OutlineWidth = 10;
    private const int _YOffset = 9;

    public TabBarWidget(Vector2 pos, int capacity) {
        this.Data = new ActorData(this, RenderPriority.B2Med);

        this.Position = pos;
        this.Size = Point.Zero;

        this.Labels = new List<Label>(capacity);
        this.Progs = [.. Enumerable.Repeat(new Progress(), capacity)];
        this.OptCount = capacity;
    }

    public TabBarWidget(Vector2 pos, params string[] optionText) {
        this.Data = new ActorData(this, RenderPriority.B2Med);

        this.Position = pos;
        this.Size = Point.Zero;

        this.Labels = new List<Label>(optionText.Length);

        for (int i = 0; i < optionText.Length; i++) this.Labels.Add(new Label() {
            Text = optionText[i],
            Padding = new(30, 20)
        });

        this.Progs = [.. Enumerable.Repeat(new Progress(), optionText.Length)];
        this.OptCount = optionText.Length;

        this.CalcLayout();
    }

    public void SetText(params string[] optionText) {
        int i = 0;
        for (; i < optionText.Length && i < this.Labels.Count; i++) this.Labels[i].Text = optionText[i];

        // New list shorter, blank out remaining Labels and progs
        for (; i < this.Labels.Count; i++) {
            this.Labels[i].Text = "";
            this.Progs[i] = new();
        }

        // New list longer, add more Labels and progs
        for (; i < optionText.Length; i++) {
            this.Labels.Add(new Label() {
                Text = optionText[i],
                Padding = new(30, 15)
            });

            this.Progs.Add(new Progress());
        }

        this.OptCount = optionText.Length;

        this.CalcLayout();
    }

    public void CalcLayout() {
        this.Size = Point.Zero;

        foreach (Label l in this.Labels) {
            this.Width += l.Padding.L;
            l.Position = new Vector2(this.X + this.Width, this.Y + l.Padding.T);
            this.Width += l.Width + l.Padding.R;
            if (l.Height + l.Padding.TB > this.Height) this.Height = l.Height + l.Padding.TB;
        }

        this.Origin = this.Data.CalcOrigin();

        if (this.Origin != Point.Zero) {
            foreach (Label l in this.Labels) l.Position -= this.Origin.ToVector2();
        }
    }

    public void Input(GameTime gameTime) => this.Index = this.CheckInput();

    public void Create() => this.AddRoutine(IAnimated.In);
    public void Destroy() => this.AddRoutine(IAnimated.Out);

    public void Draw(GameTime gameTime) {
        this.Input(gameTime);

        int w = 0;
        for (int i = 0; i < this.OptCount; i++) {
            this.Progs[i] = RenderLib.UpdateProg(this.Progs[i], this.Speed, gameTime,
                i == this.Index ? AnimDirs.In : AnimDirs.Out);

            float yOff = (float) this.Progs[i] * _YOffset;

            Vector2 pos = new(this.Position.X + w - this.Padding.L, this.Position.Y - this.Padding.T - yOff);
            
            Point size = new(this.Labels[i].Width + this.Labels[i].Padding.LR - _OutlineWidth,
                (int) (this.Height + this.Padding.TB + yOff * 2));

            RenderLib.DrawParallelogram(pos, size, this.Origin, Settings.ColorBg,
                Settings.ColorFg, _OutlineWidth, 6, 6, this.Prog);

            if (this.Progs[i] != 0) {
                // Cursor
                RenderLib.DrawParallelogram(pos, size, this.Origin, Settings.ColorAccent,
                    Color.Red, 0f, 6, 6,
                    Progress.Min(this.Prog, this.Progs[i]));
            }

            w += this.Labels[i].Width + this.Labels[i].Padding.LR;
        }

        foreach (Label l in this.Labels) {
            l.Data.Act(gameTime);
            if (DebugMenu._drawActorOutlines) l.Data.DrawDebug();
        }

        // Disabled input overlay
        if (DebugMenu._drawActorOutlines) {
            if (!this.CheckInput) this.Data.DrawBackground(Colors.ActorDisabledInput);
        }
    }
}
