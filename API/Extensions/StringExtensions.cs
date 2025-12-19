using System.Collections.Generic;
using API.Graphics;
using API.Modding;
using API.Util;
using Jeffijoe.MessageFormat;

namespace API.Extensions;

// the warning is a compiler bug. can only wait for a fix
// https://github.com/dotnet/roslyn/issues/80024
public static class StringExtensions {
    private static readonly MessageFormatter _Formatter = new();

    extension(string @this) {
        /// <returns>
        /// A string from a lang key. Checks the specified mod's <c>ResourceManager</c> (if provided),
        /// then <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s.
        /// If the key can't be found, defaults to displaying the given modId and key
        /// </returns>
        /// <para>Prefer using the properties of <c>Lang</c> when possible, to avoid writing strings in code</para>  
        /// <param name="modId">The ID of the mod to check first</param>
        public string GetLang(string modId = Core.Id) {
            string? lang = null;

            // Check provided mod
            if (modId != Core.Id) {
                lang = ModLoader.GetFromId(modId)?.ResourceManager.GetString(@this, Lang.Culture);
                if (lang is not null) return lang;
            }

            // Check API
            lang = Lang.ResourceManager.GetString(@this, Lang.Culture);
            if (lang is not null) return lang;

            // Check all mods
            foreach (GameMod gameMod in ModLoader._LoadedMods) {
                lang = gameMod.ResourceManager.GetString(@this, Lang.Culture);
                if (lang is not null) return lang;
            }

            // Default to ID
            return $"{ThemeColor.Neg.Str()}{modId}:{@this}";
        }

        /// <returns>
        /// A formatted string from a lang key. Checks the specified mod's <c>ResourceManager</c> (if provided),
        /// then <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s.
        /// If the key can't be found, defaults to displaying the given modId and key
        /// </returns>
        /// <para>Prefer calling <c>string.Format()</c> on the properties of <c>Lang</c> when possible, to avoid writing strings in code</para>  
        /// <para>Asserts > 0 args passed</para>
        /// <param name="modId">The ID of the mod to check first</param>
        /// <param name="args">The formatting arguments to apply</param>
        public string FormatLang(string modId = Core.Id, params object[] args) {
            Assert.LenNotZero(args);
            return string.Format(@this.GetLang(modId), args);
        }

        /// <returns>
        /// A formatted string from a lang key. Checks <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s.
        /// If the key can't be found, defaults to displaying the given modId and key
        /// </returns>
        /// <summary>
        /// <para>Prefer calling <c>string.Format()</c> on the properties of <c>Lang</c> when possible, to avoid writing strings in code</para>  
        /// <para>Asserts > 0 args passed</para>
        /// </summary>
        /// <param name="args">The formatting arguments to apply</param>
        // todo dont specify core id?
        public string FormatLang(params object[] args) => @this.FormatLang(Core.Id, args);

        /// <returns>
        /// An ICU MessageFormat-formatted string from a lang key.
        /// Checks the specified mod's <c>ResourceManager</c> (if provided),
        /// then <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s.
        /// If the key can't be found, defaults to displaying the given modId and key
        /// </returns>
        /// <summary>
        /// <para>Prefer calling <c>FormatIcu()</c> on the properties of <c>Lang</c> when possible,
        /// to avoid writing strings in code</para>  
        /// <para>Asserts > 0 args passed</para>
        /// </summary>
        /// <param name="modId">The ID of the mod to check first</param>
        /// <param name="args">The formatting arguments to apply</param>
        public string FormatIcuLang(string modId = Core.Id, params object[] args) {
            Assert.LenNotZero(args);

            Dictionary<string, object?> dict = new(args.Length);

            for (int i = 0; i < args.Length; i++) dict[i.ToString()] = args[i];

            return _Formatter.FormatMessage(@this.GetLang(modId), dict);
        }

        /// <returns>
        /// An ICU MessageFormat-formatted string from a lang key. Checks <c>API.Lang.ResourceManager</c>,
        /// then all mod <c>ResourceManager</c>s.
        /// If the key can't be found, defaults to displaying the given modId and key
        /// </returns>
        /// <summary>
        /// <para>Prefer calling <c>FormatIcu()</c> on the properties of <c>Lang</c> when possible, to avoid writing strings in code</para>  
        /// <para>Asserts > 0 args passed</para>
        /// </summary>
        /// <param name="args">The formatting arguments to apply</param>
        public string FormatIcuLang(params object[] args) => @this.FormatIcuLang(Core.Id, args);

        /// <returns>
        /// The provided <c>string</c> formatted with ICU MessageFormat
        /// </returns>
        /// <summary>
        /// Asserts > 0 args passed
        /// </summary>
        /// <param name="args">The formatting arguments to apply</param>
        public string FormatIcu(params object[] args) {
            Assert.LenNotZero(args);

            Dictionary<string, object?> dict = new(args.Length);

            for (int i = 0; i < args.Length; i++) dict[i.ToString()] = args[i];

            return _Formatter.FormatMessage(@this, dict);
        }

        /// <returns>
        /// The provided <c>string</c> with the first character lowercased
        /// </returns>
        public string FirstToLower() => string.IsNullOrEmpty(@this) ? @this : char.ToLower(@this[0]) + @this[1..];
    }
}