using System;
using API.Graphics;
using Microsoft.Xna.Framework;

namespace API.Debug;

public static class DebugMenu {
    private const uint Mb = 1024 * 1024;

    private static TimeSpan timeSinceUpdate = TimeSpan.FromSeconds(1);

    private static readonly Label DebugInfoL = new Label.Builder(Core.Koruri25)
        .SetText($"""
                  Press F1 to close this menu
                  Version: {BuildInfo.BuildDate}
                  OS: todo
                  CPU: todo
                  GPU: todo
                  """).SetPosition(Vector2.One * 10).HasBackground().Build();

    private static readonly Label DebugInfoR = new Label.Builder(Core.Koruri25)
        .SetPosition(new Vector2(1920 - 10, 10)).SetAlignment(Alignment.TopRight).HasBackground().Build();

    public static void HandleDebugInfo(bool isDebugInfoEnabled, GameTime gameTime) {
        if (!isDebugInfoEnabled) {
            DebugInfoL.Visible = false;
            DebugInfoR.Visible = false;
            return;
        }

        DebugInfoL.Visible = true;
        DebugInfoR.Visible = true;

        timeSinceUpdate += gameTime.ElapsedGameTime;
        if (timeSinceUpdate < TimeSpan.FromSeconds(1)) return;

        // todo average fps
        DebugInfoR.Text = $"""
                           FPS: {1.0f / gameTime.ElapsedGameTime.TotalSeconds}
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