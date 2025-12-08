using System;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

// todo cleanup
public class GuiBox(int l, int r, int t, int b, float outlineThickness = 10, Priority priority = Priority.Normal)
    : Actor(priority), IAnimatedPrimitive {
    public int L { get; set; } = l;
    public int R { get; set; } = r;
    public int T { get; set; } = t;
    public int B { get; set; } = b;

    public Color Color { get; set; } = Color.Black;

    public float OutlineThickness { get; set; } = outlineThickness;
    public Color OutlineColor { get; set; } = Color.White;

    /// <summary>
    /// Move X by 1 for every slant Y
    /// </summary>
    public int SlantL { get; set; } = 6;

    /// <inheritdoc cref="SlantL" />
    public int SlantR { get; set; } = 6;

    public float Speed { get; set; } = 2f;

    public Progress Prog { get; set; } = new();

    public override void Draw(GameTime gameTime) {
        if (this.Prog != 0) this._DrawInternal();
    }

    protected void _DrawInternal(int l, int r, int t, int b, Color color, Progress prog) {
        float height = b - t;

        float angLOff = this.SlantL > 0 ? height / this.SlantL : 0;
        float angROff = this.SlantR > 0 ? height / this.SlantR : 0;

        Vector2 tl = new(l + angLOff, t);
        Vector2 tr = new(MathHelper.SmoothStep(tl.X, r + angROff, (float) prog), t);
        Vector2 bl = new(l, b);
        Vector2 br = new(MathHelper.SmoothStep(bl.X, r, (float) prog), b);

        Core.ShapeBatch.DrawTriangleStrip(tl, tr, bl, br, color, this.OutlineColor, this.OutlineThickness);
    }

    protected void _DrawInternal(int l, int r, int t, int b, Color color) =>
        this._DrawInternal(l, r, t, b, color, this.Prog);

    protected void _DrawInternal() =>
        this._DrawInternal(this.L, this.R, this.T, this.B, this.Color, this.Prog);
}

public static class GuiBoxes {
    /// <summary>
    /// <c>GuiBox</c> that covers most of the left half of the screen
    /// </summary>
    // todo how far offscreen is needed
    public static readonly GuiBox CoverLeft = new(8, 1750, 0, World.H) {
        Speed = 4f,
        SlantL = 0,
        Priority = Priority.High
    };
}