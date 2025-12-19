using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API.Battle.State;
using API.Extensions;
using API.Graphics;
using API.Input;
using API.Menu.State;
using API.Modding;
using API.Save;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace API.Util;

public static class DebugUtil {
    // todo show size of all lists (IModItems, etc)
    private const string _ClassName = nameof(DebugUtil);

    private const int _Mb = 1024 * 1024;

    private const int _LogLimit = 8;
    private static readonly List<string> _LogText = new(8);

    private static TimeSpan _timeSinceUpdate = TimeSpan.FromSeconds(1);
    private static TimeSpan _avgFrameTime = TimeSpan.FromMilliseconds(10);

    internal static bool _drawDebugInfo = false;
    internal static bool _drawActorOutlines = false;
    internal static bool _drawPalette = false;

    #region Labels

    private static readonly Label _DebugInfoL = new() {
        Text = "_GetInfoLText()",
        Position = new(10, 10),
        Padding = new(10),
        HasBackground = true,
        Priority = RenderPriority.Highest,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private static readonly Label _DebugInfoR = new() {
        Position = new(World.W - 10, 10),
        Padding = new(10),
        HasBackground = true,
        Alignment = Alignment.TopRight,
        Priority = RenderPriority.Highest,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private static readonly Label _DebugInfoHelp = new() {
        Text = "_GetInfoHelpText()",
        Position = new(10, World.H - 10),
        Padding = new(10),
        HasBackground = true,
        Alignment = Alignment.BottomLeft,
        Priority = RenderPriority.Highest,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private static readonly Label _DebugLog = new() {
        Position = new(10, World.H - 10),
        Padding = new(10),
        HasBackground = true,
        Alignment = Alignment.BottomLeft,
        Priority = RenderPriority.Highest,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private const int _KeyYOff = 927;

    private static readonly Label _DebugInfoKeyNames = new() {
        Text = "_GetKeyNameText()",
        Position = World.Vec - new Vector2(412, _KeyYOff),
        Padding = new(10),
        HasBackground = true,
        Priority = RenderPriority.Highest,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private static readonly Label _DebugInfoKeyHeld = new() {
        Position = World.Vec - new Vector2(112, _KeyYOff),
        Padding = new(10, 120, 10, 10),
        HasBackground = true,
        Priority = RenderPriority.Highest,
        AnimType = AnimType.None,
        IsVisible = false
    };

    #endregion

    static DebugUtil() {
        Stage.Add(_DebugInfoL);
        Stage.Add(_DebugInfoR);
        Stage.Add(_DebugInfoHelp);
        Stage.Add(_DebugLog);

        Stage.Add(_DebugInfoKeyNames);
        Stage.Add(_DebugInfoKeyHeld);

        // todo this should only tick when its visible
        _DebugInfoKeyHeld.AddRoutine(InputLib._TrackInput);

        InputLib.OnDeviceChange += static () => {
            _DebugInfoL.Text = _GetInfoLText();
            _DebugInfoHelp.Text = _GetInfoHelpText();
            _DebugInfoKeyNames.Text = _GetKeyNameText();
        };
    }

    // todo for some reason, setting this in the static ctor started to throw a nullreference exception wrt FSS text size
    private static void _Test() {
        _DebugInfoL.Text = _GetInfoLText();
        _DebugInfoHelp.Text = _GetInfoHelpText();
        _DebugInfoKeyNames.Text = _GetKeyNameText();
    }

    /// <summary>
    /// Write a message to the ingame debug log and the attached OS console
    /// </summary>
    /// <param name="msg">Message</param>
    /// <param name="source">Origin to display for the message. API uses the name of the current class, but mods should
    /// use more specific names so it's clear exactly what mod it's coming from</param>
    /// <param name="logLevel">Color to use to indicate message severity</param>
    public static void Log(string msg, string source, LogLevel logLevel = LogLevel.Info) {
        string str = $"{logLevel switch {
            LogLevel.Info => ThemeColor.White.Str(),
            LogLevel.Warning => ThemeColor.Imp.Str(),
            LogLevel.Error => ThemeColor.Neg.Str(),
            _ => throw new ClosedEnumsWhenException()
        }}[{source}]{ThemeColor.White.Str()} {msg}";

        if (_LogText.Count == _LogLimit) _LogText.RemoveFirst();

        _LogText.Add(str);
        _DebugLog.Text = string.Join('\n', _LogText);

        Console.WriteLine(str);
    }

    /// <summary>
    /// Determines the color of log messages
    /// </summary>
    public enum LogLevel {
        Info,
        Warning,
        Error
    }

    internal static void _Update(GameTime gameTime) {
        if (!Settings.EnableDebugFeatures) return;

        _CheckInputs();

        // todo update text if lang changed


        // Lerped FPS counter
        _avgFrameTime += (gameTime.ElapsedGameTime - _avgFrameTime) * 0.01f;

        _timeSinceUpdate += gameTime.ElapsedGameTime;

        // Update timed text
        if (_timeSinceUpdate < TimeSpan.FromSeconds(1)) return;

        _DebugInfoR.Text = string.Format(Lang.DebugInfoR,
            $"{(int) (1 / _avgFrameTime.TotalSeconds)}({(int) (1 / gameTime.ElapsedGameTime.TotalSeconds)})", // todo temp
            GC.GetTotalMemory(false) / _Mb,
            "todo",
            StateMachine.ToString(),
            StateMachine.GetState().GetMenuString(),
            Stage.ActorCount(),
            "todo",
            ModLoader._LoadedMods.Count);

        _timeSinceUpdate = TimeSpan.Zero;
    }

    private static void _CheckInputs() {
        if (InputLib.Check(Keybinds.DebugInfo)) {
            // temp
            _Test();

            if (InputLib.Check(Keybinds.Hotkey1)) {
                _DebugInfoHelp.IsVisible ^= true;
            } else {
                _DebugInfoL.IsVisible ^= true;
                _DebugInfoR.IsVisible ^= true;
            }
        }

        if (InputLib.IsKeyJustPressed(Keys.F2)) {
            if (InputLib.Check(Keybinds.Hotkey1)) {
                Console.WriteLine(_DebugLog.Text);
                Log("Output debug log to console", _ClassName);
            } else _DebugLog.IsVisible ^= true;
        }

        _drawActorOutlines ^= InputLib.IsKeyJustPressed(Keys.F3);

        if (InputLib.IsKeyJustPressed(Keys.F4)) {
            _DebugInfoKeyNames.IsVisible ^= true;
            _DebugInfoKeyHeld.IsVisible ^= true;
        }

        if (InputLib.IsKeyJustPressed(Keys.F5)) {
            Console.WriteLine(Stage.ToString());
            Log("Output Stage to console", _ClassName);
        }

        if (InputLib.IsKeyJustPressed(Keys.F6)) {
            string str = string.Join('\n', LogLib._LogText);

            if (InputLib.Check(Keybinds.Hotkey1)) {
                Console.WriteLine(str);
                Log("Output raw battle log to console", _ClassName);
            } else {
                Console.WriteLine(Regexes.RemoveFormattingCodes(str));
                Log("Output battle log to console", _ClassName);
            }
        }

        if (InputLib.IsKeyJustPressed(Keys.F7)) {
            if (InputLib.Check(Keybinds.Hotkey1)) _CyclePalette();
            else if (InputLib.Check(Keybinds.Hotkey2)) {
                Console.WriteLine(Settings.Theme.ToString());
                Log("Output Theme to console", _ClassName);
            } else _drawPalette ^= true;
        }

        if (InputLib.IsKeyJustPressed(Keys.F8)) {
            Console.WriteLine(string.Join(", ", ModLoader._LoadedMods));
            Log("Output LoadedMods to console", _ClassName);
        }

        // todo remove functions after this? theyre not rly used

        if (InputLib.IsKeyJustPressed(Keys.F9)) {
            Stage._RecalcLayoutWidgets();
            Log("Recalculated ILayoutWidgets", _ClassName);
        }

        if (InputLib.IsKeyJustPressed(Keys.F10)) {
            Stage.Cleanup();
            Log("Cleaned up Stage", _ClassName);
        }

        if (InputLib.IsKeyJustPressed(Keys.F11)) {
            GC.Collect();
            Log("Forced GC collect", _ClassName);
        }
    }

    /// <summary>
    /// Increase current palette index by 1 or loop around
    /// </summary>
    private static void _CyclePalette() {
        Theme[] themes = [.. Registry.OfType<Theme>()];
        int i = themes.IndexOf(Settings.Theme);
        Settings.Theme = themes[i == themes.Length - 1 ? 0 : i + 1];
        Log($"Theme changed to {Settings.Theme.GetName()}", _ClassName);
    }

    // todo cleanup
    private static string _GetInfoLText() =>
        string.Format(Lang.DebugInfoL, Keybinds.DebugInfo.GetCurrentGlyph(),
            Keybinds.Hotkey1.GetCurrentGlyph(), BuildInfo.BuildDate);

    private static string _GetInfoHelpText() =>
        string.Format(Lang.DebugInfoHelp, Keybinds.Hotkey1.GetCurrentGlyph(), Keybinds.Hotkey2.GetCurrentGlyph());

    private static string _GetKeyNameText() {
        StringBuilder sb = new();
        for (int i = 0; i < Keybinds.UniqueKeybinds.Count; i++) {
            Keybind kb = Keybinds.UniqueKeybinds[i];
            sb.Append(kb.GetCurrentGlyph()).Append(kb.GetName());

            if (i != Keybinds.UniqueKeybinds.Count - 1) sb.Append('\n');
        }

        return sb.ToString();
    }


}