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

    Hotkey1,
    Hotkey2,

    DebugInfo,
    DebugConsole,
    DebugOverlay,

    /// <summary>
    /// Marker. Always add non-merged keybinds above this
    /// </summary>
    LastBeforeMerged,

    // Merged (must be last)
    LeftUp,
    RightDown,

    LeftRight,
    UpDown,
    LeftRightUpDown
}
