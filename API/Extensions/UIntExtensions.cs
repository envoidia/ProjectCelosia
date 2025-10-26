namespace API.Extensions;

public static class UIntExtensions {
    extension(uint val) {
        public bool ToBool() {
            return val != 0;
        }
    }
}