using System;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

// todo cleanup
public class GuiBox : Actor, IAnimatedPrimitive {
    public int L { get; set; }
    public int R { get; set; }
    public int T { get; set; }
    public int B { get; set; }

    public Color Color { get; set; } = Color.Black;

    public float OutlineThickness { get; set; }
    public Color OutlineColor { get; set; } = Color.White;

    /// <inheritdoc cref="AnimDirs" />
    public AnimDirs Dir { get; set; } = AnimDirs.Collapsing;

    /// <summary>
    /// Move X by 1 for every slant Y
    /// </summary>
    public int SlantL { get; set; } = 6;

    /// <inheritdoc cref="SlantL" />
    public int SlantR { get; set; } = 6;

    public float Speed { get; set; } = 2f;

    public Progress Prog { get; set; } = new();

    public GuiBox(Stage? stage, int l, int r, int t, int b, float outlineThickness = 10) {
        this.L = l;
        this.R = r;
        this.T = t;
        this.B = b;
        this.OutlineThickness = outlineThickness;
        stage?.Add(this);
    }

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
    /// <c>GuiBox</c> that covers most of the left half of the screen. Not part of any <c>Stage</c>, so draw manually
    /// </summary>
    // todo how far offscreen is needed
    public static readonly GuiBox CoverLeft = new(null, 8, 1750, 0, World.H) {
        Speed = 4f,
        SlantL = 0,
        RenderPriority = RenderPriority.High
    };
}