using System.Collections.Generic;
using System.IO;
using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Util;

namespace API.Save;

// todo serialize + use dict from props
public static class Settings
{
    public const string FilePath = "Settings.properties";

    /// <summary>
    /// Contains the .properties file data for the settings. Not used for live settings.
    /// To change a setting, change it in this field, then call <c>Write</c>, then <c>Reload</c>
    /// </summary>
    public static Dictionary<string, string> AllSettings = null!;

    #region Gameplay

    public static Lang.Language Language
    {
        get;
        set
        {
            field = value;
            Lang.Language._Change();
        }
    } = null!;

    // Speed of battle animations
    // Duration of in-battle pauses relative to 100% (1 = 100%, 0.1 = 10%)
    public static float BattleSpeed;

    public static bool ShowInvalidMoveWarning;

    #endregion

    #region Display

    /// <summary>
    /// Y resolution. -1 = auto
    /// </summary>
    public static int Resolution;

    public static bool Fullscreen;
    public static bool EnableVsync;

    /// <summary>
    /// Target FPS. Only used when <c>EnableVsync</c> is <c>false</c>. -1 = auto
    /// </summary>
    public static int TargetFps;

    public static Theme Theme
    {
        get;
        set
        {
            field = value;
            Theme._Change();
        }
    } = null!;

    #endregion

    #region Audio

    public static Progress MusicVolume;
    public static Progress SfxVolume;

    #endregion

    #region Controls

    // todo

    public static bool ShowInputGuide;
    public static bool DetectNintendoController;

    #endregion

    #region Debug

    public static bool EnableDebugFeatures;
    public static bool SelectOpponentMoves;

    #endregion

    internal static void _Init()
    {
        Reload();
    }

    /// <summary>
    /// Reloads all settings from the config file
    /// </summary>
    public static void Reload()
    {
        if (!File.Exists(FilePath))
        {
            Create();
        }

        AllSettings = Properties.Parse(FilePath);

        // Gameplay
        Language = Lang.Language.Langs.GetValueOrDefault(
            AllSettings.GetValueOrDefault("Language", Lang.Language.EnUS), Lang.Language.English);

        BattleSpeed = float.ParseOrDefault(AllSettings.GetValueOrDefault("BattleSpeed"), 1f);
        ShowInvalidMoveWarning = bool.ParseOrDefault(AllSettings.GetValueOrDefault("ShowInvalidMoveWarning"), true);

        // Display
        Resolution = int.ParseOrDefault(AllSettings.GetValueOrDefault("Resolution"), -1);
        Fullscreen = bool.ParseOrDefault(AllSettings.GetValueOrDefault("Fullscreen"), true);
        EnableVsync = bool.ParseOrDefault(AllSettings.GetValueOrDefault("EnableVsync"), true);
        TargetFps = int.ParseOrDefault(AllSettings.GetValueOrDefault("TargetFps"), -1);

        IRegistrable? r = Registry.Get(AllSettings.GetValueOrDefault("Theme", Theme.Apollo.GetId()));
        Theme = r is Theme t ? t : Theme.Apollo;

        // Audio
        MusicVolume = Progress.ParseOrDefault(AllSettings.GetValueOrDefault("MusicVolume"), new(0.75f));
        SfxVolume = Progress.ParseOrDefault(AllSettings.GetValueOrDefault("SfxVolume"), new(0.75f));

        // Controls
        ShowInputGuide = bool.ParseOrDefault(AllSettings.GetValueOrDefault("ShowInputGuide"), true);
        DetectNintendoController = bool.ParseOrDefault(AllSettings.GetValueOrDefault("DetectNintendoController"), true);

        // Debug
        EnableDebugFeatures = bool.ParseOrDefault(AllSettings.GetValueOrDefault("EnableDebugFeatures"), true);
        SelectOpponentMoves = bool.ParseOrDefault(AllSettings.GetValueOrDefault("SelectOpponentMoves"), false);
    }

    public static void Write()
    {
        Properties.Create(FilePath, AllSettings);
    }

    public static void Create()
    {
        // todo should this just set the keys instead of remaking the dict
        AllSettings = new Dictionary<string, string>
        {
            ["Language"] = Lang.Language.EnUS,
            ["BattleSpeed"] = "1",
            ["ShowInvalidMoveWarning"] = "true",
            ["Resolution"] = "-1",
            ["Fullscreen"] = "true",
            ["EnableVsync"] = "true",
            ["TargetFps"] = "-1",
            ["Theme"] = "__API:ThemeApollo",
            ["MusicVolume"] = "0.75",
            ["SfxVolume"] = "0.75",
            ["ShowInputGuide"] = "true",
            ["DetectNintendoController"] = "true",
            ["EnableDebugFeatures"] = "true",
            ["SelectOpponentMoves"] = "false",
        };
    }
}