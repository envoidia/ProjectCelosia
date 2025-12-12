using API.Input;
using API.Util;

namespace API.Menu;

/// <summary>
/// The directions that an <c>IWidget</c> would like to use for input
/// </summary>
public enum WidgetSelectionType {
    /// <summary>
    /// No navigation
    /// </summary>
    None,

    /// <summary>
    /// Left/Right
    /// </summary>
    Horiz,

    /// <summary>
    /// Up/Down
    /// </summary>
    Vert,

    /// <summary>
    /// Left/Right/Up/Down, 1 axis
    /// </summary>
    HorizVert,

    /// <summary>
    /// PageL/PageR
    /// </summary>
    Page
}

// todo account for page
public static class WidgetSelectionTypeExtensions {
    extension(WidgetSelectionType @this) {
        public Keybind GetInc() => @this switch {
            WidgetSelectionType.Horiz => Keybinds.Right,
            WidgetSelectionType.Vert => Keybinds.Down,
            WidgetSelectionType.HorizVert => Keybinds.RightDown,
            WidgetSelectionType.Page => Keybinds.PageR,
            _ => throw new ClosedEnumsWhenException()
        };

        public Keybind GetDec() => @this switch {
            WidgetSelectionType.Horiz => Keybinds.Left,
            WidgetSelectionType.Vert => Keybinds.Up,
            WidgetSelectionType.HorizVert => Keybinds.LeftUp,
            WidgetSelectionType.Page => Keybinds.PageL,
            _ => throw new ClosedEnumsWhenException()
        };
    }
}