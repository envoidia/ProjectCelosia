using System;
using System.Collections.Generic;
using System.Reflection;
using API.Modding;
using Jeffijoe.MessageFormat;

namespace API.Extensions;

public static class StringExtensions {
    private static readonly MessageFormatter Formatter = new();

    // todo these can share a name after SDK update probably
    extension(string str) {
        /// <summary>
        /// Gets a string from a lang key. Checks the specified mod's ResourceManager (if provided), then API.Lang.ResourceManager,
        /// then all mod ResourceManagers. Throws on invalid key
        /// </summary>
        public string GetLang(IGameMod? mod = null) {
            // Check provided mod
            string? lang = mod?.ResourceManager.GetString(str, Lang.Culture);
            if (lang is not null) return lang;

            // Check API
            lang = Lang.ResourceManager.GetString(str, Lang.Culture);
            if (lang is not null) return lang;

            // Check all mods
            foreach (IGameMod gameMod in ModLoader.LoadedMods) {
                lang = gameMod.ResourceManager.GetString(str, Lang.Culture);
                if (lang is not null) return lang;
            }

#if NATIVE_AOT
            throw new ArgumentException(string.Format(Lang.ErrKeyNotFoundAOT, str, mod?.GetName()));
#else
            throw new ArgumentException(string.Format(Lang.ErrKeyNotFound, str,
                Assembly.GetCallingAssembly().GetName().Name));
#endif
        }

        /// <summary>
        /// Gets a formatted string from a lang key. Checks the specified ResourceManager, then API.Lang.ResourceManager,
        /// then all mod ResourceManagers. Throws on invalid key or 0 args
        /// </summary>
        public string FormatLangRm(IGameMod? mod, params object?[] args) => args.Length == 0
            ? throw new ArgumentException("Must pass at least 1 arg")
            : string.Format(str.GetLang(mod), args);

        /// <summary>
        /// Gets a formatted string from a lang key from API.Lang.ResourceManager. Throws on invalid key or 0 args
        /// </summary>
        public string FormatLang(params object?[] args) => str.FormatLangRm(null, args);

        /// <summary>
        /// Gets an ICU MessageFormat-formatted string from a lang key. Checks the specified ResourceManager,
        /// then API.Lang.ResourceManager, then all mod ResourceManagers. Throws on invalid key or 0 args
        /// </summary>
        public string FormatIcuRm(IGameMod? mod, params object?[] args) {
            if (args.Length == 0) throw new ArgumentException("Must pass at least 1 arg");

            Dictionary<string, object?> dict = new(args.Length);

            for (uint i = 0; i < args.Length; i++) dict[i.ToString()] = args[i];

            return Formatter.FormatMessage(str.GetLang(mod), dict);
        }

        /// <summary>
        /// Gets an ICU MessageFormat-formatted string from a lang key from API.Lang.ResourceManager. Throws on invalid key or 0 args
        /// </summary>
        public string FormatIcu(params object?[] args) => str.FormatIcuRm(null, args);
    }
}