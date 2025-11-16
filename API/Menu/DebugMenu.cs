using System;
using System.Diagnostics;
using System.Linq;
using API.Graphics;
using API.Input;
using API.Modding;
using Microsoft.Xna.Framework;

namespace API.Menu;

public static class DebugMenu {
    private const uint Mb = 1024 * 1024;

    private static readonly Process Process = Process.GetCurrentProcess();

    private static TimeSpan timeSinceUpdate = TimeSpan.FromSeconds(1);

    private static TimeSpan avgFrameTime = TimeSpan.FromMilliseconds(10);

    private static readonly Label DebugInfoL = new(Core.StageSuper) {
        Text = GetDebugInfoLText(),
        Position = new Vector2(10, 10),
        HasBackground = true
    };

    private static readonly Label DebugInfoR = new(Core.StageSuper) {
        Position = new Vector2(World.W - 10, 10),
        Alignment = Alignment.TopRight,
        HasBackground = true
    };

    private static readonly Label DebugInfoHelp = new(Core.StageSuper) {
        Text = GetDebugInfoHelpText(),
        Position = new Vector2(10, World.H - 10),
        Alignment = Alignment.BottomLeft,
        HasBackground = true
    };

    public static void HandleDebugInfo(bool isDebugInfoEnabled, GameTime gameTime) {
        DebugInfoL.IsVisible = isDebugInfoEnabled;
        DebugInfoR.IsVisible = isDebugInfoEnabled;

        if (!isDebugInfoEnabled) {
            DebugInfoHelp.IsVisible = false;
            return;
        }

        if (Core.Input.InputDeviceChanged) DebugInfoL.Text = GetDebugInfoLText();

        // todo update text if lang changed

        // Lerped FPS counter
        avgFrameTime += (gameTime.ElapsedGameTime - avgFrameTime) * 0.01f;

        timeSinceUpdate += gameTime.ElapsedGameTime;

        // Check for inputs
        DebugInfoHelp.IsVisible ^= Core.Input.CheckInput(Keybinds.DebugHelp);

        if (Core.Input.CheckInput(Keybinds.DebugDumpMods)) {
            Console.WriteLine(string.Join(", ", ModLoader.LoadedMods).Replace(".Main", ""));
        }

        // Update timed text
        if (timeSinceUpdate < TimeSpan.FromSeconds(1)) return;

        DebugInfoR.Text = string.Format(Lang.DebugInfoR,
            $"{(int) (1 / avgFrameTime.TotalSeconds)}({(int) (1 / gameTime.ElapsedGameTime.TotalSeconds)})", // todo temp
            GC.GetTotalMemory(false) / Mb,
            "todo",
            "todo",
            string.Join(", ", Core.NavPath.Reverse()),
            "todo",
            ModLoader.LoadedMods.Count);

        timeSinceUpdate = TimeSpan.Zero;
    }

    private static string GetDebugInfoLText() =>
        string.Format(Lang.DebugInfoL, Keybinds.DebugInfo.GetCurrentGlyph(), BuildInfo.BuildDate);

    private static string GetDebugInfoHelpText() => Lang.DebugInfoHelp;
}