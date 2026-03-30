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

    // Speed of battle animations
    // Duration of in-battle pauses relative to 100% (1 = 100%, 0.1 = 10%)
    public static float BattleSpeed;

    public static bool ShowInvalidMoveWarning;

    #endregion

    #region Display

    public static Lang.Language Language
    {
        get;
        set
        {
            field = value;
            Lang.Language._Change();
        }
    } = null!;

    public const int Auto = -1;

    /// <summary>
    /// Y resolution. <c>Auto</c> = auto
    /// </summary>
    public static int Resolution;

    public static bool Fullscreen;

    public static bool EnableVsync
    {
        get;
        set
        {
            field = value;
            Core.Graphics.SynchronizeWithVerticalRetrace = value;
        }
    }

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

    public static bool EnableCheats;

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
        if (File.Exists(FilePath))
        {
            AllSettings = Properties.Parse(FilePath);
        }
        else
        {
            AllSettings = new();
            Reset();
            Write();
        }

        // Gameplay
        Language = Lang.Language.Langs.GetValueOrDefault(
            AllSettings.GetValueOrDefault(nameof(Language), Lang.Language.EnUS), Lang.Language.English);

        BattleSpeed = float.ParseOrDefault(AllSettings.GetValueOrDefault(nameof(BattleSpeed)), 1f);
        ShowInvalidMoveWarning = bool.ParseOrDefault(AllSettings.GetValueOrDefault(nameof(ShowInvalidMoveWarning)), true);

        // Display
        Resolution = int.ParseOrDefault(AllSettings.GetValueOrDefault(nameof(Resolution)), Auto);
        Fullscreen = bool.ParseOrDefault(AllSettings.GetValueOrDefault(nameof(Fullscreen)), true);
        EnableVsync = bool.ParseOrDefault(AllSettings.GetValueOrDefault(nameof(EnableVsync)), true);

        IRegistrable? r = Registry.Get(AllSettings.GetValueOrDefault(nameof(Theme), Theme.Apollo.GetId()));
        Theme = r is Theme t ? t : Theme.Apollo;

        // Audio
        MusicVolume = Progress.ParseOrDefault(AllSettings.GetValueOrDefault(nameof(MusicVolume)), new(0.75f));
        SfxVolume = Progress.ParseOrDefault(AllSettings.GetValueOrDefault(nameof(SfxVolume)), new(0.75f));

        // Controls
        ShowInputGuide = bool.ParseOrDefault(AllSettings.GetValueOrDefault(nameof(ShowInputGuide)), true);
        DetectNintendoController = bool.ParseOrDefault(AllSettings.GetValueOrDefault(nameof(DetectNintendoController)), true);

        // Debug
#if DEBUG
        EnableCheats = true;
#else
        EnableCheats = bool.ParseOrDefault(AllSettings.GetValueOrDefault(nameof(EnableCheats)), false);
#endif

        Core.Graphics.ApplyChanges();
    }

    public static void Write()
    {
        Properties.Create(FilePath, AllSettings);
    }

    public static void Reset()
    {
        AllSettings.Clear();

        // Gameplay
        AllSettings[nameof(BattleSpeed)] = "1";
        AllSettings[nameof(ShowInvalidMoveWarning)] = "true";

        // Display
        AllSettings[nameof(Language)] = Lang.Language.EnUS;
        AllSettings[nameof(Resolution)] = Auto.ToString();
        AllSettings[nameof(Fullscreen)] = "true";
        AllSettings[nameof(EnableVsync)] = "true";
        AllSettings[nameof(Theme)] = Theme.Apollo.GetId();

        // Audio
        AllSettings[nameof(MusicVolume)] = "0.75";
        AllSettings[nameof(SfxVolume)] = "0.75";

        // Controls
        AllSettings[nameof(ShowInputGuide)] = "true";
        AllSettings[nameof(DetectNintendoController)] = "true";

        // Debug
        AllSettings[nameof(EnableCheats)] = "false";
    }
}