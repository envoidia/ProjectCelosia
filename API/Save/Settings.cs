using System.Collections.Generic;
using System.IO;
using API.Debug;
using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Util;

namespace API.Save;

// todo serialize + use dict from props
public static class Settings
{
    public const string FilePath = "Settings.properties";

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

        Dictionary<string, string> settings = Properties.Parse(FilePath);

        // Gameplay
        Language = Lang.Language.Langs.GetValueOrDefault(
            settings.GetValueOrDefault("Language", Lang.Language.EnUS), Lang.Language.English);

        BattleSpeed = float.ParseOrDefault(settings.GetValueOrDefault("BattleSpeed"), 1f);
        ShowInvalidMoveWarning = bool.ParseOrDefault(settings.GetValueOrDefault("ShowInvalidMoveWarning"), true);

        // Display
        Resolution = int.ParseOrDefault(settings.GetValueOrDefault("Resolution"), -1);
        Fullscreen = bool.ParseOrDefault(settings.GetValueOrDefault("Fullscreen"), true);
        EnableVsync = bool.ParseOrDefault(settings.GetValueOrDefault("EnableVsync"), true);
        TargetFps = int.ParseOrDefault(settings.GetValueOrDefault("TargetFps"), -1);

        IRegistrable? r = Registry.Get(settings.GetValueOrDefault("Theme", Theme.Apollo.GetId()));
        Theme = r is Theme t ? t : Theme.Apollo;

        // Audio
        MusicVolume = Progress.ParseOrDefault(settings.GetValueOrDefault("MusicVolume"), new(0.75f));
        SfxVolume = Progress.ParseOrDefault(settings.GetValueOrDefault("SfxVolume"), new(0.75f));

        // Controls
        ShowInputGuide = bool.ParseOrDefault(settings.GetValueOrDefault("ShowInputGuide"), true);
        DetectNintendoController = bool.ParseOrDefault(settings.GetValueOrDefault("DetectNintendoController"), true);

        // Debug
        EnableDebugFeatures = bool.ParseOrDefault(settings.GetValueOrDefault("EnableDebugFeatures"), true);
        SelectOpponentMoves = bool.ParseOrDefault(settings.GetValueOrDefault("SelectOpponentMoves"), false);
    }

    public static void Create(
        string language = Lang.Language.EnUS,
        float battleSpeed = 1f,
        bool showInvalidMoveWarning = true,
        int resolution = -1,
        bool fullscreen = true,
        bool enableVsync = true,
        int targetFps = -1,
        string theme = "__API:ThemeApollo",
        float musicVolume = 0.75f,
        float sfxVolume = 0.75f,
        bool showInputGuide = true,
        bool detectNintendoController = true,
        bool enableDebugFeatures = true,
        bool selectOpponentMoves = false)
    {
        File.WriteAllText(FilePath, $"""
        ### Settings
        # If this file is deleted, it will be regenerated with default settings
        # Invalid values will default

        ### Gameplay

        # Default: en-US
        Language={language}

        # Default: 1
        BattleSpeed={battleSpeed}

        # Default: True
        ShowInvalidMoveWarning={showInvalidMoveWarning}

        ### Display

        # Default: -1 (auto)
        Resolution={resolution}

        # Default: True
        Fullscreen={fullscreen}

        # Default: True
        EnableVsync={enableVsync}

        # Default: -1 (auto)
        TargetFps={targetFps}

        # Default: __API:ThemeApollo
        Theme={theme}

        ### Audio

        # 0-1
        # Default: 0.75
        MusicVolume={musicVolume}

        # Default: 0.75
        SfxVolume={sfxVolume}

        ### Controls

        # todo

        # Default: True
        ShowInputGuide={showInputGuide}

        # Default: True
        DetectNintendoController={detectNintendoController}

        ### Debug

        # Default: True
        EnableDebugFeatures={enableDebugFeatures}

        # Default: False
        SelectOpponentMoves={selectOpponentMoves}
        """);
    }
}