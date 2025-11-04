using API.Graphics;

namespace API.Extensions;

public static class NumberExtensions {
    public const string NumberFormat = "N";

    extension(int val) {
        public string Format(string color, string suffix = "", double? divisor = null) {
            string sign = val > 0 ? "+" : "";

            if (divisor != null) return color + sign + ((double) (val / divisor)).ToString(NumberFormat) + suffix;

            return color + sign + val.ToString(NumberFormat) + suffix;
        }

        public string Format(bool invertColors) {
            string pos = invertColors ? Colors.Neg : Colors.Pos;
            string neg = invertColors ? Colors.Pos : Colors.Neg;

            return val.Format(val > 0 ? pos : val < 0 ? neg : Colors.Num);
        }

        public string Format() => val.Format(false);
    }

    extension(uint val) {
        public string Format(string color, string suffix = "", double divisor = 1) {
            string sign = val == 0 ? "" : "+";
            return color + sign + (val / divisor).ToString(NumberFormat) + suffix;
        }

        public string Format() => val.Format(Colors.Num);

        public string FormatStat(uint statDefault) =>
            val > statDefault ? Colors.Pos : val < statDefault ? Colors.Neg : Colors.Num;
    }

    extension(double val) {
        public string Format(string color, string suffix = "", double divisor = 1) {
            string sign = val > 0 ? "+" : "";
            return color + sign + (val / divisor).ToString(NumberFormat) + suffix;
        }
    }
}