using System.Text.RegularExpressions;

namespace API.Debug;

public static partial class FormattingRegexes
{
    extension(string @this)
    {
        /// <summary>
        /// Removes text formatting codes <c>/i[image]</c>/<c>/c[color]</c> and undoubles slashes.
        /// For dumping text to the stdout in a readable format
        /// </summary>
        public string RemoveFormattingCodes()
        {
            return _RemoveFormattingCodes().Replace(@this, "");
        }

        /// <summary>
        /// Removes text formatting codes <c>/i[image]</c> and undoubles slashes.
        /// For dumping text to the ingame console in a readable format
        /// </summary>
        public string RemoveImageCodes()
        {
            return _RemoveImageCodes().Replace(@this, "");
        }
    }

    [GeneratedRegex(@"\/[ci]\[.*?]|\/\/")]
    private static partial Regex _RemoveFormattingCodes();

    [GeneratedRegex(@"\/i\[.*?]|\/\/")]
    private static partial Regex _RemoveImageCodes();
}
