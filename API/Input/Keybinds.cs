using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace API.Input;

public static class Keybinds
{
    /// <summary>
    /// Number of <c>Keybinds</c> with standard check behavior
    /// </summary>
    public const int StdKeybindCount = (int) KeybindId.LastBeforeMerged;

    /// <summary>
    /// Confirm/Yes. Bottom face button
    /// </summary>
    public static readonly Keybind Confirm = new("KeyConfirm", KeybindId.Confirm, Keys.Z, Buttons.A);

    /// <summary>
    /// Back/Cancel/No. Right face button
    /// </summary>
    public static readonly Keybind Back = new("KeyBack", KeybindId.Back, Keys.X, Buttons.B);

    /// <summary>
    /// Open menu/full log. Top face button
    /// </summary>
    public static readonly Keybind Menu1 = new("KeyMenu1", KeybindId.Menu1, Keys.C, Buttons.Y);

    /// <summary>
    /// Open map/inspect. Left face button
    /// </summary>
    public static readonly Keybind Menu2 = new("KeyMenu2", KeybindId.Menu2, Keys.V, Buttons.X);

    public static readonly Keybind PageL = new("KeyPageL", KeybindId.PageL, Keys.S, Buttons.LeftShoulder);
    public static readonly Keybind PageR = new("KeyPageR", KeybindId.PageR, Keys.D, Buttons.RightShoulder);

    public static readonly Keybind Left = new("KeyLeft", KeybindId.Left, Keys.Left, Buttons.DPadLeft);
    public static readonly Keybind Right = new("KeyRight", KeybindId.Right, Keys.Right, Buttons.DPadRight);
    public static readonly Keybind Up = new("KeyUp", KeybindId.Up, Keys.Up, Buttons.DPadUp);
    public static readonly Keybind Down = new("KeyDown", KeybindId.Down, Keys.Down, Buttons.DPadDown);

    /// <summary>
    /// Used for various hotkeys, including doubling held input repeat speed
    /// </summary>
    // todo better names for these
    public static readonly Keybind Hotkey1 = new("KeyHotkey1", KeybindId.Hotkey1, Keys.LeftShift, Buttons.LeftTrigger);

    /// <summary>
    /// Used for various hotkeys, including jump to start of menu
    /// </summary>
    public static readonly Keybind Hotkey2 = new("KeyHotkey2", KeybindId.Hotkey2, Keys.LeftControl, Buttons.RightTrigger);

    /// <summary>
    /// Keybinds that don't call other keybinds
    /// </summary>
    public static readonly List<Keybind> NonMergedKeybinds =
        [Confirm, Back, Menu1, Menu2, PageL, PageR, Left, Right, Up, Down, Hotkey1, Hotkey2];

    // Merged

    // Acceptable for InputLib.Check, has no glyph (defers to left/right):
    public static readonly Keybind LeftUp = new("", KeybindId.LeftUp, Keys.None, Buttons.None);
    public static readonly Keybind RightDown = new("", KeybindId.RightDown, Keys.None, Buttons.None);

    // Not acceptable, has glyph:
    public static readonly Keybind LeftRight = new("", KeybindId.LeftRight, Keys.None, Buttons.None);
    public static readonly Keybind UpDown = new("", KeybindId.UpDown, Keys.None, Buttons.None);
    public static readonly Keybind LeftRightUpDown = new("", KeybindId.LeftRightUpDown, Keys.None, Buttons.None);
}
