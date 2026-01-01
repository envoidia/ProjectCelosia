using Apos.Shapes;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public static class ShapeBatchExtensions
{
    private static readonly Vector2 _TR = new(+1, -1);
    private static readonly Vector2 _BL = new(-1, +1);

    extension(ShapeBatch @this)
    {
        /// <summary>
        /// Not really a triangle strip, but we're gonna pretend it is
        /// </summary>
        public void DrawTriangleStrip(Vector2 tl, Vector2 tr, Vector2 bl, Vector2 br,
            Color fill, Color border, float thickness)
        {
            // Outline
            if (thickness != 0)
            {
                Vector2 t = new(thickness);

                Vector2 tlo = tl - t;
                Vector2 tro = tr + (t * _TR);
                Vector2 blo = bl + (t * _BL);
                Vector2 bro = br + t;

                @this.DrawTriangle(tlo, tro, blo, border, Color.Red, 0);
                @this.DrawTriangle(tro, blo, bro, border, Color.Red, 0);
            }

            @this.DrawTriangle(tl, tr, bl, fill, Color.Red, 0);
            @this.DrawTriangle(tr, bl, br, fill, Color.Red, 0);
        }
    }
}