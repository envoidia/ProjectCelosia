using API.Graphics;

namespace API.Util;

public static class TextLib {
    public static (string, string) GetColors(bool isPositive) =>
        isPositive ? (Colors.Pos, Colors.Neg) : (Colors.Neg, Colors.Pos);
}