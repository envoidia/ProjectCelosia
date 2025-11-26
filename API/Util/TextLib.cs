using API.Extensions;
using API.Graphics;

namespace API.Util;

public static class TextLib {
    public static (string, string) GetColors(bool isPositive) =>
        isPositive ? (Colors.Pos, Colors.Neg) : (Colors.Neg, Colors.Pos);

    public static string FormatStat(int stat, int statDefault) =>
        stat.Format(stat > statDefault ? Colors.Pos : stat < statDefault ? Colors.Neg : Colors.Num, false);
}