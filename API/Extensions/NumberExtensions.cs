using API.Graphics;
using API.Util;

namespace API.Extensions;

// todo can i use a mock union (struct with explicit layout) as input
public static class NumberExtensions {
    public const string NumberFormat = "N";
    public const string IntegerFormat = "N0";

    extension(int @this) {
        public string Format(string color, string suffix = "", float divisor = 1) {
            string sign = @this > 0 ? "+" : "";
            return color + sign + ((int) (@this / divisor)).ToString(IntegerFormat) + suffix;
        }

        public string Format(bool isPositive) {
            (string pos, string neg) = TextLib.GetColors(isPositive);

            return @this.Format(@this > 0 ? pos : @this < 0 ? neg : Colors.Num);
        }

        public string Format() => @this.Format(true);
    }

    extension(uint @this) {
        public string Format(string color, string suffix = "", float divisor = 1) {
            string sign = @this == 0 ? "" : "+";
            return color + sign + ((int) (@this / divisor)).ToString(IntegerFormat) + suffix;
        }

        public string Format() => @this.Format(Colors.Num);

        public string Format(uint threshold) =>
            @this.Format(@this > threshold ? Colors.Pos : @this < threshold ? Colors.Neg : Colors.Num);
    }

    extension(float @this) {
        public string Format(string color, string suffix = "", float divisor = 1) {
            string sign = @this > 0 ? "+" : "";
            return color + sign + (@this / divisor).ToString(NumberFormat) + suffix;
        }
    }
}