using System;
using Apos.Shapes;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public static class ShapeBatchExtensions
{
    extension(ShapeBatch @this)
    {
        public void DrawQuad(Vector2 tl, Vector2 tr, Vector2 bl, Vector2 br,
            Color fill, Color border, float thickness)
        {
            // Outline
            if (thickness != 0)
            {
                Vector2 offset_tl = getOffset(tl, bl, tr);
                Vector2 offset_tr = getOffset(tr, tl, br);
                Vector2 offset_br = getOffset(br, tr, bl);
                Vector2 offset_bl = getOffset(bl, br, tl);

                Vector2 tlo = tl + offset_tl;
                Vector2 tro = tr + offset_tr;
                Vector2 bro = br + offset_br;
                Vector2 blo = bl + offset_bl;

                @this.DrawTriangle(tlo, tro, blo, border, Color.Red, 0);
                @this.DrawTriangle(tro, blo, bro, border, Color.Red, 0);
            }

            // Fill
            @this.DrawTriangle(tl, tr, bl, fill, Color.Red, 0);
            @this.DrawTriangle(tr, bl, br, fill, Color.Red, 0);

            Vector2 getOffset(Vector2 curr, Vector2 prev, Vector2 next)
            {
                Vector2 edgeIn = curr - prev;
                Vector2 edgeOut = next - curr;

                // Outward normals
                Vector2 nIn = new(edgeIn.Y, -edgeIn.X);
                Vector2 nOut = new(edgeOut.Y, -edgeOut.X);

                float lenIn = (float) Math.Sqrt(nIn.X * nIn.X + nIn.Y * nIn.Y);
                float lenOut = (float) Math.Sqrt(nOut.X * nOut.X + nOut.Y * nOut.Y);

                const float Tolerance = 0.0001f;
                if (lenIn > Tolerance)
                {
                    nIn = new(nIn.X / lenIn, nIn.Y / lenIn);
                }
                if (lenOut > Tolerance)
                {
                    nOut = new(nOut.X / lenOut, nOut.Y / lenOut);
                }

                float dot = nIn.X * nOut.X + nIn.Y * nOut.Y;
                float k = thickness / (1f + dot);

                Vector2 sum = new(nIn.X + nOut.X, nIn.Y + nOut.Y);
                return new(sum.X * k, sum.Y * k);
            }
        }
    }
}