using API.Extensions;
using API.Graphics;

namespace API.Util;

public static class TextLib {
    public static (string, string) GetColors(bool isPositive) =>
        isPositive ? (Colors.Pos, Colors.Neg) : (Colors.Neg, Colors.Pos);

    /// <returns>
    /// The color that should be used when the value is increased beyond normal, depending on if it is desirable
    /// </returns>
    public static string GetIncColor(bool isPositive) => isPositive ? Colors.Pos : Colors.Neg;

    /// <returns>
    /// The color that should be used when the value is decreased below normal, depending on if it is desirable
    /// </returns>
    public static string GetDecColor(bool isPositive) => isPositive ? Colors.Pos : Colors.Neg;

    public static string FormatStat(int stat, int statDefault) =>
        stat.Format(stat > statDefault ? Colors.Pos : stat < statDefault ? Colors.Neg : Colors.Num, false);
}