using System;
using System.Collections.Generic;
using System.Linq;
using API.Graphics;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Menu;

/* todo:
    - internal label alignment setting
    - max height (creates scrollbar)
*/
public class ListWidget : ILayoutWidget, IInputWidget, IActor {
    public List<Label> Labels { get; private set; } = null!;

    public Padding ItemPadding {
        get;
        init {
            field = value;
            foreach (Label l in this.Labels) l.Padding = value;
        }
    } = new(30, 20);

    public SelectionType PrefDir => SelectionType.Vert;
    public SelectionType CurDir { get; set; } = SelectionType.None;

    public bool CheckInput { get; set; } = true;

    public int Index { get; set; } = 0;

    public int OptCount {
        get;
        protected set {
            field = value;
            this.Index = Math.Clamp(this.Index, 0, Math.Max(value - 1, 0));
        }
    }

    public Action<int>? OnSelect { get; set; }

    /// <summary>
    /// Animation progress per-item
    /// </summary>
    public List<Progress> Progs { get; private set; } = null!;

    public ActorData Data { get; private set; } = null!;

    /// <inheritdoc cref="ActorData.AnimFromDir" />
    public Dir AnimFromDir {
        get => this.Data.AnimFromDir;
        set {
            this.Data.AnimFromDir = value;
            foreach (Label l in this.Labels) l.AnimFromDir = value;
        }
    }

    public ListWidget(Vector2 pos, int capacity) {
        this._Setup(pos, capacity);
    }

    public ListWidget(Vector2 pos, params string[] optionText) {
        this._Setup(pos, optionText.Length);
        for (int i = 0; i < this.Labels.Count; i++) this.Labels[i].Text = optionText[i];
        this.CalcLayout();
    }

    protected virtual void _Setup(Vector2 pos, int capacity) {
        this.Data = new ActorData(this, RenderPriority.B2Med);

        this.Position = pos;
        this.Size = Point.Zero;

        this.Labels = new List<Label>(capacity);
        for (int i = 0; i < capacity; i++) this.Labels.Add(new Label());

        this.Progs = [.. Enumerable.Repeat(Progress.Zero, capacity)];
        this.OptCount = capacity;
    }

    public void SetText(params string[] optionText) {
        int i = 0;
        for (; i < optionText.Length && i < this.Labels.Count; i++) {
            this.Labels[i].IsVisible = true;
            this.Labels[i].Padding = this.ItemPadding;
            this.Labels[i].Text = optionText[i];
        }

        // New list shorter, blank out remaining Labels and progs
        for (; i < this.Labels.Count; i++) {
            this.Labels[i].IsVisible = false;
            this.Labels[i].Padding = Padding.Zero;
            this.Progs[i] = new();
        }

        // New list longer, add more Labels and progs
        for (; i < optionText.Length; i++) {
            this.Labels.Add(new Label() {
                Text = optionText[i],
                Padding = this.ItemPadding
            });

            this.Progs.Add(Progress.Zero);
        }

        this.OptCount = optionText.Length;

        this.CalcLayout();
    }

    public virtual void CalcLayout() {
        this.Size = Point.Zero;

        foreach (Label l in this.Labels) {
            this.Height += l.Padding.T;
            l.Position = this.Position + new Vector2(l.Padding.L, this.Height);
            this.Height += l.Height + l.Padding.B;
            if (l.Width + l.Padding.LR > this.Width) this.Width = l.Width + l.Padding.LR;
        }

        this.Origin = this.Data.CalcOrigin();
    }

    public void Input(GameTime gameTime) => this.Index = this.CheckInput();

    public virtual void OnCreate() {
        foreach (Label l in this.Labels) l.Create();
    }

    public virtual void OnDestroy() {
        foreach (Label l in this.Labels) l.Destroy();
    }

    public virtual void Draw(GameTime gameTime) {
        if (this.OptCount != 0) {
            this.Input(gameTime);

            int h = 0;
            for (int i = 0; i < this.OptCount; i++) {
                this.Progs[i] = RenderLib.UpdateProg(this.Progs[i], IActor.DefaultSpeed, gameTime,
                    i == this.Index ? AnimDirs.In : AnimDirs.Out);

                if (this.Progs[i] != 0) {
                    // Cursor
                    RenderLib.DrawParallelogram(new(MathHelper.SmoothStep(this.AnimFrom.X,
                        this.Position.X - this.Padding.L, (float) this.Prog),
                        this.Position.Y + h - this.Padding.T),
                        new(this.Width + this.Padding.LR, this.Labels[i].Height +
                        this.Labels[i].Padding.TB + this.Padding.TB),
                        this.Origin, Settings.ColorAccent, Color.Red,
                        0f, RenderLib.DefaultSlant, RenderLib.DefaultSlant,
                        Progress.Min(this.Prog, this.Progs[i]));
                }

                h += this.Labels[i].Height + this.Labels[i].Padding.TB;
            }
        }

        foreach (Label l in this.Labels) {
            l.Data.Act(gameTime);
            if (_DebugMenu._drawActorOutlines) l.Data.DrawDebug(false);
        }

        // Disabled input overlay
        if (_DebugMenu._drawActorOutlines) {
            if (!this.CheckInput) this.Data.DrawBackground(Colors.ActorDisabledInput);
        }
    }
}
