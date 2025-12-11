using System;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

// todo rename
public sealed class GuiBoxBar(int l, int r, int t, int b, RenderPriority priority = RenderPriority.B1Med,
    params Color[] colors) : Parellelogram(l, r, t, b, renderPriority: priority) {
    public Progress[] BarProgs { get; set; } = new Progress[colors.Length];
    public Color[] BarColors { get; } = colors;

    public override void Draw(GameTime gameTime) {
        if (this.Prog == 0) return;

        draw(this.L, this.R, this.T, this.B, this.Color);

        // Draw overlay bars from longest to shortest so they're all visible
        Array.Sort(this.BarProgs, this.BarColors);

        for (int i = this.BarProgs.Length - 1; i >= 0; i--) {
            draw(this.L, (int) (this.L + ((this.R - this.L) *
                Math.Min((float) this.BarProgs[i], (float) this.Prog))), this.T, this.B,
                this.BarColors[i]);
        }

        void draw(int l, int r, int t, int b, Color color) =>
            RenderLib.DrawParallelogram(l, r, t, b, color, this.OutlineColor,
                this.OutlineThickness, this.SlantL, this.SlantR, this.Prog);
    }
}