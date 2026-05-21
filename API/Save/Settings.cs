using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using API.Debug;
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

    // todo shoudl this exist?
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

    private const float _DefaultVolume = 0.75f;
    public static Progress MusicVolume;
    public static Progress SfxVolume;

    #endregion

    #region Controls

    // todo

    public static bool EnableMouse;
    public static bool ShowInputGuide;
    public static bool DetectNintendoController;

    #endregion

    #region Debug

    public static bool EnableCheats;

    #endregion

    #region Methods

    internal static void _Init()
    {
        Reload();
    }

    /// <summary>
    /// Reloads all settings from the config file
    /// </summary>
    public static void Reload()
    {
        try
        {
            Exception? parseError = Properties.TryParse(FilePath, out Dictionary<string, string>? dict);

            if (parseError is not null)
            {
                AllSettings = [];
                _ResetWithErrorMsg(parseError);
            }

            Assert.NotNull(dict);
            AllSettings = dict!;
        }
        catch (Exception e)
        {
            AllSettings = [];
            _ResetWithErrorMsg(e);
        }

        // Gameplay
        if (AllSettings.TryGetValue(nameof(Language), out string langId))
        {
            if (!Lang.Language.Langs.TryGetValue(langId, out Lang.Language l))
            {
                Core._LogOrEarlyLog($"\"Language\" invalid, defaulting to {Lang.Language.EnUS}",
                    nameof(Settings), LogLevel.Err);

                Language = Lang.Language.English;
            }
            else
            {
                Language = l;
            }
        }
        else
        {
            Core._LogOrEarlyLog($"\"Language\" unset, defaulting to {Lang.Language.EnUS}",
                nameof(Settings), LogLevel.Err);

            Language = Lang.Language.English;
        }

        BattleSpeed = _ParseFloatSetting(nameof(BattleSpeed), 1f);
        ShowInvalidMoveWarning = _ParseBoolSetting(nameof(ShowInvalidMoveWarning), true);

        // Display
        Resolution = _ParseIntSetting(nameof(Resolution), Auto);
        Fullscreen = _ParseBoolSetting(nameof(Fullscreen), true);
        EnableVsync = _ParseBoolSetting(nameof(EnableVsync), true);

        if (AllSettings.TryGetValue(nameof(Theme), out string themeId))
        {
            IRegistrable? themeMaybe = Registry.Get(themeId!);

            if (themeMaybe is Theme t)
            {
                Theme = t;
            }
            else
            {
                Core._LogOrEarlyLog($"\"Theme\" invalid, defaulting to {Theme.SeleneAbyss.GetId()}",
                    nameof(Settings), LogLevel.Err);

                Theme = Theme.SeleneAbyss;
            }
        }
        else
        {
            Core._LogOrEarlyLog($"\"Theme\" unset, defaulting to {Theme.SeleneAbyss.GetId()}",
                nameof(Settings), LogLevel.Err);

            Theme = Theme.SeleneAbyss;
        }

        // Audio
        MusicVolume = new Progress(_ParseFloatSetting(nameof(MusicVolume), _DefaultVolume));
        SfxVolume = new Progress(_ParseFloatSetting(nameof(SfxVolume), _DefaultVolume));

        // Controls
        EnableMouse = _ParseBoolSetting(nameof(EnableMouse), true);
        Core.Instance.IsMouseVisible = EnableMouse;

        ShowInputGuide = _ParseBoolSetting(nameof(ShowInputGuide), true);
        DetectNintendoController = _ParseBoolSetting(nameof(DetectNintendoController), true);

        // Debug
#if DEBUG
        EnableCheats = true;
#else
        EnableCheats = _ParseBoolSetting(nameof(EnableCheats), false);
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

        string t = true.ToString();
        string f = false.ToString();
        string vol = _DefaultVolume.ToString();

        // Gameplay
        AllSettings[nameof(BattleSpeed)] = "1";
        AllSettings[nameof(ShowInvalidMoveWarning)] = t;

        // Display
        AllSettings[nameof(Language)] = Lang.Language.EnUS;
        AllSettings[nameof(Resolution)] = Auto.ToString();
        AllSettings[nameof(Fullscreen)] = t;
        AllSettings[nameof(EnableVsync)] = t;
        AllSettings[nameof(Theme)] = Theme.SeleneAbyss.GetId();

        // Audio
        AllSettings[nameof(MusicVolume)] = vol;
        AllSettings[nameof(SfxVolume)] = vol;

        // Controls
        AllSettings[nameof(EnableMouse)] = t;
        AllSettings[nameof(ShowInputGuide)] = t;
        AllSettings[nameof(DetectNintendoController)] = t;

        // Debug
        AllSettings[nameof(EnableCheats)] = f;
    }

    public new static string ToString()
    {
        const int Cap = 1000;
        StringBuilder sb = new(Cap);

        int i = 0;
        foreach (KeyValuePair<string, string> kvp in AllSettings)
        {
            sb.Append($"{kvp.Key} = {kvp.Value}");

            i++;
            if (i != AllSettings.Count)
            {
                sb.Append('\n');
            }
        }

        Assert.CapIs(sb, Cap); // todo remove before final release
        return sb.ToString();
    }

    #endregion

    #region Parsing

    internal static void _ResetWithErrorMsg(Exception e)
    {
        Core._LogOrEarlyLog($"Failed to parse settings file, resetting to default: {e.Message}",
            nameof(Settings), LogLevel.Err);
        Reset();
        Write();
    }

    private static int _ParseIntSetting(string? key, int defaultValue)
    {
        if (!AllSettings.TryGetValue(key, out string setting))
        {
            _LogMissing(key, defaultValue);
            return defaultValue;
        }

        if (!int.TryParse(setting, out int res))
        {
            _LogInvalid(key, defaultValue, res);
            return defaultValue;
        }

        return res;
    }

    private static float _ParseFloatSetting(string? key, float defaultValue)
    {
        if (!AllSettings.TryGetValue(key, out string setting))
        {
            _LogMissing(key, defaultValue);
            return defaultValue;
        }

        if (!float.TryParse(setting, out float res))
        {
            _LogInvalid(key, defaultValue, res);
            return defaultValue;
        }

        return res;
    }

    private static bool _ParseBoolSetting(string? key, bool defaultValue)
    {
        if (!AllSettings.TryGetValue(key, out string setting))
        {
            _LogMissing(key, defaultValue);
            return defaultValue;
        }

        if (!bool.TryParse(setting, out bool res))
        {
            _LogInvalid(key, defaultValue, res);
            return defaultValue;
        }

        return res;
    }

    private static void _LogMissing<T>(string? key, T defaultValue)
    {
        Core._LogOrEarlyLog($"\"{key}\" missing, using default of {defaultValue}",
            nameof(Settings), LogLevel.Err);
    }

    private static void _LogInvalid<T>(string? key, T defaultValue, T res)
    {
        Core._LogOrEarlyLog($"\"{key}\" should be a {typeof(T)} but was set to {res}, using default of {defaultValue}",
            nameof(Settings), LogLevel.Err);
    }

    #endregion
}
