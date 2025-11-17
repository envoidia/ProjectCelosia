using System;
using Microsoft.Xna.Framework;

namespace API.Graphics;

// todo cleanup
public sealed class GuiBoxBar(int l, int r, int t, int b, params Color[] colors) : GuiBox(l, r, t, b, 0) {
    public float[] BarProgs { get; set; } = new float[colors.Length];
    public Color[] BarColors { get; } = colors;

    public override void Draw(GameTime gameTime) {
        this.Update(gameTime);

        if (this.Prog == 0) return;

        this.DrawWithoutUpdate(this.L, this.R, this.T, this.B, this.Color);

        // Draw overlay bars from longest to shortest so they're all visible
        // todo do i need copies
        //float[] progsSorted = Arrays.copyOf(barProgs, barProgs.length);
        //Color[] colorsSorted = Arrays.copyOf(barColors, barColors.length);

        Array.Sort(this.BarProgs, this.BarColors);

        for (int i = this.BarProgs.Length - 1; i >= 0; i--) {
            this.DrawWithoutUpdate(this.L, (int) (this.L + ((this.R - this.L) * Math.Min(this.BarProgs[i], this.Prog))),
                this.T, this.B, this.BarColors[i], this.Prog);
        }
    }
}