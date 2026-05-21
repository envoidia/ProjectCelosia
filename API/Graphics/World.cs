using Microsoft.Xna.Framework;

namespace API.Graphics;

public static class World
{
    public const int W = 3840;
    public const int H = 2160;
    public const int W2 = 1920;
    public const int H2 = 1080;
    public static readonly Vector2 Vec = new(W, H);

    // todo allow changing
    public const int WindowW = 2560;
    public const int WindowH = 1440;
    public static readonly Vector2 WindowVec = new(WindowW, WindowH);
}