using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using API.Save;
using API.Util;

namespace API.Lang;

/// <summary>
/// A language that the game can be set to
/// </summary>
/// <param name="Name">The display name of the language</param>
/// <param name="LocaleCode">The language's locale code, <c>lang-REGION</c>, eg <c>en-US</c></param>
/// <param name="UseHarfBuzz">(nyi) Whether to use HarfBuzz text shaping. Required for certain languages, like Hindi and Arabic</param>
public record Language(string Name, string LocaleCode, bool UseHarfBuzz = false) {
    /// <summary>
    /// Lang entries for this
    /// </summary>
    public Dictionary<string, string> Entries { get; } = [];

    /// <summary>
    /// English. Default
    /// </summary>
    public const string EnUS = "en-US";

    /// <inheritdoc cref="EnUS" />
    public static readonly Language English = new("English", EnUS);

    /// <summary>
    /// Meta-dictionary to map locale codes to languages
    /// </summary>
    public static readonly Dictionary<string, Language> Langs = new() {
        [EnUS] = English
    };

    /// <summary>
    /// Notified when the current <c>Language</c> changes
    /// </summary>
    public static event Action? OnChange;

    static Language() => AddLangFile(EnUS, Core.Id, "Lang/Lang.en-US.properties");

    /// <summary>
    /// Parses a .properties file and adds its entries to the lang dictionary for the given locale code under the given mod ID
    /// </summary>
    public static void AddLangFile(string localeCode, string modId, string file) {
        Dictionary<string, string> entries = Langs[localeCode].Entries;

        foreach (KeyValuePair<string, string> kvp in Properties.Parse(file)) {
            if (entries.GetValueOrDefault(kvp.Key) is not null) {
                DebugConsole.Log(
                    $"Language {localeCode} already has a value at {modId}:{kvp.Key} ({entries[kvp.Key]}), overwriting with {kvp.Value}",
                    nameof(Language), DebugConsole.LogLevel.Warning);
            }
            entries[$"{modId}:{kvp.Key}"] = kvp.Value;
        }
    }

    internal static void _Change() {
        CultureInfo culture = new(Settings.Language.LocaleCode);

        // todo do i need both of these? whats the difference?
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        // todo harfbuzz logic https://github.com/FontStashSharp/FontStashSharp/wiki/HarfBuzz-Text-Shaping

        OnChange?.Invoke();
    }

    public override string ToString() {
        return $"Language {this.Name}, {this.LocaleCode}:\n{string.Join('\n', this.Entries.OrderBy(kvp => kvp.Key)
            .Select(kvp => $"{kvp.Key} = {kvp.Value}"))}";
    }
}
