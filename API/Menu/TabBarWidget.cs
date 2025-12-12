using System;
using API.Graphics;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Menu;

public sealed class TabBarWidget : IWidget {
    public Label[] Labels { get; }

    /// <summary>
    /// Animation progress per-item
    /// </summary>
    public Progress[] Progs { get; }

    public int Index { get; set; } = 0;
    public int MaxIndex { get; }

    public Menu Menu { get; }

    public WidgetSelectionType PrefDir => WidgetSelectionType.Horiz;
    public WidgetSelectionType CurDir { get; set; }

    public Action? OnSelect { get; set; }

    public ActorData Data { get; }

    private const int _OutlineWidth = 10;
    private const int _YOffset = 9;

    public TabBarWidget(Menu menu, Vector2 pos, params string[] optionText) {
        this.Menu = menu;
        this.Data = new ActorData(this, RenderPriority.B2Med);

        this.Position = pos;
        this.Size = Point.Zero;

        this.Labels = new Label[optionText.Length];

        for (int i = 0; i < optionText.Length; i++) this.Labels[i] = new Label() {
            Text = optionText[i],
            Padding = new(30, 20)
        };

        this.Progs = new Progress[optionText.Length];
        this.MaxIndex = optionText.Length;

        this.CalcLayout();
    }

    public void Create() { }
    public void Destroy() => this.MarkForRemoval();

    public void Draw(GameTime gameTime) {
        this.Index = MenuLib.CheckMovement1D(this.Index, this.MaxIndex, this.CurDir);

        int w = 0;
        for (int i = 0; i < this.Progs.Length; i++) {
            this.Progs[i] = RenderLib.UpdateProg(this.Progs[i], 2f, gameTime,
                i == this.Index ? AnimDirs.In : AnimDirs.Out);

            float yOff = (float) this.Progs[i] * _YOffset;

            RenderLib.DrawParallelogram(
                new Vector2(this.Position.X + w - this.Padding.L, this.Position.Y - this.Padding.T - yOff),

                new Point(this.Labels[i].Width + this.Labels[i].Padding.LR - _OutlineWidth,
                (int) (this.Height + this.Padding.TB + yOff * 2)),

                this.Origin, Settings.ColorBg, Settings.ColorFg,
                _OutlineWidth, 6, 6, new Progress(1f));

            if (this.Progs[i] != 0) {
                // Cursor
                RenderLib.DrawParallelogram(
                    new Vector2(this.Position.X + w - this.Padding.L, this.Position.Y - this.Padding.T - yOff),

                    new Point(this.Labels[i].Width + this.Labels[i].Padding.LR - _OutlineWidth,
                    (int) (this.Height + this.Padding.TB + yOff * 2)),

                    this.Origin, Settings.ColorAccent, Settings.ColorAccent,
                    0f, 6, 6, this.Progs[i]);
            }

            w += this.Labels[i].Width + this.Labels[i].Padding.LR;
        }

        foreach (Label l in this.Labels) l.Data.Act(gameTime);
    }

    public void CalcLayout() {
        this.Size = Point.Zero;

        foreach (Label l in this.Labels) {
            this.Width += l.Padding.L;
            l.Position = new Vector2(this.X + this.Width, this.Y + l.Padding.T);
            this.Width += l.Width + l.Padding.R;
            if (l.Height + l.Padding.TB > this.Height) this.Height = l.Height + l.Padding.TB;
        }
    }
}
