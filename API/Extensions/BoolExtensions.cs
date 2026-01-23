namespace API.Extensions;

public static class BoolExtensions
{
    extension(bool @this)
    {
        /// <returns>
        /// 1 if true, otherwise -1
        /// </returns>
        public int ToSign()
        {
            return @this ? 1 : -1;
        }

        /// <returns>The given <c>string</c> parsed as a <c>bool</c>, or the provided default if there was an error</returns>
        public static bool ParseOrDefault(string? str, bool defaultValue = default)
        {
            if (!bool.TryParse(str, out bool res))
            {
                return defaultValue;
            }

            return res;
        }
    }
}