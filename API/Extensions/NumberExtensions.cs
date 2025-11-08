using API.Graphics;
using API.Util;

namespace API.Extensions;

public static class NumberExtensions {
    public const string NumberFormat = "N";
    public const string IntegerFormat = "N0";

    extension(int val) {
        public string Format(string color, string suffix = "", float divisor = 1) {
            string sign = val > 0 ? "+" : "";
            return color + sign + ((int) (val / divisor)).ToString(IntegerFormat) + suffix;
        }

        public string Format(bool isPositive) {
            (string pos, string neg) = TextLib.GetColors(isPositive);

            return val.Format(val > 0 ? pos : val < 0 ? neg : Colors.Num);
        }

        public string Format() => val.Format(true);
    }

    extension(uint val) {
        public string Format(string color, string suffix = "", float divisor = 1) {
            string sign = val == 0 ? "" : "+";
            return color + sign + ((int) (val / divisor)).ToString(IntegerFormat) + suffix;
        }

        public string Format() => val.Format(Colors.Num);

        public string Format(uint threshold) =>
            val.Format(val > threshold ? Colors.Pos : val < threshold ? Colors.Neg : Colors.Num);
    }

    extension(float val) {
        public string Format(string color, string suffix = "", float divisor = 1) {
            string sign = val > 0 ? "+" : "";
            return color + sign + (val / divisor).ToString(NumberFormat) + suffix;
        }
    }
}