using API.Graphics;
using API.Input;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

public static class PopupLib {
    private static readonly Label _PopupTitle = new(Stages.Popup) {
        Position = new Vector2(World.W2, World.H2 - 225),
        Alignment = Alignment.Center
    };

    private static readonly Label _PopupText = new(Stages.Popup) {
        Position = new Vector2(World.W2 - 630, World.H2 - 120),
    };

    private static readonly GuiBox _PopupBg = new(Stages.Super, World.W2 - 660, World.H2 + 300, World.W2 + 660, World.H2 - 300);

    public static void Update(GameTime gameTime) {
        if (InputLib.Check(Keybinds.Back)) {
            NavPath.Remove();
            return;
        }
    }
}
