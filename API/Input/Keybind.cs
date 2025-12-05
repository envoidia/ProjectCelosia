using Microsoft.Xna.Framework.Input;

namespace API.Input;

// todo if closed enums get added, use them in various places
public enum KeybindId {
    Confirm,
    Back,
    Menu,
    Map,
    PageL1,
    PageR1,
    PageL2,
    PageR2,
    Left,
    Right,
    Up,
    Down,
    ScrollFaster,

    // Debug
    DebugInfo,
    DebugHelp,
    DebugDumpMods,
    DebugDumpLog,

    /// <summary>
    /// Marker. Always add non-merged keybinds above this
    /// </summary>
    LastBeforeMerged,

    // Merged (must be last)
    LeftRight,
    UpDown,
    LeftRightUpDown
}

public sealed class Keybind(string keyName, KeybindId id, Keys key, Buttons button) {
    public string KeyName => keyName;
    public KeybindId Id => id;
    public Keys Key { get; set; } = key;
    public Buttons Button { get; set; } = button;

    public string GetCurrentGlyph() => this.Id switch {
        KeybindId.LeftRight or KeybindId.UpDown or KeybindId.LeftRightUpDown =>
            Core.Input.LastInputSource.GetMergedGlyph(this.Id),
        _ => this._GetGlyph()
    };

    private string _GetGlyph() => Core.Input.LastInputSource == InputDevice.Keyboard
        ? this.Key.GetGlyph()
        : this.Button.GetGlyph(Core.Input.LastInputSource);
}

public static class Keybinds {
    public static int KeybindCount => (int) KeybindId.LastBeforeMerged;

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
    public static readonly Keybind Menu = new("KeyMenu", KeybindId.Menu, Keys.C, Buttons.Y);

    /// <summary>
    /// Open map/inspect. Left face button
    /// </summary>
    public static readonly Keybind Map = new("KeyMap", KeybindId.Map, Keys.V, Buttons.X);

    public static readonly Keybind PageL1 = new("KeyPageL1", KeybindId.PageL1, Keys.F, Buttons.LeftShoulder);
    public static readonly Keybind PageR1 = new("KeyPageR1", KeybindId.PageR1, Keys.G, Buttons.RightShoulder);
    public static readonly Keybind PageL2 = new("KeyPageL2", KeybindId.PageL2, Keys.S, Buttons.LeftTrigger);
    public static readonly Keybind PageR2 = new("KeyPageR2", KeybindId.PageR2, Keys.D, Buttons.RightTrigger);

    public static readonly Keybind Left = new("KeyLeft", KeybindId.Left, Keys.Left, Buttons.DPadLeft);
    public static readonly Keybind Right = new("KeyRight", KeybindId.Right, Keys.Right, Buttons.DPadRight);
    public static readonly Keybind Up = new("KeyUp", KeybindId.Up, Keys.Up, Buttons.DPadUp);
    public static readonly Keybind Down = new("KeyDown", KeybindId.Down, Keys.Down, Buttons.DPadDown);

    /// <summary>
    /// Hold to double input repeat speed when holding (todo should it be more than double)
    /// </summary>
    public static readonly Keybind ScrollFaster = new("KeyScroll", KeybindId.ScrollFaster, Keys.LeftShift, Buttons.Start);

    // Debug
    // Ensure names are never displayed
    // todo remake icons with more buttons (like stick clicks)
    public static readonly Keybind DebugInfo = new("", KeybindId.DebugInfo, Keys.F1, Buttons.Back);
    public static readonly Keybind DebugHelp = new("", KeybindId.DebugHelp, Keys.F2, Buttons.None);
    public static readonly Keybind DebugDumpMods = new("", KeybindId.DebugDumpMods, Keys.F3, Buttons.None);
    public static readonly Keybind DebugDumpLog = new("", KeybindId.DebugDumpLog, Keys.F4, Buttons.None);

    // Merged
    public static readonly Keybind LeftRight = new("", KeybindId.LeftRight, Keys.None, Buttons.None);
    public static readonly Keybind UpDown = new("", KeybindId.UpDown, Keys.None, Buttons.None);
    public static readonly Keybind LeftRightUpDown = new("", KeybindId.LeftRightUpDown, Keys.None, Buttons.None);
}