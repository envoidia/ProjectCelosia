using System;
using API.Graphics;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

/* todo:
    - internal label alignment setting
    - max height (creates scrollbar)
    - fix cursor pos
*/
public sealed class ListWidget : IWidget {
    public Label[] Labels { get; }

    /// <summary>
    /// Animation progress per-item
    /// </summary>
    public Progress[] Progs { get; }

    public int Index { get; set; } = 0;
    public int MaxIndex { get; }

    public WidgetSelectionType PrefDir => WidgetSelectionType.Vert;
    public WidgetSelectionType CurDir { get; set; }

    public Padding Padding { get; set; } = new(L: 20);

    public Action? OnSelect { get; set; }

    public ActorData Data { get; }

    public ListWidget(params string[] optionText) {
        this.Data = new ActorData(this, RenderPriority.B2Med);

        this.Size = Point.Zero;

        this.Labels = new Label[optionText.Length];
        for (int i = 0; i < optionText.Length; i++) this.Labels[i] = new Label() {
            Text = optionText[i],
            Padding = new(20, 20, 20, 20)
        };

        this.Progs = new Progress[optionText.Length];

        this.MaxIndex = optionText.Length;
    }

    public void Create() => this.CalcLayout();

    public void Destroy() => this.MarkForRemoval();

    private int _oldIndex = 0;

    public void Draw(GameTime gameTime) {
        this.Index = MenuLib.CheckMovement1D(this.Index, this.MaxIndex, this.CurDir);

        // Cursor + afterimage
        int h = 0;
        for (int i = 0; i < this.Progs.Length; i++) {
            this.Progs[i] = RenderLib.UpdateProg(this.Progs[i], 2f, gameTime,
                i == this.Index ? AnimDirs.In : AnimDirs.Out);

            if (this.Progs[i] != 0) {
                RenderLib.DrawParallelogram(new Vector2(this.Position.X - this.Padding.L,
                    this.Position.Y + h - this.Padding.T),
                    new Point(this.Width + this.Padding.LR, this.Labels[i].Height +
                    this.Labels[i].Padding.TB + this.Padding.TB),
                    this.Origin, Settings.ColorAccent,
                    Settings.ColorAccent, 0f, 6, 6, this.Progs[i]);
            }

            h += this.Labels[i].Height + this.Labels[i].Padding.TB;
        }

        foreach (Label l in this.Labels) l.Data.Act(gameTime);
    }

    public void CalcLayout() {
        this.Size = Point.Zero;

        foreach (Label l in this.Labels) {
            this.Height += l.Padding.T;
            l.Position = this.Position + new Vector2(l.Padding.L, this.Height);
            this.Height += l.Height + l.Padding.B;
            if (l.Width + l.Padding.LR > this.Width) this.Width = l.Width + l.Padding.LR;
        }
    }
}
