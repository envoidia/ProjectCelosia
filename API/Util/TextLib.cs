using API.Extensions;
using API.Graphics;

namespace API.Util;

public static class TextLib {
    public static (string, string) GetColors(bool isPositive) =>
        isPositive ? (ColorCode.Pos, ColorCode.Neg) : (ColorCode.Neg, ColorCode.Pos);

    /// <returns>
    /// The color that should be used when the value is increased beyond normal, depending on if it is desirable
    /// </returns>
    public static ColorCode GetIncColor(bool isPositive) => isPositive ? ColorCode.Pos : ColorCode.Neg;

    /// <returns>
    /// The color that should be used when the value is decreased below normal, depending on if it is desirable
    /// </returns>
    public static ColorCode GetDecColor(bool isPositive) => isPositive ? ColorCode.Pos : ColorCode.Neg;

    public static string FormatStat(int stat, int statDefault) =>
        stat.Format(stat > statDefault ? ColorCode.Pos : stat < statDefault ? ColorCode.Neg : ColorCode.Num, false);
}