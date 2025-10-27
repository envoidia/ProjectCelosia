using System;
using API.Graphics;
using API.Input;
using Microsoft.Xna.Framework;

namespace API.Debug;

public static class DebugMenu {
    private const uint Mb = 1024 * 1024;

    private static TimeSpan timeSinceUpdate = TimeSpan.FromSeconds(1);

    private static readonly Label DebugInfoL = new() {
        Text = $"""
                Press {Keybind.DebugInfo.GetCurrentGlyph()} to close this menu
                Version: {BuildInfo.BuildDate}
                OS: todo
                CPU: todo
                GPU: todo
                """,
        Position = new Vector2(10, 10),
        HasBackground = true
    };

    private static readonly Label DebugInfoR = new() {
        Position = new Vector2(World.W - 10, 10),
        Alignment = Alignment.TopRight,
        HasBackground = true
    };

    public static void HandleDebugInfo(bool isDebugInfoEnabled, GameTime gameTime) {
        if (!isDebugInfoEnabled) {
            DebugInfoL.Visible = false;
            DebugInfoR.Visible = false;
            return;
        }

        DebugInfoL.Visible = true;
        DebugInfoR.Visible = true;

        if (Core.Input.InputDeviceChanged) {
            DebugInfoL.Text = $"""
                               Press {Keybind.DebugInfo.GetCurrentGlyph()} to close this menu
                               Version: {BuildInfo.BuildDate}
                               OS: todo
                               CPU: todo
                               GPU: todo
                               """;
        }

        timeSinceUpdate += gameTime.ElapsedGameTime;
        if (timeSinceUpdate < TimeSpan.FromSeconds(1)) return;

        // todo average fps
        DebugInfoR.Text = $"""
                           FPS: {(int) (1.0f / gameTime.ElapsedGameTime.TotalSeconds)}
                           RAM: {System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64 / Mb}MB
                           Last Input Source: todo
                           Resolution: todo
                           NavPath: todo
                           Overworld Location: todo
                           Loaded Mod Count: todo
                           """;
        timeSinceUpdate = TimeSpan.Zero;
    }
}