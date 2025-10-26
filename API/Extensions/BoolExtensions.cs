namespace API.Extensions;

public static class BoolExtensions {
    extension(bool val) {
        public uint ToUInt() {
            return val ? 1u : 0u;
        }

        public int ToSign() {
            return val ? 1 : -1;
        }
    }
}