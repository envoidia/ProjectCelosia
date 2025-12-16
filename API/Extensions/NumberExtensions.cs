using API.Graphics;
using API.Util;

namespace API.Extensions;

public static class NumberExtensions {
    public const string NumberFormat = "N";
    public const string IntegerFormat = "N0";

    extension(int @this) {
        /// <returns>
        /// The given <c>int</c> formatted based on the current locale
        /// </returns>
        /// <param name="color">The color to use</param>
        /// <param name="useSign">Whether to insert a <c>+</c> if the <c>int</c> is positive</param>
        /// <param name="suffix">Added after the formatted <c>int</c></param>
        /// <param name="divisor"><c>float</c> to divide the <c>int</c> by before displaying it</param>
        /// todo a lot of things need to be redone to account for how this auto appends white now
        public string Format(ColorCode color, bool useSign = true, char? suffix = null, float divisor = 1f) =>
            color + (useSign && @this > 0 ? '+' : null) + ((int) (@this / divisor)).ToString(IntegerFormat) +
            suffix + ColorCode.White;

        /// <returns>
        /// The given <c>int</c> formatted based on the current locale, with no color
        /// </returns>
        public string FormatNoColor(bool useSign = true, char? suffix = null, float divisor = 1f) =>
            (useSign && @this > 0 ? '+' : null) + ((int) (@this / divisor)).ToString(IntegerFormat) + suffix;


        /// <returns>
        /// The given <c>int</c> formatted based on the current locale
        /// </returns>
        /// <param name="threshold">Above this number is considered positive</param>
        /// <param name="isPositive">Whether to use green for > threshold</param>
        /// <param name="useSign">Whether to insert a <c>+</c> if the <c>int</c> is positive and use <c>isPositive</c> to pick the color</param>
        public string Format(bool useSign, int threshold = 0, bool isPositive = true) {
            if (!useSign) return @this.Format(ColorCode.White, false);
            return @this.Format(TextLib.GetColor(@this, threshold, isPositive));
        }

        /// <returns>
        /// The given <c>int</c> formatted based on the current locale
        /// </returns>
        /// <param name="useSign">Whether to insert a <c>+</c> if the <c>int</c> is positive</param>
        public string Format(bool useSign = true) => @this.Format(useSign, 0);

        /// <returns>
        /// The given <c>int</c> formatted as a percentage. Expects 1000 = 100% with default params
        /// </returns>
        public string FormatPerc(bool useSign = false, int threshold = 1000, bool isPositive = true, float divisor = 10f) =>
            @this.Format(TextLib.GetColor(@this, threshold, isPositive), useSign, '%', divisor);
    }

    extension(float @this) {
        /// <returns>
        /// The given <c>float</c> formatted based on the current locale
        /// </returns>
        /// <param name="threshold">Above this number is considered positive</param>
        /// <param name="isPositive">Whether to use green for > threshold</param>
        /// <param name="useSign">Whether to insert a <c>+</c> if the <c>float</c> is positive</param>
        /// <param name="suffix">Added after the formatted <c>float</c></param>
        /// <param name="divisor"><c>float</c> to divide the <c>float</c> by before displaying it</param>
        public string Format(int threshold, bool isPositive = true, bool useSign = true, char? suffix = null, float divisor = 1f) =>
            TextLib.GetColor(@this, threshold, isPositive) + (useSign && @this > 0 ? '+' : null) + (@this / divisor)
            .ToString(NumberFormat) + suffix + ColorCode.White;

        /// <returns>
        /// The given <c>float</c> formatted as a percentage. Expects 1 = 100% with default params
        /// </returns>
        public string FormatPerc(bool useSign = false, int threshold = 1, bool isPositive = true, float divisor = 1f) =>
            @this.Format(threshold, isPositive, useSign, '%', divisor);
    }
}