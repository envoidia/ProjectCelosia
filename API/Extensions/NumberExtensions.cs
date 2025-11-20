using API.Graphics;
using API.Util;

namespace API.Extensions;

public static class NumberExtensions {
    public const string NumberFormat = "N";
    public const string IntegerFormat = "N0";

    extension(int @this) {
        public string Format(string color, bool useSign = true, string suffix = "", float divisor = 1) {
            string sign = useSign && @this > 0 ? "+" : "";
            return color + sign + ((int) (@this / divisor)).ToString(IntegerFormat) + suffix;
        }

        public string Format(bool isPositive, bool useSign) {
            if(!useSign) return @this.Format(Colors.White, useSign: false);

            (string pos, string neg) = TextLib.GetColors(isPositive);

            return @this.Format(@this > 0 ? pos : @this < 0 ? neg : Colors.Num);
        }

        public string Format(bool useSign = true) => @this.Format(true, useSign);
    }

    extension(float @this) {
        public string Format(string color, string suffix = "", float divisor = 1) {
            string sign = @this > 0 ? "+" : "";
            return color + sign + (@this / divisor).ToString(NumberFormat) + suffix;
        }
    }
}