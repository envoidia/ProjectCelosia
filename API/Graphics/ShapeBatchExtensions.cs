using Apos.Shapes;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public static class ShapeBatchExtensions {
    extension(ShapeBatch @this) {
        // Not really a triangle strip but we're gonna pretend it is
        public void DrawTriangleStrip(Vector2 tl, Vector2 tr, Vector2 bl, Vector2 br, Color fill, Color border,
            float thickness) {
            @this.DrawTriangle(tl, tr, bl, fill, fill, 0);
            @this.DrawTriangle(tr, bl, br, fill, fill, 0);

            // Outline
            if (thickness == 0) return;
            @this.DrawLine(tl, tr, thickness, border, border, 0);
            @this.DrawLine(tr, br, thickness, border, border, 0);
            @this.DrawLine(br, bl, thickness, border, border, 0);
            @this.DrawLine(bl, tl, thickness, border, border, 0);
        }
    }
}