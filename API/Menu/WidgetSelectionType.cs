using API.Input;
using API.Util;

namespace API.Menu;

/// <summary>
/// The directions that an <c>IWidget</c> would like to use for input
/// </summary>
public enum SelectionType {
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

// todo account for none
public static class SelectionTypeExtensions {
    extension(SelectionType @this) {
        public Keybind? GetInc() => @this switch {
            SelectionType.Horiz => Keybinds.Right,
            SelectionType.Vert => Keybinds.Down,
            SelectionType.HorizVert => Keybinds.RightDown,
            SelectionType.Page => Keybinds.PageR,
            SelectionType.None => null,
            _ => throw new ClosedEnumsWhenException()
        };

        public Keybind? GetDec() => @this switch {
            SelectionType.Horiz => Keybinds.Left,
            SelectionType.Vert => Keybinds.Up,
            SelectionType.HorizVert => Keybinds.LeftUp,
            SelectionType.Page => Keybinds.PageL,
            SelectionType.None => null,
            _ => throw new ClosedEnumsWhenException()
        };
    }
}