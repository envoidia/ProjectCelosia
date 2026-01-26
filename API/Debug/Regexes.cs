using System.Text.RegularExpressions;

namespace API.Debug;

public static partial class Regexes
{
    extension(string @this)
    {
        /// <summary>
        /// Removes text formatting codes <c>/i[image]</c>/<c>/c[color]</c> and undoubles slashes.
        /// For dumping text to the OS console in a readable format
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

    // Errors only show in the editor and do not prevent compilation

    [GeneratedRegex(@"\/c\[.*?]|\/i\[.*?]|\/\/")]
    private static partial Regex _RemoveFormattingCodes();

    [GeneratedRegex(@"\/i\[.*?]|\/\/")]
    private static partial Regex _RemoveImageCodes();
}
