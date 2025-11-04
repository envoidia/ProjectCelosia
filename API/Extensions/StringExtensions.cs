using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Jeffijoe.MessageFormat;

namespace API.Extensions;

public static class StringExtensions {
    private static readonly MessageFormatter Formatter = new();

#if !NATIVE_AOT
    // Cache mod langs for performance (todo test difference)
    private static readonly Dictionary<Assembly, ResourceManager> LangCache = new();
#endif

    extension(string str) {
        /// <summary>
        /// Gets a string from a lang key.
        /// Crashes on invalid key
        /// </summary>
        public string GetLang() {
#if NATIVE_AOT // Call API.Lang
                return Lang.ResourceManager.GetString(str, Lang.Culture) ?? throw new ArgumentException("Invalid key");
#else // Call the Lang class of whatever assembly called this
            Assembly callingAsm = Assembly.GetCallingAssembly();

            ResourceManager? rm;

            // Check cache
            if (LangCache.TryGetValue(callingAsm, out ResourceManager? resourceManager)) {
                rm = resourceManager;
            } else {
                string langTypeName = $"{callingAsm.GetName().Name}.Lang";

                // Find the calling assembly's Lang class
                Type? langType = callingAsm.GetType(langTypeName);

                if (langType is not null) {
                    rm = langType
                        .GetProperty("ResourceManager", BindingFlags.Public | BindingFlags.Static)?
                        .GetValue(null) as ResourceManager;
                } else {
                    // Fallback to API.Lang
                    rm = Lang.ResourceManager;
#if DEBUG
                    Console.WriteLine(Lang.FallingBackToAPILang, callingAsm.GetName().Name);
#endif
                }

                // Cache the value
                LangCache[callingAsm] = rm!;
            }

            return rm?.GetString(str, Lang.Culture)
                   ?? throw new ArgumentException(string.Format(Lang.LangError, str, callingAsm.GetName().Name));
#endif
        }

        /// <summary>
        /// Gets a formatted string from a lang key.
        /// Crashes on invalid key or 0 args
        /// </summary>
        public string FormatLang(params object?[] args) => args.Length == 0
            ? throw new ArgumentException("Must pass at least 1 arg")
            : string.Format(str.GetLang(), args);

        /// <summary>
        /// Gets an ICU MessageFormat-formatted string from a lang key.
        /// Crashes on invalid key or 0 args
        /// </summary>
        public string FormatIcu(params object?[] args) {
            if (args.Length == 0) throw new ArgumentException("Must pass at least 1 arg");

            Dictionary<string, object?> dict = new(args.Length);

            for (int i = 0; i < args.Length; i++) dict[i.ToString()] = args[i];

            return Formatter.FormatMessage(str.GetLang(), dict);
        }
    }
}