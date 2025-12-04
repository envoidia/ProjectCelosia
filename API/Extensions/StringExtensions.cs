using System;
using System.Collections.Generic;
using System.Reflection;
using API.Modding;
using Jeffijoe.MessageFormat;

namespace API.Extensions;

// todo fix warning CS8620: Argument of type 'string' cannot be used for parameter 'args' of type 'object?[]' in 'string extension(string).FormatIcu(params object?[] args)' due to differences in the nullability of reference types
public static class StringExtensions {
    private static readonly MessageFormatter Formatter = new();

    extension(string @this) {
        /// <returns>
        /// A string from a lang key. Checks the specified mod's <c>ResourceManager</c> (if provided),
        /// then <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s. Throws on invalid key
        /// </returns>
        /// <para>Prefer using the properties of <c>Lang</c> when possible, to avoid writing strings in code</para>  
        /// <para>Throws <c>ArgumentException</c> if key is invalid</para>
        /// <param name="mod">The mod to check first</param>
        /// <exception cref="ArgumentException">If key is invalid</exception>
        public string GetLang(GameMod? mod = null) {
            // Check provided mod
            string? lang = mod?.ResourceManager.GetString(@this, Lang.Culture);
            if (lang is not null) return lang;

            // Check API
            lang = Lang.ResourceManager.GetString(@this, Lang.Culture);
            if (lang is not null) return lang;

            // Check all mods
            foreach (GameMod gameMod in ModLoader.LoadedMods) {
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

        /// <returns>
        /// A formatted string from a lang key. Checks the specified mod's <c>ResourceManager</c> (if provided),
        /// then <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s
        /// </returns>
        /// <para>Prefer calling <c>string.Format()</c> on the properties of <c>Lang</c> when possible, to avoid writing strings in code</para>  
        /// <para>Throws <c>ArgumentException</c> if key is invalid or in debug and 0 args are passed</para>
        /// <param name="mod">The mod to check first</param>
        /// <param name="args">The formatting arguments to apply</param>
        /// <exception cref="ArgumentException">If key is invalid or in debug and 0 args are passed</exception>
        public string FormatLang(GameMod? mod = null, params object?[] args) {
#if DEBUG
            if (args.Length == 0) throw new ArgumentException(Lang.Err0Args);
#endif

            return string.Format(@this.GetLang(mod), args);
        }

        /// <returns>
        /// A formatted string from a lang key. Checks <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s. Throws on invalid key
        /// </returns>
        /// <para>Prefer calling <c>string.Format()</c> on the properties of <c>Lang</c> when possible, to avoid writing strings in code</para>  
        /// <para>Throws <c>ArgumentException</c> if key is invalid or in debug and 0 args are passed</para>
        /// <exception cref="ArgumentException">If key is invalid or in debug and 0 args are passed</exception>
        /// <param name="args">The formatting arguments to apply</param>
        public string FormatLang(params object?[] args) => @this.FormatLang(null, args);

        /// <returns>
        /// An ICU MessageFormat-formatted string from a lang key. Checks <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s. Throws on invalid key
        /// </returns>
        /// <para>Prefer calling <c>FormatIcu()</c> on the properties of <c>Lang</c> when possible, to avoid writing strings in code</para>  
        /// <para>Throws <c>ArgumentException</c> if key is invalid or in debug and 0 args are passed</para>
        /// <exception cref="ArgumentException">If key is invalid or in debug and 0 args are passed</exception>
        /// <param name="mod">The <c>IGameMod</c> to check first</param>
        /// <param name="args">The formatting arguments to apply</param>
        public string FormatIcuLang(GameMod? mod, params object?[] args) {
#if DEBUG
            if (args.Length == 0) throw new ArgumentException(Lang.Err0Args);
#endif

            Dictionary<string, object?> dict = new(args.Length);

            for (int i = 0; i < args.Length; i++) dict[i.ToString()] = args[i];

            return Formatter.FormatMessage(@this.GetLang(mod), dict);
        }

        /// <returns>
        /// An ICU MessageFormat-formatted string from a lang key. Checks <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s. Throws on invalid key
        /// </returns>
        /// <para>Prefer calling <c>FormatIcu()</c> on the properties of <c>Lang</c> when possible, to avoid writing strings in code</para>  
        /// <para>Throws <c>ArgumentException</c> if key is invalid or in debug and 0 args are passed</para>
        /// <exception cref="ArgumentException">If key is invalid or in debug and 0 args are passed</exception>
        /// <param name="args">The formatting arguments to apply</param>
        public string FormatIcuLang(params object?[] args) => @this.FormatIcuLang(null, args);

        /// <returns>
        /// The provided <c>string</c> formatted with ICU MessageFormat
        /// </returns>
        /// <para>Throws <c>ArgumentException</c> if key is invalid or in debug and 0 args are passed</para>
        /// <exception cref="ArgumentException">If in debug and 0 args are passed</exception>
        /// <param name="args">The formatting arguments to apply</param>
        public string FormatIcu(params object?[] args) {
#if DEBUG
            if (args.Length == 0) throw new ArgumentException(Lang.Err0Args);
#endif

            Dictionary<string, object?> dict = new(args.Length);

            for (int i = 0; i < args.Length; i++) dict[i.ToString()] = args[i];

            return Formatter.FormatMessage(@this, dict);
        }
    }
}