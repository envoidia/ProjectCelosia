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

    private static readonly Label _DebugInfoL = new(RenderPriority.Highest, Core.Mono40)
    {
        Text = "_GetInfoLText()", // todo
        Position = new(10, 10),
        Padding = new(10),
        HasBackground = true,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private static readonly Label _DebugInfoR = new(RenderPriority.Highest, Core.Mono40)
    {
        Position = new(World.W - 10, 10),
        Padding = new(10),
        HasBackground = true,
        Alignment = Alignment.TopRight,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private const int _InfoRUpdateRateS = 1;
    private static TimeSpan _timeSinceUpdateInfoR = TimeSpan.FromSeconds(_InfoRUpdateRateS);

    private const int _KeyYOff = 576;

    internal static readonly GraphWidget _PerfGraph = new(new(700), new(1500, 500),
        "Blue = update time, Green = draw time, Red = total", "Time (ms)", RenderPriority.Highest)
    {
        IsVisible = false,
        AnimType = AnimType.None
    };

    private const float _GraphUpdateRateS = 0.025f;
    private static TimeSpan _timeSinceUpdateGraph = TimeSpan.FromSeconds(_GraphUpdateRateS);

    private static readonly Label _DebugInfoKeyNames = new(RenderPriority.Highest, Core.Mono40)
    {
        Text = "_GetKeyNameText()",
        Position = World.Vec - new Vector2(422, _KeyYOff),
        Padding = new(10),
        HasBackground = true,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private static readonly Label _DebugInfoKeyHeld = new(RenderPriority.Highest, Core.Mono40)
    {
        Position = World.Vec - new Vector2(112, _KeyYOff),
        Padding = new(10, 120, 10, 10),
        HasBackground = true,
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

    private static void _CheckInputs()
    {
        if (InputLib.IsKeyJustPressed(Keys.F1))
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
        return $"[F1] Close, [F2] Console, [F3] Outlines\nVersion: {BuildInfo.BuildDate}";
    }

    private static string _GetKeyNameText()
    {
        const int Cap = 450;
        StringBuilder sb = new(Cap);
        for (int i = 0; i < Keybinds.NonMergedKeybinds.Count; i++)
        {
            Keybind kb = Keybinds.NonMergedKeybinds[i];
            sb.Append($"{ThemeColor.Imp.Str}[{kb.GetCurrentGlyphName()}]{ThemeColor.White.Str} {kb.GetName()}");

            if (i != Keybinds.NonMergedKeybinds.Count - 1)
            {
                sb.Append('\n');
            }
        }

        Assert.CapIs(sb, Cap);
        return sb.ToString();
    }


}