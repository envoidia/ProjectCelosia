using API.Extensions;
using API.Graphics;

namespace API.Util;

public static class TextLib {
    /// <returns>
    /// The color that should be used for the current value, depending on its relation to a threshold and if it is desirable
    /// </returns>
    public static ColorCode GetColor(float val, int threshold, bool isPositive = true) =>
        val > threshold ? GetIncColor(isPositive) : val < threshold ? GetDecColor(isPositive) : ColorCode.Num;

    /// <returns>
    /// The color that should be used when the value is increased beyond normal, depending on if it is desirable
    /// </returns>
    public static ColorCode GetIncColor(bool isPositive) => isPositive ? ColorCode.Pos : ColorCode.Neg;

    /// <returns>
    /// The color that should be used when the value is decreased below normal, depending on if it is desirable
    /// </returns>
    public static ColorCode GetDecColor(bool isPositive) => isPositive ? ColorCode.Neg : ColorCode.Pos;

    /// <summary>
    /// Formats a number based off of whether it exceeds the given threshold
    /// </summary>
    public static string FormatStat(int stat, int statDefault) =>
        stat.Format(stat > statDefault ? ColorCode.Pos : stat < statDefault ? ColorCode.Neg : ColorCode.Num, false);
}