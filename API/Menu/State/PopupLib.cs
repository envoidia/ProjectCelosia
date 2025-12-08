using API.Graphics;
using API.Input;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

public static class PopupLib {
    private static readonly GuiBox _PopupBg = new(World.W2 - 660, World.H2 + 300, World.W2 + 660, World.H2 - 300) {
        Priority = Priority.Highest
    };

    private static readonly Label _PopupTitle = new() {
        Position = new Vector2(World.W2, World.H2 - 225),
        Alignment = Alignment.Center,
        Priority = Priority.Highest
    };

    private static readonly Label _PopupText = new() {
        Position = new Vector2(World.W2 - 630, World.H2 - 120),
        Priority = Priority.Highest
    };

    /// <summary>
    /// Adds the relevant <c>Actor</c>s to the <c>Stage</c>.
    /// Doesn't resort because these should always be on top
    /// </summary>
    internal static void _Create() {
        Stage.Add(_PopupBg);
        Stage.Add(_PopupTitle);
        Stage.Add(_PopupText);

        _PopupBg.AddRoutine(IAnimatedPrimitive.In);

        Stage._needsSorting = false;
    }

    /// <summary>
    /// Removes the relevant <c>Actor</c>s from the <c>Stage</c>.
    /// Doesn't resort because these should always be on top
    /// </summary>
    internal static void _Destroy() {
        _PopupTitle.MarkForRemoval();
        _PopupText.MarkForRemoval();

        _PopupBg.AddRoutine(IAnimatedPrimitive.Out);

        Stage._needsSorting = false;

        Stage.Cleanup();
    }

    public static void Update(GameTime gameTime) {
        if (InputLib.Check(Keybinds.Back)) {
            StateMachine.Remove();
            return;
        }
    }
}
