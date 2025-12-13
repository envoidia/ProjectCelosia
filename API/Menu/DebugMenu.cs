using System;
using System.Text;
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
        Text = _GetInfoLText(),
        Position = new Vector2(10, 10),
        Padding = new(10),
        HasBackground = true,
        Priority = RenderPriority.Highest
    };

    private static readonly Label _DebugInfoR = new() {
        Position = new Vector2(World.W - 10, 10),
        Padding = new(10),
        HasBackground = true,
        Alignment = Alignment.TopRight,
        Priority = RenderPriority.Highest
    };

    private static readonly Label _DebugInfoHelp = new() {
        Text = _GetInfoHelpText(),
        Position = new Vector2(10, World.H - 10),
        Padding = new(10),
        HasBackground = true,
        Alignment = Alignment.BottomLeft,
        Priority = RenderPriority.Highest,
        IsVisible = false
    };

    private const int _KeyYOff = 927;

    private static readonly Label _DebugInfoKeyNames = new() {
        Text = _GetKeyNameText(),
        Position = World.Vec - new Vector2(430, _KeyYOff),
        Padding = new(10),
        HasBackground = true,
        Priority = RenderPriority.Highest,
        IsVisible = false
    };

    private static readonly Label _DebugInfoKeyHeld = new() {
        Text = _GetKeyNameText(),
        Position = World.Vec - new Vector2(120, _KeyYOff),
        Padding = new(10, 120, 10, 10),
        HasBackground = true,
        Priority = RenderPriority.Highest,
        IsVisible = false
    };

    internal static bool _drawActorOutlines = true;

    static DebugMenu() {
        Stage.Add(_DebugInfoHelp);

        Stage.Add(_DebugInfoKeyNames);

        Stage.Add(_DebugInfoKeyHeld);
        _DebugInfoKeyHeld.AddRoutine(InputLib.TrackInput);
    }

    /// <summary>
    /// Adds the relevant <c>Actor</c>s to the <c>Stage</c>.
    /// </summary>
    public static void Create() {
        Stage.Add(_DebugInfoL);
        Stage.Add(_DebugInfoR);

        Stage.Cleanup();
    }

    /// <summary>
    /// Removes the relevant <c>Actor</c>s from the <c>Stage</c>.
    /// </summary>
    public static void Destroy() {
        _DebugInfoL.MarkForRemoval();
        _DebugInfoR.MarkForRemoval();

        Stage.Cleanup();
    }

    public static void Update(GameTime gameTime) {
        if (InputLib.InputDeviceChanged) {
            _DebugInfoL.Text = _GetInfoLText();
            _DebugInfoHelp.Text = _GetInfoHelpText();
            _DebugInfoKeyNames.Text = _GetKeyNameText();
        }

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

    internal static void _CheckDebugHotkeys() {
        _DebugInfoHelp.IsVisible ^= InputLib.IsKeyJustPressed(Keys.F2);

        _drawActorOutlines ^= InputLib.IsKeyJustPressed(Keys.F3);

        if (InputLib.IsKeyJustPressed(Keys.F4)) {
            _DebugInfoKeyNames.IsVisible ^= true;
            _DebugInfoKeyHeld.IsVisible ^= true;

        }
        if (InputLib.IsKeyJustPressed(Keys.F5)) Console.WriteLine(Stage.ToString());

        if (InputLib.IsKeyJustPressed(Keys.F6)) Stage.Cleanup();

        if (InputLib.IsKeyJustPressed(Keys.F7)) {
            string str = string.Join('\n', LogLib._LogText);

            if (InputLib.Check(Keybinds.Hotkey1)) {
                Console.WriteLine(str);
                return;
            }

            Console.WriteLine(Regexes.RemoveFormattingCodes(str));
        }

        if (InputLib.IsKeyJustPressed(Keys.F8)) {
            Console.WriteLine(string.Join(", ", ModLoader._LoadedMods));
        }

        if (InputLib.IsKeyJustPressed(Keys.F9)) GC.Collect();
    }

    // todo cleanup
    private static string _GetInfoLText() =>
        string.Format(Lang.DebugInfoL, Keybinds.DebugInfo.GetCurrentGlyph(), BuildInfo.BuildDate);

    private static string _GetInfoHelpText() =>
        string.Format(Lang.DebugInfoHelp, Keybinds.Hotkey1.GetCurrentGlyph());

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