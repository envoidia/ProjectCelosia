using API.Graphics;

namespace API.Menu.State;

// todo remove
internal static class _PopupLib {
    private static readonly Parellelogram _PopupBg = new(World.W2 - 660, World.H2 + 300, World.W2 + 660, World.H2 - 300) {
        Priority = RenderPriority.Highest
    };

    private static readonly Label _PopupTitle = new() {
        Position = new(World.W2, World.H2 - 225),
        Alignment = Alignment.Center,
        Priority = RenderPriority.Highest
    };

    private static readonly Label _PopupText = new() {
        Position = new(World.W2 - 630, World.H2 - 120),
        Priority = RenderPriority.Highest
    };

    /// <summary>
    /// Adds the relevant <c>Actor</c>s to the <c>Stage</c>.
    /// Doesn't resort because these should always be on top
    /// </summary>
    internal static void _Create() {
        Stage.Add(_PopupBg);
        Stage.Add(_PopupTitle);
        Stage.Add(_PopupText);

        _PopupBg.AddRoutine(IActor.In);

        Stage._needsSorting = false;
    }

    /// <summary>
    /// Removes the relevant <c>Actor</c>s from the <c>Stage</c>.
    /// Doesn't resort because these should always be on top
    /// </summary>
    internal static void _Destroy() {
        Stage.Remove(_PopupTitle);
        Stage.Remove(_PopupText);

        _PopupBg.AddRoutine(IActor.Out);

        Stage._needsSorting = false;

        Stage.Cleanup();
    }
}
