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
public sealed class ListWidget : IWidget {
    public List<Label> Labels { get; private set; }

    /// <summary>
    /// Animation progress per-item
    /// </summary>
    public Progress[] Progs { get; private set; }

    public int Index { get; set; } = 0;
    public int MaxIndex { get; private set; }

    public Menu Menu { get; }

    public WidgetSelectionType PrefDir => WidgetSelectionType.Vert;
    public WidgetSelectionType CurDir { get; set; }

    public Action? OnSelect { get; set; }

    public ActorData Data { get; }

    public ListWidget(Menu menu, Vector2 pos, params string[] optionText) {
        this.Menu = menu;
        this.Data = new ActorData(this, RenderPriority.B2Med);

        this.Position = pos;
        this.Size = Point.Zero;

        this.Labels = new(optionText.Length);

        for (int i = 0; i < optionText.Length; i++) this.Labels.Add(new Label() {
            Text = optionText[i],
            Padding = new(40, 20, 20, 20)
        });

        this.Progs = new Progress[optionText.Length];
        this.MaxIndex = optionText.Length;

        this.CalcLayout();
    }

    public void SetText(params string[] optionText) {
        int i = 0;
        for (; i < optionText.Length && i < this.Labels.Count; i++) this.Labels[i].Text = optionText[i];

        // New list shorter, blank out remaining Labels
        for (; i < this.Labels.Count; i++) this.Labels[i].Text = "";

        // New list longer, add more Labels
        for (; i < optionText.Length; i++) this.Labels.Add(new Label() {
            Text = optionText[i],
            Padding = new(40, 20, 20, 20)
        });

        this.Progs = new Progress[optionText.Length];
        this.MaxIndex = optionText.Length;

        this.CalcLayout();
    }

    public void Create() { }
    public void Destroy() => this.MarkForRemoval();

    public void Draw(GameTime gameTime) {
        this.Index = MenuLib.CheckMovement1D(this.Index, this.MaxIndex, this.CurDir);

        int h = 0;
        for (int i = 0; i < this.Progs.Length; i++) {
            this.Progs[i] = RenderLib.UpdateProg(this.Progs[i], 2f, gameTime,
                i == this.Index ? AnimDirs.In : AnimDirs.Out);

            if (this.Progs[i] != 0) {
                // Cursor
                RenderLib.DrawParallelogram(new Vector2(this.Position.X - this.Padding.L,
                    this.Position.Y + h - this.Padding.T),
                    new Point(this.Width + this.Padding.LR, this.Labels[i].Height +
                    this.Labels[i].Padding.TB + this.Padding.TB),
                    this.Origin, Settings.ColorAccent,
                    Settings.ColorAccent, 0f, 6, 6, this.Progs[i]);
            }

            h += this.Labels[i].Height + this.Labels[i].Padding.TB;
        }

        foreach (Label l in this.Labels) {
            l.Data.Act(gameTime);
            if(DebugMenu._drawActorOutlines) l.Data.DrawDebug();
        }
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
