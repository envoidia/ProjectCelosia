using System;
using System.Text.RegularExpressions;

namespace API.Util;

public static partial class Regexes {
    [GeneratedRegex(@"\/(i|c)\[.*?]")]
    public static partial Regex FormattingCodeRemover();
}
