using API.Input;
using API.Util;

namespace API.Menu;

/// <summary>
/// The directions that an <c>IWidget</c> would like to use for input
/// </summary>
public enum SelectionType
{
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
    Page,

    /// <summary>
    /// Almost all keys on KB, L/R/U/D on controller
    /// </summary>
    TextInput
}

public static class SelectionTypeExtensions
{
    extension(SelectionType @this)
    {
        /// <returns><c>Keybind</c> to increase this' index by 1</returns>
        public Keybind? GetInc()
        {
            return @this switch
            {
                SelectionType.Horiz => Keybinds.Right,
                SelectionType.Vert => Keybinds.Down,
                SelectionType.HorizVert => Keybinds.RightDown,
                SelectionType.Page => Keybinds.PageR,
                SelectionType.None or SelectionType.TextInput => null,
                _ => throw new ClosedEnumsWhenException()
            };
        }

        /// <returns><c>Keybind</c> to decrease this' index by 1</returns>
        public Keybind? GetDec()
        {
            return @this switch
            {
                SelectionType.Horiz => Keybinds.Left,
                SelectionType.Vert => Keybinds.Up,
                SelectionType.HorizVert => Keybinds.LeftUp,
                SelectionType.Page => Keybinds.PageL,
                SelectionType.None or SelectionType.TextInput => null,
                _ => throw new ClosedEnumsWhenException()
            };
        }
    }
}