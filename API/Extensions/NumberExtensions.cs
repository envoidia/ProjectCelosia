using API.Graphics;
using API.Util;

namespace API.Extensions;

public static class NumberExtensions
{
    public const string NumberFormat = "N";
    public const string IntegerFormat = "N0";

    extension(int @this)
    {
        /// <returns>
        /// The given <c>int</c> formatted based on the current locale
        /// </returns>
        /// <param name="color">The color to use</param>
        /// <param name="useSign">Whether to insert a <c>+</c> if the <c>int</c> is positive</param>
        /// <param name="suffix">Added after the formatted <c>int</c></param>
        /// <param name="divisor"><c>float</c> to divide the <c>int</c> by before displaying it</param>
        /// todo a lot of things need to be redone to account for how this auto appends white now
        public string Format(ThemeColor color, bool useSign = true, char? suffix = null, float divisor = 1f)
        {
            return color.Str + (useSign && @this > 0 ? '+' : null) + ((int) (@this / divisor)).ToString(IntegerFormat) +
            suffix + ThemeColor.White.Str;
        }

        /// <returns>
        /// The given <c>int</c> formatted based on the current locale, with no color
        /// </returns>
        public string FormatNoColor(bool useSign = true, char? suffix = null, float divisor = 1f)
        {
            return (useSign && @this > 0 ? '+' : null) + ((int) (@this / divisor)).ToString(IntegerFormat) + suffix;
        }


        /// <returns>
        /// The given <c>int</c> formatted based on the current locale
        /// </returns>
        /// <param name="threshold">Above this number is considered positive</param>
        /// <param name="isPositive">Whether to use green for > threshold</param>
        /// <param name="useSign">Whether to insert a <c>+</c> if the <c>int</c> is positive and use <c>isPositive</c> to pick the color</param>
        public string Format(bool useSign, int threshold = 0, bool isPositive = true)
        {
            if (!useSign)
            {
                return @this.Format(ThemeColor.White, false);
            }

            return @this.Format(TextLib.GetColor(@this, threshold, isPositive));
        }

        /// <returns>
        /// The given <c>int</c> formatted based on the current locale
        /// </returns>
        /// <param name="useSign">Whether to insert a <c>+</c> if the <c>int</c> is positive and color based off of that</param>
        public string Format(bool useSign = true)
        {
            return @this.Format(useSign, 0);
        }

        /// <returns>
        /// The given <c>int</c> formatted as a percentage. Expects 1000 = 100% with default params
        /// </returns>
        public string FormatPerc(bool useSign = false, int threshold = 1000, bool isPositive = true, float divisor = 10f)
        {
            return @this.Format(TextLib.GetColor(@this, threshold, isPositive), useSign, '%', divisor);
        }
    }

    extension(float @this)
    {
        /// <returns>
        /// The given <c>float</c> formatted based on the current locale
        /// </returns>
        /// <param name="threshold">Above this number is considered positive</param>
        /// <param name="isPositive">Whether to use green for > threshold</param>
        /// <param name="useSign">Whether to insert a <c>+</c> if the <c>float</c> is positive</param>
        /// <param name="suffix">Added after the formatted <c>float</c></param>
        /// <param name="divisor"><c>float</c> to divide the <c>float</c> by before displaying it</param>
        public string Format(int threshold, bool isPositive = true, bool useSign = true, char? suffix = null, float divisor = 1f)
        {
            return TextLib.GetColor(@this, threshold, isPositive).Str + (useSign && @this > 0 ? '+' : null) + (@this / divisor)
            .ToString(NumberFormat) + suffix + ThemeColor.White.Str;
        }

        /// <returns>
        /// The given <c>float</c> formatted as a percentage. Expects 1 = 100% with default params
        /// </returns>
        public string FormatPerc(bool useSign = false, int threshold = 1, bool isPositive = true, float divisor = 1f)
        {
            return @this.Format(threshold, isPositive, useSign, '%', divisor);
        }
    }
}