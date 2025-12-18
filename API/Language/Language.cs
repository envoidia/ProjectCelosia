using System;
using System.Globalization;
using System.Threading;
using API.Save;

namespace API.Language;

/// <summary>
/// A language that the game can be set to
/// </summary>
/// <param name="Name">The display name of the language</param>
/// <param name="LocaleCode">The language's locale code, <c>lang-REGION</c>, eg <c>en-US</c></param>
/// <param name="UseHarfBuzz">(nyi) Whether to use HarfBuzz text shaping. Required for certain languages, like Hindi and Arabic</param>
public record Language(string Name, string LocaleCode, bool UseHarfBuzz = false) {
    /// <summary>
    /// English. Default
    /// </summary>
    // todo this might need a blank localeCode?
    public static readonly Language English = new("English", "en-US");

    /// <summary>
    /// Notified when the current <c>Language</c> changes
    /// </summary>
    public static event Action? OnChange;

    internal static void _Change() {
        CultureInfo culture = new(Settings.Language.LocaleCode);

        // todo do i need both of these? whats the difference?
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        // todo harfbuzz logic https://github.com/FontStashSharp/FontStashSharp/wiki/HarfBuzz-Text-Shaping

        OnChange?.Invoke();
    }
}
