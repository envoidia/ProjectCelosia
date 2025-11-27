namespace API.Extensions;

public static class BoolExtensions {
    extension(bool @this) {
        /// <returns>
        /// 1 if true, otherwise 0
        /// </returns>
        public int ToInt() => @this ? 1 : 0;

        /// <returns>
        /// 1 if true, otherwise -1
        /// </returns>
        public int ToSign() => @this ? 1 : -1;
    }
}