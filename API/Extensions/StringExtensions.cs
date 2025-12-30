using System.Collections.Generic;
using API.Graphics;
using API.Modding;
using API.Save;
using API.Util;
using Jeffijoe.MessageFormat;

namespace API.Extensions;

// todo the warning is a compiler bug. can only wait for a fix
// https://github.com/dotnet/roslyn/issues/80024
public static class StringExtensions {
    private static readonly MessageFormatter _Formatter = new();

    extension(string @this) {
        /// <returns>
        /// Searches the current lang for key <c>modId:this</c>.
        /// If provided <c>this</c> has a mod ID portion (<c>:</c>), uses that instead of <c>modId</c> param.
        /// If the key can't be found, defaults to displaying the given key
        /// </returns>
        public string GetLang(string modId = Core.Id) {
            string key = @this.Split(':').Length == 1 ? $"{modId}:{@this}" : @this;
            string? lang;
            if ((lang = Settings.Language.Entries.GetValueOrDefault(key)) is not null) return lang;

            // Fall back to en-US
            DebugUtil.Log(
                $"Lang entry {key} not found in language {Settings.Language.Name}: {Settings.Language.LocaleCode}, falling back to en-US",
                nameof(StringExtensions), DebugUtil.LogLevel.Warning);

            if ((lang = Lang.Language.English.Entries.GetValueOrDefault(key)) is not null) return lang;

            // Default to ID
            DebugUtil.Log($"Lang entry {key} not found", nameof(StringExtensions),
                DebugUtil.LogLevel.Warning);
            return $"{ThemeColor.Neg.Str()}MISSING LANG ENTRY: {key}";
        }

        /// <returns>
        /// Searches the current lang for key <c>modId:this</c> and formats it.
        /// If provided <c>this</c> has a mod ID portion (<c>:</c>), uses that instead of <c>modId</c> param.
        /// If the key can't be found, defaults to displaying the given key
        /// </returns>
        /// <para>Asserts > 0 args passed</para>
        /// <param name="args">The formatting arguments to apply</param>
        public string FormatLang(string modId, object[] args) {
            // todo Assert.LenNotZero(args);
            return string.Format(@this.GetLang(modId), args);
        }

        /// <inheritdoc cref="FormatLang(string, string, object[])" />
        public string FormatLang(object[] args) => @this.FormatLang(Core.Id, args);

        /// <inheritdoc cref="FormatLang(string, string, object[])" />
        public string FormatLang(object args) => @this.FormatLang(Core.Id, [args]);

        /// <returns>
        /// Searches the current lang for key <c>modId:this</c> and ICU MessageFormats it.
        /// If provided <c>this</c> has a mod ID portion (<c>:</c>), uses that instead of <c>modId</c> param.
        /// If the key can't be found, defaults to displaying the given key
        /// </returns>
        /// <summary>
        /// <para>Asserts > 0 args passed</para>
        /// </summary>
        /// <param name="args">The formatting arguments to apply</param>
        public string IcuFormatLang(string modId, object[] args) {
            // todo Assert.LenNotZero(args);

            Dictionary<string, object?> dict = new(args.Length);

            for (int i = 0; i < args.Length; i++) dict[i.ToString()] = args[i];

            return _Formatter.FormatMessage(@this.GetLang(modId), dict);
        }

        /// <inheritdoc cref="IcuFormatLang(string, string, object[])" />
        public string IcuFormatLang(object[] args) => @this.IcuFormatLang(Core.Id, args);

        /// <inheritdoc cref="IcuFormatLang(string, string, object[])" />
        public string IcuFormatLang(object args) => @this.IcuFormatLang(Core.Id, [args]);

        /// <returns>
        /// The provided <c>string</c> with the first character lowercased
        /// </returns>
        public string FirstToLower() => string.IsNullOrEmpty(@this) ? @this : char.ToLower(@this[0]) + @this[1..];
    }
}