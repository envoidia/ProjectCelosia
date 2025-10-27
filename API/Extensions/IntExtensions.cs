namespace API.Extensions;

public static class IntExtensions {
    extension(int val) {
        // Traditionally this would be != 0, but for my purposes this is better
        public bool ToBool() => val > 0;

        public string Format() => "todo";
        // todo apply sign, color, commas
    }
}