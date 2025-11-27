using System;
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
        public string Format(string color, bool useSign = true, char? suffix = null, float divisor = 1f) =>
            color + (useSign && @this > 0 ? '+' : null) + ((int) (@this / divisor)).ToString(IntegerFormat) + suffix;

        /// <returns>
        /// The given <c>int</c> formatted based on the current locale
        /// </returns>
        /// <param name="isPositive">Whether to insert a <c>+</c> if the <c>int</c> is positive</param>
        /// <param name="useSign">Whether to insert a <c>+</c> if the <c>int</c> is positive and use <c>isPositive</c> to pick the color</param>
        public string Format(bool isPositive, bool useSign) {
            if (!useSign) return @this.Format(Colors.White, useSign: false);

            return @this switch {
                > 0 => @this.Format(TextLib.GetIncColor(isPositive)),
                < 0 => @this.Format(TextLib.GetDecColor(isPositive)),
                _ => @this.Format(Colors.Num)
            };
        }

        /// <returns>
        /// The given <c>int</c> formatted based on the current locale
        /// </returns>
        /// <param name="useSign">Whether to insert a <c>+</c> if the <c>int</c> is positive</param>
        public string Format(bool useSign = true) => @this.Format(true, useSign);
    }

    extension(float @this) {
        /// <returns>
        /// The given <c>float</c> formatted based on the current locale
        /// </returns>
        /// <param name="color">The color to use</param>
        /// <param name="useSign">Whether to insert a <c>+</c> if the <c>float</c> is positive</param>
        /// <param name="suffix">Added after the formatted <c>float</c></param>
        /// <param name="divisor"><c>float</c> to divide the <c>float</c> by before displaying it</param>
        public string Format(string color, bool useSign = true, char? suffix = null, float divisor = 1f) =>
            color + (useSign && @this > 0 ? '+' : null) + (@this / divisor).ToString(NumberFormat) + suffix;
    }
}