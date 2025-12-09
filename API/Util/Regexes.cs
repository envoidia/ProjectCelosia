using System.Text.RegularExpressions;

namespace API.Util;

public static partial class Regexes {
    /// <summary>
    /// Removes text formatting codes <c>/i[image]</c> and <c>/c[color]</c>, and undoubles slashes.
    /// For dumping text to the console in a readable format
    /// </summary>
    public static string RemoveFormattingCodes(string s) => _RemoveFormattingCodes().Replace(s, "");

    [GeneratedRegex(@"\/c\[.*?]| \/i\[.*?] |\/\/")]
    private static partial Regex _RemoveFormattingCodes();
}
