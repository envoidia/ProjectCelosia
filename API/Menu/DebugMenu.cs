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

public static class DebugMenu {
    private const int _Mb = 1024 * 1024;

    private static TimeSpan _timeSinceUpdate = TimeSpan.FromSeconds(1);

    private static TimeSpan _avgFrameTime = TimeSpan.FromMilliseconds(10);

    private static readonly Label _DebugInfoL = new() {
        Text = _GetDebugInfoLText(),
        Position = new Vector2(10, 10),
        Padding = new Padding(10),
        HasBackground = true,
        Priority = RenderPriority.Highest
    };

    private static readonly Label _DebugInfoR = new() {
        Position = new Vector2(World.W - 10, 10),
        Padding = new Padding(10),
        HasBackground = true,
        Alignment = Alignment.TopRight,
        Priority = RenderPriority.Highest
    };

    private static readonly Label _DebugInfoHelp = new() {
        Text = _GetDebugInfoHelpText(),
        Position = new Vector2(10, World.H - 10),
        Padding = new Padding(10),
        HasBackground = true,
        Alignment = Alignment.BottomLeft,
        Priority = RenderPriority.Highest,
        IsVisible = false
    };

    // temp
    internal static bool _drawActorOutlines = true;

    /// <summary>
    /// Adds the relevant <c>Actor</c>s to the <c>Stage</c>.
    /// </summary>
    public static void Create() {
        Stage.Add(_DebugInfoL);
        Stage.Add(_DebugInfoR);
        Stage.Add(_DebugInfoHelp);

        Stage.Cleanup();
    }

    /// <summary>
    /// Removes the relevant <c>Actor</c>s from the <c>Stage</c>.
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

        // Toggle help text
        _DebugInfoHelp.IsVisible ^= InputLib.IsKeyJustPressed(Keys.F2);

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
            Stage.ActorCount(),
            "todo",
            ModLoader._LoadedMods.Count);

        _timeSinceUpdate = TimeSpan.Zero;
    }

    internal static void _CheckDebugHotkeys() {
        _drawActorOutlines ^= InputLib.IsKeyJustPressed(Keys.F3);

        if (InputLib.IsKeyJustPressed(Keys.F4)) Console.WriteLine(Stage.ToString());

        if (InputLib.IsKeyJustPressed(Keys.F5)) Stage.Cleanup();

        if (InputLib.IsKeyJustPressed(Keys.F6)) {
            string str = string.Join('\n', LogLib._LogText);

            if (InputLib.Check(Keybinds.Hotkey1)) {
                Console.WriteLine(str);
                return;
            }

            Console.WriteLine(Regexes.RemoveFormattingCodes(str));
        }

        if (InputLib.IsKeyJustPressed(Keys.F7)) {
            Console.WriteLine(string.Join(", ", ModLoader._LoadedMods));
        }

        // todo use first unused F key
        if (InputLib.IsKeyJustPressed(Keys.F12)) GC.Collect();
    }

    // todo cleanup
    private static string _GetDebugInfoLText() =>
        string.Format(Lang.DebugInfoL, Keybinds.DebugInfo.GetCurrentGlyph(), BuildInfo.BuildDate);

    private static string _GetDebugInfoHelpText() =>
        string.Format(Lang.DebugInfoHelp, Keybinds.Hotkey1.GetCurrentGlyph());
}