using System;
using System.Collections.Generic;
using System.Reflection;
using API.Modding;
using API.Util;
using Jeffijoe.MessageFormat;

namespace API.Extensions;

// the warning is a compiler bug. can only wait for a fix
public static class StringExtensions {
    private static readonly MessageFormatter _Formatter = new();

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
            foreach (GameMod gameMod in ModLoader._LoadedMods) {
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
        /// <para>Throws <c>ArgumentException</c> if key is invalid. Asserts > 0 args passed</para>
        /// <param name="mod">The mod to check first</param>
        /// <param name="args">The formatting arguments to apply</param>
        /// <exception cref="ArgumentException">If key is invalid</exception>
        public string FormatLang(GameMod? mod = null, params object[] args) {
            Assert.SizeNotZero(args);
            return string.Format(@this.GetLang(mod), args);
        }

        /// <returns>
        /// A formatted string from a lang key. Checks <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s
        /// </returns>
        /// <summary>
        /// <para>Prefer calling <c>string.Format()</c> on the properties of <c>Lang</c> when possible, to avoid writing strings in code</para>  
        /// <para>Throws <c>ArgumentException</c> if key is invalid. Asserts > 0 args passed</para>
        /// </summary>
        /// <exception cref="ArgumentException">If key is invalid. Asserts > 0 args passed</exception>
        /// <param name="args">The formatting arguments to apply</param>
        public string FormatLang(params object[] args) => @this.FormatLang(null, args);

        /// <returns>
        /// An ICU MessageFormat-formatted string from a lang key. Checks <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s
        /// </returns>
        /// <summary>
        /// <para>Prefer calling <c>FormatIcu()</c> on the properties of <c>Lang</c> when possible, to avoid writing strings in code</para>  
        /// <para>Throws <c>ArgumentException</c> if key is invalid. Asserts > 0 args passed</para>
        /// </summary>
        /// <exception cref="ArgumentException">If key is invalid</exception>
        /// <param name="mod">The <c>IGameMod</c> to check first</param>
        /// <param name="args">The formatting arguments to apply</param>
        public string FormatIcuLang(GameMod? mod, params object[] args) {
            Assert.SizeNotZero(args);

            Dictionary<string, object?> dict = new(args.Length);

            for (int i = 0; i < args.Length; i++) dict[i.ToString()] = args[i];

            return _Formatter.FormatMessage(@this.GetLang(mod), dict);
        }

        /// <returns>
        /// An ICU MessageFormat-formatted string from a lang key. Checks <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s
        /// </returns>
        /// <summary>
        /// <para>Prefer calling <c>FormatIcu()</c> on the properties of <c>Lang</c> when possible, to avoid writing strings in code</para>  
        /// <para>Throws <c>ArgumentException</c> if key is invalid. Asserts > 0 args passed</para>
        /// </summary>
        /// <exception cref="ArgumentException">If key is invalid</exception>
        /// <param name="args">The formatting arguments to apply</param>
        public string FormatIcuLang(params object[] args) => @this.FormatIcuLang(null, args);

        /// <returns>
        /// The provided <c>string</c> formatted with ICU MessageFormat
        /// </returns>
        /// <summary>
        /// Throws <c>ArgumentException</c> if key is invalid. Asserts > 0 args passed
        /// </summary>
        /// <exception cref="ArgumentException">If key is invalid</exception>
        /// <param name="args">The formatting arguments to apply</param>
        public string FormatIcu(params object[] args) {
            Assert.SizeNotZero(args);

            Dictionary<string, object?> dict = new(args.Length);

            for (int i = 0; i < args.Length; i++) dict[i.ToString()] = args[i];

            return _Formatter.FormatMessage(@this, dict);
        }
    }
}