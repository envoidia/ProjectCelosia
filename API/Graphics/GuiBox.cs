using System;
using Microsoft.Xna.Framework;

namespace API.Graphics;

// todo cleanup
public class GuiBox(int l, int r, int t, int b, float outlineThickness = 10) {
    public int L { get; set; } = l;
    public int R { get; set; } = r;
    public int T { get; set; } = t;
    public int B { get; set; } = b;

    public Color Color { get; set; } = Color.Black;

    public float OutlineThickness { get; set; } = outlineThickness;
    public Color OutlineColor { get; set; } = Color.White;

    // Animation progress 0-1
    public float Prog { get; set; } = 0;

    // 1 = unfolding, -1 = collapsing
    public int Dir { get; set; } = -1;

    // Speed multiplier. 1f = animation completes in 1s. 2f = 0.5s. Speed is doubled when closing
    public float Speed { get; set; } = 2;

    // Move X by 1 for every slant Y
    public int SlantL { get; set; } = 6;
    public int SlantR { get; set; } = 6;

    // todo will this need a setter
    public RenderPriority RenderPriority { get; set; } = RenderPriority.Low;

    public virtual void Draw(GameTime gameTime) {
        this.Update(gameTime);

        if (this.Prog != 0) this.DrawWithoutUpdate();
    }

    protected void Update(GameTime gameTime) =>
        this.Prog = Math.Clamp(this.Prog + (float) (gameTime.ElapsedGameTime.TotalSeconds * this.Dir * this.Speed *
                                                    (this.Dir == -1 ? 2 : 1)), 0f, 1f);

    public void DrawWithoutUpdate(int l, int r, int t, int b, Color color, float prog) {
        float height = b - t;

        float angLOff = this.SlantL > 0 ? height / this.SlantL : 0;
        float angROff = this.SlantR > 0 ? height / this.SlantR : 0;

        Vector2 tl = new(l + angLOff, t);
        Vector2 tr = new(MathHelper.SmoothStep(tl.X, r + angROff, prog), t);
        Vector2 bl = new(l, b);
        Vector2 br = new(MathHelper.SmoothStep(bl.X, r, prog), b);

        Core.ShapeBatch.DrawTriangleStrip(tl, tr, bl, br, color, this.OutlineColor, this.OutlineThickness);
    }

    public void DrawWithoutUpdate(int l, int r, int t, int b, Color color) =>
        this.DrawWithoutUpdate(l, r, t, b, color, this.Prog);

    public void DrawWithoutUpdate() => this.DrawWithoutUpdate(this.L, this.R, this.T, this.B, this.Color, this.Prog);
}