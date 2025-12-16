using System;
using System.Collections.Generic;
using API.Graphics;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Menu;

/* todo:
    - internal label alignment setting
    - max height (creates scrollbar)
*/
public sealed class ListWidget : ILayoutWidget, IInputWidget, IActor {
    public List<Label> Labels { get; private set; }

    public SelectionType PrefDir => SelectionType.Vert;
    public SelectionType CurDir { get; set; } = SelectionType.None;

    public bool CheckInput { get; set; } = true;

    public int Index { get; set; } = 0;

    public int OptCount { get; private set; }

    public Action<int>? OnSelect { get; set; }

    /// <summary>
    /// Animation progress per-item
    /// </summary>
    public List<Progress> Progs { get; }

    public ActorData Data { get; }

    public ListWidget(Vector2 pos, params string[] optionText) {
        this.Data = new ActorData(this, RenderPriority.B2Med);

        this.Position = pos;
        this.Size = Point.Zero;

        this.Labels = new List<Label>(optionText.Length);

        for (int i = 0; i < optionText.Length; i++) this.Labels.Add(new Label() {
            Text = optionText[i],
            Padding = new(40, 20, 20, 20)
        });

        this.Progs = new List<Progress>(optionText.Length);
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
                Padding = new(30, 20)
            });

            this.Progs.Add(new Progress());
        }

        this.OptCount = optionText.Length;

        this.CalcLayout();
    }

    public void CalcLayout() {
        this.Size = Point.Zero;

        foreach (Label l in this.Labels) {
            this.Height += l.Padding.T;
            l.Position = this.Position + new Vector2(l.Padding.L, this.Height);
            l.BasePos = new Vector2(Const.OffXDest, l.Y);
            this.Height += l.Height + l.Padding.B;
            if (l.Width + l.Padding.LR > this.Width) this.Width = l.Width + l.Padding.LR;
        }

        this.Origin = this.Data.CalcOrigin();
    }

    public void Input(GameTime gameTime) => this.Index = this.CheckInput();

    public void Create() {
        this.AddRoutine(IActor.In);
        foreach (Label l in this.Labels) l.AddRoutine(IActor.In);
    }

    public void Destroy() {
        this.AddRoutine(IActor.Out);
        foreach (Label l in this.Labels) l.AddRoutine(IActor.Out);
    }

    public void Draw(GameTime gameTime) {
        this.Input(gameTime);

        int h = 0;
        for (int i = 0; i < this.OptCount; i++) {
            this.Progs[i] = RenderLib.UpdateProg(this.Progs[i], 2f, gameTime,
                i == this.Index ? AnimDirs.In : AnimDirs.Out);

            if (this.Progs[i] != 0) {
                // Cursor
                RenderLib.DrawParallelogram(new Vector2(this.Position.X - this.Padding.L,
                    this.Position.Y + h - this.Padding.T),
                    new Point(this.Width + this.Padding.LR, this.Labels[i].Height +
                    this.Labels[i].Padding.TB + this.Padding.TB),
                    this.Origin, Settings.ColorAccent, Color.Red,
                    0f, RenderLib.DefaultSlant, RenderLib.DefaultSlant,
                    Progress.Min(this.Prog, this.Progs[i]));
            }

            h += this.Labels[i].Height + this.Labels[i].Padding.TB;
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
