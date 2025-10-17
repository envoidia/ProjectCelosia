using API.Graphics;
using Microsoft.Xna.Framework;

namespace API.Debug;

public static class DebugMenu {
    private const uint Mb = 1024 * 1024;

    private static readonly Label DebugInfoL = new Label.Builder(Core.Koruri25)
        .SetPosition(Vector2.One * 10).HasBackground().Build();

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

        DebugInfoL.Text = $"""
                           Press F3 to close this menu
                           Version: 0a-todo
                           FPS: {1.0f / gameTime.ElapsedGameTime.TotalSeconds}
                           Resolution: todo
                           NavPath: todo
                           Overworld Location: todo
                           Loaded Mod Count: todo
                           """;

        DebugInfoR.Text = $"""
                           OS: todo
                           RAM: {System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64 / Mb}MB
                           CPU: todo
                           GPU: todo
                           Last Input Source: todo
                           """;
    }
}