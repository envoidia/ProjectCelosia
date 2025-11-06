using API.Graphics;

namespace API.Extensions;

public static class NumberExtensions {
    public const string NumberFormat = "N";

    extension(int val) {
        public string Format(string color, string suffix = "", float divisor = 1) {
            string sign = val > 0 ? "+" : "";
            return color + sign + (val / divisor).ToString(NumberFormat) + suffix;
        }

        public string Format(bool invertColors) {
            string pos = invertColors ? Colors.Neg : Colors.Pos;
            string neg = invertColors ? Colors.Pos : Colors.Neg;

            return val.Format(val > 0 ? pos : val < 0 ? neg : Colors.Num);
        }

        public string Format() => val.Format(false);
    }

    extension(uint val) {
        public string Format(string color, string suffix = "", float divisor = 1) {
            string sign = val == 0 ? "" : "+";
            return color + sign + (val / divisor).ToString(NumberFormat) + suffix;
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