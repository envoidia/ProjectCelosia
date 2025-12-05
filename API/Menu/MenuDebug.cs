using System;
using System.Linq;
using API.Graphics;
using API.Input;
using API.Menu.State;
using API.Modding;
using Microsoft.Xna.Framework;

namespace API.Menu;

public static class MenuDebug {
    private const int _Mb = 1024 * 1024;

    private static TimeSpan _timeSinceUpdate = TimeSpan.FromSeconds(1);

    private static TimeSpan _avgFrameTime = TimeSpan.FromMilliseconds(10);

    private static readonly Label _DebugInfoL = new(Core.StageSuper) {
        Text = _GetDebugInfoLText(),
        Position = new Vector2(10, 10),
        HasBackground = true
    };

    private static readonly Label _DebugInfoR = new(Core.StageSuper) {
        Position = new Vector2(World.W - 10, 10),
        Alignment = Alignment.TopRight,
        HasBackground = true
    };

    private static readonly Label _DebugInfoHelp = new(Core.StageSuper) {
        Text = _GetDebugInfoHelpText(),
        Position = new Vector2(10, World.H - 10),
        Alignment = Alignment.BottomLeft,
        HasBackground = true
    };

    public static void HandleDebugInfo(bool isDebugInfoEnabled, GameTime gameTime) {
        _DebugInfoL.IsVisible = isDebugInfoEnabled;
        _DebugInfoR.IsVisible = isDebugInfoEnabled;

        if (!isDebugInfoEnabled) {
            _DebugInfoHelp.IsVisible = false;
            return;
        }

        if (InputLib.InputDeviceChanged) _DebugInfoL.Text = _GetDebugInfoLText();

        // todo update text if lang changed

        // Lerped FPS counter
        _avgFrameTime += (gameTime.ElapsedGameTime - _avgFrameTime) * 0.01f;

        _timeSinceUpdate += gameTime.ElapsedGameTime;

        // Check for inputs
        _DebugInfoHelp.IsVisible ^= InputLib.Check(Keybinds.DebugHelp);

        if (InputLib.Check(Keybinds.DebugDumpMods)) {
            Console.WriteLine(string.Join(", ", ModLoader._LoadedMods).Replace(".Main", ""));
        }

        // Update timed text
        if (_timeSinceUpdate < TimeSpan.FromSeconds(1)) return;

        // todo is it at all reasonable to stackalloc this (probably not)
        _DebugInfoR.Text = string.Format(Lang.DebugInfoR,
            $"{(int) (1 / _avgFrameTime.TotalSeconds)}({(int) (1 / gameTime.ElapsedGameTime.TotalSeconds)})", // todo temp
            GC.GetTotalMemory(false) / _Mb,
            "todo",
            string.Join(", ", [.. NavPath.Path.Select(s => s.Name)]), // todo sanitize string
            "todo",
            ModLoader._LoadedMods.Count);

        _timeSinceUpdate = TimeSpan.Zero;
    }

    private static string _GetDebugInfoLText() =>
        string.Format(Lang.DebugInfoL, Keybinds.DebugInfo.GetCurrentGlyph(), BuildInfo.BuildDate);

    private static string _GetDebugInfoHelpText() => Lang.DebugInfoHelp;
}