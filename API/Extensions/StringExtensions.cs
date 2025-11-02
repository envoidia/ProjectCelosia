using System;
using System.Collections.Generic;
using Jeffijoe.MessageFormat;

namespace API.Extensions;

public static class StringExtensions {
    private static readonly MessageFormatter Formatter = new(true, "en");

    extension(string str) {
        /// <summary>
        /// Gets a string from a lang key
        /// Crashes on invalid key
        /// </summary>
        public string GetLang() =>
            Lang.ResourceManager.GetString(str, Lang.Culture) ?? throw new ArgumentException("Invalid key");

        /// <summary>
        /// Gets a formatted string from a lang key
        /// Crashes on invalid key or 0 args
        /// </summary>
        public string FormatLang(params object?[] args) => args.Length == 0
            ? throw new ArgumentException("Must pass at least 1 arg")
            : string.Format(Lang.ResourceManager.GetString(str, Lang.Culture)!, args);

        /// <summary>
        /// Gets an ICU MessageFormat-formatted string from a lang key
        /// Crashes on invalid key or 0 args
        /// </summary>
        public string FormatIcu(params object?[] args) {
            if (args.Length == 0) throw new ArgumentException("Must pass at least 1 arg");
            Dictionary<string, object?> dict = new(args.Length);
            for (int i = 0; i < args.Length; i++) dict[i.ToString()] = args[i];
            return Formatter.FormatMessage(Lang.ResourceManager.GetString(str, Lang.Culture)!, dict);
        }
    }
}