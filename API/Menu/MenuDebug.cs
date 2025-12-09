using System;
using API.Battle.State;
using API.Graphics;
using API.Input;
using API.Menu.State;
using API.Modding;
using API.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace API.Menu;

public static class MenuDebug {
    private const int _Mb = 1024 * 1024;

    private static TimeSpan _timeSinceUpdate = TimeSpan.FromSeconds(1);

    private static TimeSpan _avgFrameTime = TimeSpan.FromMilliseconds(10);

    private static readonly Label _DebugInfoL = new() {
        Text = _GetDebugInfoLText(),
        Position = new Vector2(10, 10),
        HasBackground = true,
        Priority = RenderPriority.Highest
    };

    private static readonly Label _DebugInfoR = new() {
        Position = new Vector2(World.W - 10, 10),
        Alignment = Alignment.TopRight,
        HasBackground = true,
        Priority = RenderPriority.Highest
    };

    private static readonly Label _DebugInfoHelp = new() {
        Text = _GetDebugInfoHelpText(),
        Position = new Vector2(10, World.H - 10),
        Alignment = Alignment.BottomLeft,
        HasBackground = true,
        IsVisible = false,
        Priority = RenderPriority.Highest
    };

    /// <summary>
    /// Adds the relevant <c>Actor</c>s to the <c>Stage</c>.
    /// Doesn't resort because these should always be on top
    /// </summary>
    public static void Create() {
        Stage.Add(_DebugInfoL);
        Stage.Add(_DebugInfoR);
        Stage.Add(_DebugInfoHelp);

        Stage.Cleanup();
    }

    /// <summary>
    /// Removes the relevant <c>Actor</c>s from the <c>Stage</c>.
    /// Doesn't resort because these should always be on top
    /// </summary>
    public static void Destroy() {
        _DebugInfoL.MarkForRemoval();
        _DebugInfoR.MarkForRemoval();
        _DebugInfoHelp.MarkForRemoval();

        Stage.Cleanup();
    }

    public static void Update(GameTime gameTime) {
        if (InputLib.InputDeviceChanged) {
            _DebugInfoL.Text = _GetDebugInfoLText();
            _DebugInfoHelp.Text = _GetDebugInfoHelpText();
        }

        // todo update text if lang changed

        // Lerped FPS counter
        _avgFrameTime += (gameTime.ElapsedGameTime - _avgFrameTime) * 0.01f;

        _timeSinceUpdate += gameTime.ElapsedGameTime;

        // Check for inputs
        _DebugInfoHelp.IsVisible ^= InputLib.IsKeyJustPressed(Keys.F2);

        if (InputLib.IsKeyJustPressed(Keys.F3)) {
            Console.WriteLine(string.Join(", ", ModLoader._LoadedMods));
        }

        if (InputLib.IsKeyJustPressed(Keys.F4)) {
            string str = string.Join('\n', LogLib._LogText);

            if (InputLib.Check(Keybinds.Hotkey)) {
                Console.WriteLine(str);
                return;
            }

            Console.WriteLine(Regexes.RemoveFormattingCodes(str));
        }

        if (InputLib.IsKeyJustPressed(Keys.F5)) Console.WriteLine(Stage.ToString());

        // Update timed text
        if (_timeSinceUpdate < TimeSpan.FromSeconds(1)) return;

        // todo is it at all reasonable to stackalloc this (probably not)
        _DebugInfoR.Text = string.Format(Lang.DebugInfoR,
            $"{(int) (1 / _avgFrameTime.TotalSeconds)}({(int) (1 / gameTime.ElapsedGameTime.TotalSeconds)})", // todo temp
            GC.GetTotalMemory(false) / _Mb,
            "todo",
            StateMachine.ToString(),
            "todo",
            ModLoader._LoadedMods.Count);

        _timeSinceUpdate = TimeSpan.Zero;
    }

    // todo cleanup
    private static string _GetDebugInfoLText() =>
        string.Format(Lang.DebugInfoL, Keybinds.DebugInfo.GetCurrentGlyph(), BuildInfo.BuildDate);

    private static string _GetDebugInfoHelpText() =>
        string.Format(Lang.DebugInfoHelp, Keybinds.Hotkey.GetCurrentGlyph());
}