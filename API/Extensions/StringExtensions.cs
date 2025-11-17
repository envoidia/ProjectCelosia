using System;
using System.Collections.Generic;
using System.Reflection;
using API.Modding;
using Jeffijoe.MessageFormat;

namespace API.Extensions;

public static class StringExtensions {
    private static readonly MessageFormatter Formatter = new();
    
    extension(string @this) {
        /// <summary>
        /// Gets a string from a lang key. Checks the specified mod's <c>ResourceManager</c> (if provided), then <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s. Throws on invalid key
        /// </summary>
        public string GetLang(IGameMod? mod = null) {
            // Check provided mod
            string? lang = mod?.ResourceManager.GetString(@this, Lang.Culture);
            if (lang is not null) return lang;

            // Check API
            lang = Lang.ResourceManager.GetString(@this, Lang.Culture);
            if (lang is not null) return lang;

            // Check all mods
            foreach (IGameMod gameMod in ModLoader.LoadedMods) {
                lang = gameMod.ResourceManager.GetString(@this, Lang.Culture);
                if (lang is not null) return lang;
            }

#if NATIVE_AOT
            throw new ArgumentException(string.Format(Lang.ErrKeyNotFoundAOT, str, mod?.GetName()));
#else
            throw new ArgumentException(string.Format(Lang.ErrKeyNotFound, @this,
                Assembly.GetCallingAssembly().GetName().Name));
#endif
        }

        /// <summary>
        /// Gets a formatted string from a lang key. Checks the specified <c>ResourceManager</c>, then <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s. Throws on invalid key or 0 args
        /// </summary>
        public string FormatLang(IGameMod? mod, params object?[] args) => args.Length == 0
            ? throw new ArgumentException("Must pass at least 1 arg")
            : string.Format(@this.GetLang(mod), args);

        /// <summary>
        /// Gets a formatted string from a lang key from <c>API.Lang.ResourceManager</c>. Throws on invalid key or 0 args
        /// </summary>
        public string FormatLang(params object?[] args) => @this.FormatLang(null, args);

        /// <summary>
        /// Gets an ICU MessageFormat-formatted string from a lang key. Checks the specified <c>ResourceManager</c>, then <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s. Throws on invalid key or 0 args
        /// </summary>
        public string FormatIcu(IGameMod? mod, params object?[] args) {
            if (args.Length == 0) throw new ArgumentException("Must pass at least 1 arg");

            Dictionary<string, object?> dict = new(args.Length);

            for (int i = 0; i < args.Length; i++) dict[i.ToString()] = args[i];

            return Formatter.FormatMessage(@this.GetLang(mod), dict);
        }

        /// <summary>
        /// Gets an ICU MessageFormat-formatted string from a lang key from <c>API.Lang.ResourceManager</c>. Throws on invalid key or 0 args
        /// </summary>
        public string FormatIcu(params object?[] args) => @this.FormatIcu(null, args);
    }
}