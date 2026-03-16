using System;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public static class RenderLib
{
    public const int DefaultSlant = 6;
    public const int UnitSpriteSize = 384;
    public const int BgOutlineThickness = 10;

    public static Progress UpdateProg(Progress prog, float speed, GameTime gt, AnimDirs dir)
    {
        return prog + (float) (gt.ElapsedGameTime.TotalSeconds * (int) dir * speed *
            (1 + Convert.ToInt32((int) dir == -1)));
    }


    // todo deprecate
    public static void DrawParallelogram(int l, int r, int t, int b, Color color, Color outlineColor,
        float outlineThickness, int slantL, int slantR, Progress prog)
    {
        float height = b - t;

        float angLOff = slantL > 0 ? height / slantL : 0;
        float angROff = slantR > 0 ? height / slantR : 0;

        Vector2 tl = new(l + angLOff, t);
        Vector2 tr = new(MathHelper.SmoothStep(tl.X, r + angROff, (float) prog), t);
        Vector2 bl = new(l, b);
        Vector2 br = new(MathHelper.SmoothStep(bl.X, r, (float) prog), b);

        Core.ShapeBatch.DrawTriangleStrip(tl, tr, bl, br, color, outlineColor, outlineThickness);
    }

    public static void DrawParallelogram(Vector2 pos, Point size, Point origin, Color color, Color outlineColor,
        float outlineThickness, int slantL, int slantR, Progress prog)
    {
        pos -= origin.ToVector2();

        float angLOff = slantL > 0 ? size.Y / slantL : 0;
        float angROff = slantR > 0 ? size.Y / slantR : 0;

        Vector2 tl = new(pos.X + angLOff, pos.Y);
        Vector2 tr = new(MathHelper.SmoothStep(tl.X, (pos.X + size.X) + angROff,
            (float) prog), pos.Y);
        Vector2 bl = new(pos.X, pos.Y + size.Y);
        Vector2 br = new(MathHelper.SmoothStep(bl.X, pos.X + size.X,
            (float) prog), pos.Y + size.Y);

        Core.ShapeBatch.DrawTriangleStrip(tl, tr, bl, br, color, outlineColor, outlineThickness);
    }
}
