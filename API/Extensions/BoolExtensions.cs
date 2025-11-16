namespace API.Extensions;

public static class BoolExtensions {
    extension(bool @this) {
        public int ToInt() => @this ? 1 : 0;

        public int ToSign() => @this ? 1 : -1;

        //public explicit operator int(bool @this) => @this ? 1 : 0; todo
    }
}