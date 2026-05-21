using API.Extensions;
using API.Graphics;

namespace API.Util;

public static class TextLib
{
    /// <returns>
    /// The color that should be used for the current value, depending on its relation to a threshold and if it is desirable
    /// </returns>
    public static ThemeColor GetColor(float val, int threshold, bool isPositive = true)
    {
        return val > threshold ? GetIncColor(isPositive) : val < threshold ? GetDecColor(isPositive) : ThemeColor.Emphasis;
    }

    /// <returns>
    /// The color that should be used when the value is increased beyond normal, depending on if it is desirable
    /// </returns>
    public static ThemeColor GetIncColor(bool isPositive)
    {
        return isPositive ? ThemeColor.Positive : ThemeColor.Negative;
    }

    /// <returns>
    /// The color that should be used when the value is decreased below normal, depending on if it is desirable
    /// </returns>
    public static ThemeColor GetDecColor(bool isPositive)
    {
        return isPositive ? ThemeColor.Negative : ThemeColor.Positive;
    }

    /// <summary>
    /// Formats a number based off of whether it exceeds the given threshold
    /// </summary>
    public static string FormatStat(int stat, int statDefault)
    {
        return stat.Format(stat > statDefault ? ThemeColor.Positive : stat < statDefault ? ThemeColor.Negative : ThemeColor.Emphasis, false);
    }
}
