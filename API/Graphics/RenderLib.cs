using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public static class RenderLib {
    public static void DrawParallelogram(int l, int r, int t, int b, Color color, Color outlineColor,
        float outlineThickness, int slantL, int slantR, Progress prog) {
        float height = b - t;

        float angLOff = slantL > 0 ? height / slantL : 0;
        float angROff = slantR > 0 ? height / slantR : 0;

        Vector2 tl = new(l + angLOff, t);
        Vector2 tr = new(MathHelper.SmoothStep(tl.X, r + angROff, (float) prog), t);
        Vector2 bl = new(l, b);
        Vector2 br = new(MathHelper.SmoothStep(bl.X, r, (float) prog), b);

        Core.ShapeBatch.DrawTriangleStrip(tl, tr, bl, br, color, outlineColor, outlineThickness);
    }
}
