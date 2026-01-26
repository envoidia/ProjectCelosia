using System;
using System.Diagnostics;
using System.Text;
using API.Graphics;
using API.Input;
using API.Menu.State;
using API.Menu.Widget;
using API.Modding;
using API.Save;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace API.Debug;

// todo dont even init any of this stuff until its used
public static class DebugUtil
{
    private const string _ClassName = nameof(DebugUtil);

    internal const int _Mb = 1024 * 1024;

    private static TimeSpan _avgFrameTime = TimeSpan.FromMilliseconds(10);

    /// <summary>
    /// todo docs
    /// </summary>
    public static bool DrawDebugInfo { get; private set; } = false;
    public static bool DrawActorOutlines { get; set; } = false;
    public static bool DrawTheme { get; set; } = false;

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

    internal static readonly GraphWidget _PerfGraph = new(new(700), new(1500, 500),
        "Blue = update time, Green = draw time, Red = total", "Time (ms)", RenderPriority.Highest)
    {
        IsVisible = false,
        AnimType = AnimType.None
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

    internal static void _Init()
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

    // todo for some reason, setting this at init started to throw a nullreference exception wrt FSS text size
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

        DebugConsole._Update(gt);

        _CheckInputs();

        // todo update text if lang changed

        // Smoothed FPS counter
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
        if (!_DebugInfoR.IsVisible || _timeSinceUpdateInfoR < TimeSpan.FromSeconds(_InfoRUpdateRateS))
        {
            return;
        }

        _DebugInfoR.Text = $"FPS: {(int) (1 / _avgFrameTime.TotalSeconds)}\nRAM: {GC.GetTotalMemory(false) / _Mb}MB\nResolution: NYI\nStates: {StateMachine.ToString()}\nMenus: {StateMachine.State.GetMenuString()}\nActors on Stage: {Stage.ActorCount()}\nOverworld Location: NYI\nLoaded Mods: {ModLoader._LoadedMods.Count}";

        _timeSinceUpdateInfoR = TimeSpan.Zero;
    }

    // todo remove most of this
    private static void _CheckInputs()
    {
        if (InputLib.Check(Keybinds.DebugInfo))
        {
            _ToggleShowDebugInfo();
        }

        DebugConsole._Show ^= InputLib.IsKeyJustPressed(Keys.F2);
        DrawActorOutlines ^= InputLib.IsKeyJustPressed(Keys.F3);
    }

    internal static void _SetShowDebugInfo(bool show)
    {
        // temp
        _Test();

        _DebugInfoL.IsVisible = show;
        _DebugInfoR.IsVisible = show;
    }

    internal static void _ToggleShowDebugInfo()
    {
        _SetShowDebugInfo(!_DebugInfoL.IsVisible);
    }

    internal static void _SetShowInputView(bool show)
    {
        _DebugInfoKeyNames.IsVisible = show;
        _DebugInfoKeyHeld.IsVisible = show;
    }

    internal static void _ToggleShowInputView()
    {
        _DebugInfoKeyNames.IsVisible ^= true;
        _DebugInfoKeyHeld.IsVisible ^= true;
    }

    // todo cleanup
    private static string _GetInfoLText()
    {
        return $"{Keybinds.DebugInfo.GetCurrentGlyph()} Close, /i[KF2] Console, /i[KF3] Outlines\nVersion: {BuildInfo.BuildDate}";
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