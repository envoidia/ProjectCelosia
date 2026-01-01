using System.Text.RegularExpressions;

namespace API.Util;

public static partial class Regexes
{
    extension(string @this)
    {
        /// <summary>
        /// Removes text formatting codes <c>/i[image]</c> and <c>/c[color]</c>, and undoubles slashes.
        /// For dumping text to the console in a readable format
        /// </summary>
        public string RemoveFormattingCodes()
        {
            return _RemoveFormattingCodes().Replace(@this, "");
        }
    }

    [GeneratedRegex(@"\/c\[.*?]| \/i\[.*?] |\/\/")]
    private static partial Regex _RemoveFormattingCodes();
}
