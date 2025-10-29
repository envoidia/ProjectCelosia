using System;
using System.Linq;
using API.Graphics;
using API.Input;
using API.Menu;
using Microsoft.Xna.Framework;

namespace API.Debug;

public static class DebugMenu {
    private const uint Mb = 1024 * 1024;

    private static TimeSpan timeSinceUpdate = TimeSpan.FromSeconds(1);

    private static TimeSpan avgFrameTime = TimeSpan.FromMilliseconds(10);
    private const float Alpha = 0.01f;

    private static readonly Label DebugInfoL = new() {
        Text = GetDebugInfoLText(),
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
            DebugInfoL.Text = GetDebugInfoLText();
        }

        // Lerped FPS counter
        avgFrameTime += (gameTime.ElapsedGameTime - avgFrameTime) * 0.01f;

        timeSinceUpdate += gameTime.ElapsedGameTime;
        if (timeSinceUpdate < TimeSpan.FromSeconds(1)) return;

        DebugInfoR.Text = $"""
                           FPS: {(int) (1 / avgFrameTime.TotalSeconds)} ({(int) (1 / gameTime.ElapsedGameTime.TotalSeconds)})
                           RAM: {System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64 / Mb}MB
                           Last Input Source: todo
                           Resolution: todo
                           NavPath: {string.Join(", ", Core.NavPath.Reverse())}
                           Overworld Location: todo
                           Loaded Mod Count: todo
                           """;
        timeSinceUpdate = TimeSpan.Zero;
    }

    private static string GetDebugInfoLText() =>
        $"""
         Press {Keybind.DebugInfo.GetCurrentGlyph()} to close this menu
         Version: {BuildInfo.BuildDate}
         OS: todo
         CPU: todo
         GPU: todo
         """;
}