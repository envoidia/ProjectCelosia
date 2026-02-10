namespace API.Input;

public enum KeybindId
{
    Confirm,
    Back,
    Menu1,
    Menu2,
    PageL,
    PageR,
    Left,
    Right,
    Up,
    Down,

    /// <summary>
    /// Marker. Always add non-merged, non-hotkey keybinds above this
    /// </summary>
    LastBeforeAbnormal,

    Hotkey1,
    Hotkey2,

    // Merged (must be last)
    LeftUp,
    RightDown,

    LeftRight,
    UpDown,
    LeftRightUpDown
}
