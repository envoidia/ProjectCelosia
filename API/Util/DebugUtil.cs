using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// todo dont even init any of this stuff until its used
public static class DebugUtil
{
    private const string _ClassName = nameof(DebugUtil);

    private const int _Mb = 1024 * 1024;

    private static TimeSpan _avgFrameTime = TimeSpan.FromMilliseconds(10);

    /// <summary>
    /// todo docs
    /// </summary>
    public static bool DrawDebugInfo { get; private set; } = false;
    public static bool DrawDebugInfoHelp { get; private set; } = false;
    public static bool DrawActorOutlines { get; private set; } = false;
    public static bool DrawPalette { get; private set; } = false;

    public static Stopwatch Stopwatch = new();
    public static TimeSpan LastUpdateTime = TimeSpan.Zero;
    public static TimeSpan LastDrawTime = TimeSpan.Zero;

    #region Labels

    private static readonly Label _DebugInfoL = new()
    {
        Text = "_GetInfoLText()",
        Position = new(10, 10),
        Padding = new(10),
        HasBackground = true,
        Priority = RenderPriority.Highest,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private static readonly Label _DebugInfoR = new()
    {
        Position = new(World.W - 10, 10),
        Padding = new(10),
        HasBackground = true,
        Alignment = Alignment.TopRight,
        Priority = RenderPriority.Highest,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private const int _InfoRUpdateRateS = 1;
    private static TimeSpan _timeSinceUpdateInfoR = TimeSpan.FromSeconds(_InfoRUpdateRateS);

    private const int _KeyYOff = 927;

    private static readonly FrameGraph _PerfGraph = new(new(700), new(1500, 500),
        "Blue = update time, Green = draw time, Red = total", "Time (ms)", RenderPriority.Highest)
    {
        IsVisible = false
    };

    private const float _GraphUpdateRateS = 0.025f;
    private static TimeSpan _timeSinceUpdateGraph = TimeSpan.FromSeconds(_GraphUpdateRateS);

    private static readonly Label _DebugInfoKeyNames = new()
    {
        Text = "_GetKeyNameText()",
        Position = World.Vec - new Vector2(412, _KeyYOff),
        Padding = new(10),
        HasBackground = true,
        Priority = RenderPriority.Highest,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private static readonly Label _DebugInfoKeyHeld = new()
    {
        Position = World.Vec - new Vector2(112, _KeyYOff),
        Padding = new(10, 120, 10, 10),
        HasBackground = true,
        Priority = RenderPriority.Highest,
        AnimType = AnimType.None,
        IsVisible = false
    };

    #endregion

    static DebugUtil()
    {
        Stage.Add(_DebugInfoL);
        Stage.Add(_DebugInfoR);

        Stage.Add(_PerfGraph);

        Stage.Add(_DebugInfoKeyNames);
        Stage.Add(_DebugInfoKeyHeld);

        // todo this should only tick when its visible
        _DebugInfoKeyHeld.AddRoutine(InputLib._TrackInput);

        InputLib.OnDeviceChange += static () =>
        {
            _DebugInfoL.Text = _GetInfoLText();
            _DebugInfoKeyNames.Text = _GetKeyNameText();
        };
    }

    // todo for some reason, setting this in the static ctor started to throw a nullreference exception wrt FSS text size
    private static void _Test()
    {
        _DebugInfoL.Text = _GetInfoLText();
        _DebugInfoKeyNames.Text = _GetKeyNameText();
    }

    internal static void _Update(GameTime gt)
    {
        if (!Settings.EnableDebugFeatures)
        {
            return;
        }

        DebugConsole.Update(gt);
        _CheckInputs();


        // todo update text if lang changed

        // Lerped FPS counter
        _avgFrameTime += (gt.ElapsedGameTime - _avgFrameTime) * 0.01f;

        _timeSinceUpdateInfoR += gt.ElapsedGameTime;

        _timeSinceUpdateGraph += gt.ElapsedGameTime;

        // Update performance graph
        if (_timeSinceUpdateGraph > TimeSpan.FromSeconds(_GraphUpdateRateS))
        {
            float u = (float) LastUpdateTime.TotalMilliseconds;
            float d = (float) LastDrawTime.TotalMilliseconds;

            _PerfGraph.AddPoint(0, u + d);
            _PerfGraph.AddPoint(1, u);
            _PerfGraph.AddPoint(2, d);

            _timeSinceUpdateGraph = TimeSpan.Zero;
        }

        // Update timed text
        if (_timeSinceUpdateInfoR < TimeSpan.FromSeconds(_InfoRUpdateRateS))
        {
            return;
        }

        _DebugInfoR.Text = string.Format("DebugInfoR".GetLang(),
           $"{(int) (1 / _avgFrameTime.TotalSeconds)}",
           GC.GetTotalMemory(false) / _Mb,
           "todo",
           StateMachine.ToString(),
           StateMachine.State.GetMenuString(),
           Stage.ActorCount(),
           "todo",
           ModLoader._LoadedMods.Count);

        _timeSinceUpdateInfoR = TimeSpan.Zero;
    }

    private static void _CheckInputs()
    {
        if (InputLib.Check(Keybinds.DebugInfo))
        {
            // temp
            _Test();

            if (InputLib.Check(Keybinds.Hotkey1))
            {
                DrawDebugInfoHelp ^= true;
                _DebugInfoL.Text = _GetInfoLText();
            }
            else
            {
                _DebugInfoL.IsVisible ^= true;
                _DebugInfoR.IsVisible ^= true;
            }
        }

        DrawActorOutlines ^= InputLib.IsKeyJustPressed(Keys.F3);

        if (InputLib.IsKeyJustPressed(Keys.F4))
        {
            _DebugInfoKeyNames.IsVisible ^= true;
            _DebugInfoKeyHeld.IsVisible ^= true;
        }

        if (InputLib.IsKeyJustPressed(Keys.F5))
        {
            Console.WriteLine(string.Join(", ", ModLoader._LoadedMods));
            DebugConsole.Log("Output LoadedMods to console", _ClassName);
        }

        if (InputLib.IsKeyJustPressed(Keys.F6))
        {
            Console.WriteLine(Registry.ToString());
            DebugConsole.Log("Output Registry to console", _ClassName);
        }

        if (InputLib.IsKeyJustPressed(Keys.F7))
        {
            Console.WriteLine(Settings.Language.ToString());
            DebugConsole.Log("Output current language to console", _ClassName);
        }

        if (InputLib.IsKeyJustPressed(Keys.F8))
        {
            Console.WriteLine(Stage.ToString());
            DebugConsole.Log("Output Stage to console", _ClassName);
        }

        _PerfGraph.IsVisible ^= InputLib.IsKeyJustPressed(Keys.Q);

        if (InputLib.IsKeyJustPressed(Keys.F9))
        {
            string str = string.Join('\n', LogLib._LogText);

            if (InputLib.Check(Keybinds.Hotkey1))
            {
                Console.WriteLine(str);
                DebugConsole.Log("Output raw battle log to console", _ClassName);
            }
            else
            {
                Console.WriteLine(str.RemoveFormattingCodes());
                DebugConsole.Log("Output battle log to console", _ClassName);
            }
        }

        if (InputLib.IsKeyJustPressed(Keys.F10))
        {
            if (InputLib.Check(Keybinds.Hotkey1))
            {
                _CyclePalette();
            }

            else if (InputLib.Check(Keybinds.Hotkey2))
            {
                Console.WriteLine(Settings.Theme.ToDetailedString());
                DebugConsole.Log("Output Theme to console", _ClassName);
            }
            else
            {
                DrawPalette ^= true;
            }
        }

        // todo remove functions after this? theyre not rly used

        if (InputLib.IsKeyJustPressed(Keys.F11))
        {
            Stage._RecalcLayoutWidgets();
            DebugConsole.Log("Recalculated ILayoutWidgets", _ClassName);
        }

        if (InputLib.IsKeyJustPressed(Keys.F12))
        {
            Stage.Sort();
            DebugConsole.Log("Cleaned up Stage", _ClassName);
        }
    }

    /// <summary>
    /// Increase current palette index by 1 or loop around
    /// </summary>
    private static void _CyclePalette()
    {
        Theme[] themes = [.. Registry.Of<Theme>()];
        int i = themes.IndexOf(Settings.Theme);
        Settings.Theme = themes[i == themes.Length - 1 ? 0 : i + 1];
        DebugConsole.Log($"Theme changed to {Settings.Theme.GetName().RemoveFormattingCodes()}", _ClassName);
    }

    // todo cleanup
    private static string _GetInfoLText()
    {
        return string.Format("DebugInfoL".GetLang(), Keybinds.DebugInfo.GetCurrentGlyph(),
            Keybinds.Hotkey1.GetCurrentGlyph(), BuildInfo.BuildDate) +
            (DrawDebugInfoHelp ? $"\n{_GetInfoHelpText()}" : "");
    }

    private static string _GetInfoHelpText()
    {
        return string.Format("DebugInfoHelp".GetLang(), Keybinds.Hotkey1.GetCurrentGlyph(), Keybinds.Hotkey2.GetCurrentGlyph());
    }

    private static string _GetKeyNameText()
    {
        StringBuilder sb = new();
        for (int i = 0; i < Keybinds.UniqueKeybinds.Count; i++)
        {
            Keybind kb = Keybinds.UniqueKeybinds[i];
            sb.Append(kb.GetCurrentGlyph()).Append(kb.GetName());

            if (i != Keybinds.UniqueKeybinds.Count - 1)
            {
                sb.Append('\n');
            }
        }

        return sb.ToString();
    }


}