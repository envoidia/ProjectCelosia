using System;
using System.Collections.Generic;
using System.Resources;
using Jeffijoe.MessageFormat;

namespace API.Extensions;

public static class StringExtensions {
    private static readonly MessageFormatter Formatter = new();

    extension(string str) {
        /// <summary>
        /// Gets a string from a lang key from the specified ResourceManager. Crashes on invalid key
        /// </summary>
        public string GetLangRm(ResourceManager rm) => rm.GetString(str, Lang.Culture)
                                                       ?? throw new ArgumentException(string.Format(Lang.KeyNotFound,
                                                           str, rm.BaseName));

        /// <summary>
        /// Gets a string from a lang key from API.Lang.ResourceManager. Crashes on invalid key
        /// </summary>
        public string GetLang() => str.GetLangRm(Lang.ResourceManager);

        /// <summary>
        /// Gets a formatted string from a lang key from the specified ResourceManager. Crashes on invalid key or 0 args
        /// </summary>
        public string FormatLangRm(ResourceManager rm, params object?[] args) => args.Length == 0
            ? throw new ArgumentException("Must pass at least 1 arg")
            : string.Format(str.GetLangRm(rm), args);

        /// <summary>
        /// ets a formatted string from a lang key from API.Lang.ResourceManager. Crashes on invalid key or 0 args
        /// </summary>
        public string FormatLang(params object?[] args) => str.FormatLangRm(Lang.ResourceManager, args);

        /// <summary>
        /// Gets an ICU MessageFormat-formatted string from a lang key from the specified ResourceManager. Crashes on invalid key or 0 args
        /// </summary>
        public string FormatIcuRm(ResourceManager rm, params object?[] args) {
            if (args.Length == 0) throw new ArgumentException("Must pass at least 1 arg");

            Dictionary<string, object?> dict = new(args.Length);

            for (uint i = 0; i < args.Length; i++) dict[i.ToString()] = args[i];

            return Formatter.FormatMessage(str.GetLangRm(rm), dict);
        }

        /// <summary>
        /// Gets an ICU MessageFormat-formatted string from a lang key from API.Lang.ResourceManager. Crashes on invalid key or 0 args
        /// </summary>
        public string FormatIcu(params object?[] args) => str.FormatIcuRm(Lang.ResourceManager, args);
    }
}