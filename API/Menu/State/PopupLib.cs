using API.Graphics;
using API.Input;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

public static class PopupLib {
    private static readonly Label _PopupTitle = new(Core.StagePopup) {
        Position = new Vector2(World.W2, World.H2 - 225),
        Alignment = Alignment.Center
    };

    private static readonly Label _PopupText = new(Core.StagePopup) {
        Position = new Vector2(World.W2 - 630, World.H2 - 120),
    };

    private static readonly GuiBox _PopupBg = new(World.W2 - 660, World.H2 + 300, World.W2 + 660, World.H2 - 300);

    public static void Update(GameTime gameTime) {
        if (InputLib.Check(Keybinds.Back)) {
            NavPath.Remove();
            return;
        }
    }

    public static void Draw(GameTime gameTime) {
        // Draw the previous IState underneath
        NavPath.Path[^2].Draw(gameTime);

        // Draw popup
        Core.StagePopup.Draw(gameTime);
    }
}
