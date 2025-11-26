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
        /// <summary>
        /// Gets a string from a lang key. Checks the specified mod's <c>ResourceManager</c> (if provided),
        /// then <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s. Throws on invalid key.
        /// <para>
        /// Prefer calling <c>string.Format()</c> on the properties of <c>Lang.Designer</c> when possible,
        /// to avoid writing strings in code
        /// </para>  
        /// <para>
        /// Throws <c>ArgumentException</c> if key is <c>null</c>
        /// </para> 
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
        /// Gets a formatted string from a lang key. Checks the specified <c>ResourceManager</c> (if provided), 
        /// then <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s
        /// <para>
        /// Prefer calling <c>string.Format()</c> on the properties of <c>Lang.Designer</c> when possible,
        /// to avoid writing strings in code
        /// </para>   
        /// <para>
        /// Throws <c>ArgumentException</c> if key is <c>null</c>.
        /// In debug, also throws <c>ArgumentException</c> if 0 args are passed
        /// </para>    
        /// </summary>
        public string FormatLang(IGameMod? mod, params object?[] args) {
#if DEBUG
            if (args.Length == 0) throw new ArgumentException(Lang.Err0Args);
#endif

            return string.Format(@this.GetLang(mod), args);
        }

        /// <summary>
        /// Gets a formatted string from a lang key. Checks <c>API.Lang.ResourceManager</c>,
        /// then all mod <c>ResourceManager</c>s
        /// <para>
        /// Prefer calling <c>string.Format()</c> on the properties of <c>Lang.Designer</c> when possible,
        /// to avoid writing strings in code
        /// </para>
        /// <para>
        /// Throws <c>ArgumentException</c> if key is <c>null</c>.
        /// In debug, also throws <c>ArgumentException</c> if 0 args are passed
        /// </para> 
        /// </summary>
        public string FormatLang(params object?[] args) => @this.FormatLang(null, args);

        /// <summary>
        /// Gets an ICU MessageFormat-formatted string from a lang key. Checks the specified <c>ResourceManager</c>,
        /// then <c>API.Lang.ResourceManager</c>, then all mod <c>ResourceManager</c>s
        /// <para>
        /// Prefer calling <c>FormatIcu()</c> on the properties of <c>Lang.Designer</c> when possible,
        /// to avoid writing strings in code
        /// </para>
        /// <para>
        /// Throws <c>ArgumentException</c> if key is <c>null</c>.
        /// In debug, also throws <c>ArgumentException</c> if 0 args are passed
        /// </para> 
        /// </summary>
        public string FormatIcuLang(IGameMod? mod, params object?[] args) {
#if DEBUG
            if (args.Length == 0) throw new ArgumentException(Lang.Err0Args);
#endif

            Dictionary<string, object?> dict = new(args.Length);

            for (int i = 0; i < args.Length; i++) dict[i.ToString()] = args[i];

            return Formatter.FormatMessage(@this.GetLang(mod), dict);
        }

        /// <summary>
        /// Gets an ICU MessageFormat-formatted string from a lang key. Checks <c>API.Lang.ResourceManager</c>,
        /// then all mod <c>ResourceManager</c>s
        /// <para>
        /// Prefer calling <c>FormatIcu()</c> on the properties of <c>Lang.Designer</c> when possible,
        /// to avoid writing strings in code
        /// </para>
        /// <para>
        /// Throws <c>ArgumentException</c> if key is <c>null</c>.
        /// In debug, also throws <c>ArgumentException</c> if 0 args are passed
        /// </para> 
        /// </summary>
        public string FormatIcuLang(params object?[] args) => @this.FormatIcuLang(null, args);

        /// <summary>
        /// Formats the provided <c>string</c> with ICU MessageFormat
        /// <para>
        /// Throws <c>ArgumentException</c> if key is <c>null</c>.
        /// In debug, also throws <c>ArgumentException</c> if 0 args are passed
        /// </para> 
        /// </summary>
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