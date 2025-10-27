namespace API.Extensions;

public static class BoolExtensions {
    extension(bool val) {
        public int ToInt() => val ? 1 : 0;

        public int ToSign() => val ? 1 : -1;
    }
}