using System.Collections.Generic;
using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;
using Microsoft.Xna.Framework.Input;

namespace API.Input;

// todo if closed enums get added, use them in various places
public enum KeybindId {
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
    DebugInfo,

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

public sealed class Keybind(GameMod? source, string keyName, KeybindId id, Keys key, Buttons button) : IDescribable {
    public KeybindId Id => id;
    public Keys Key { get; set; } = key;
    public Buttons Button { get; set; } = button;

    public GameMod? Source => source;
    public string KeyName { get; set; } = keyName;
    public string KeyDesc { get; set; } = $"{keyName}Desc";

    public string GetCurrentGlyph() => this.Id switch {
        KeybindId.LeftRight or KeybindId.UpDown or KeybindId.LeftRightUpDown =>
            InputLib.LastInputSource.GetMergedGlyph(this.Id),
        KeybindId.LeftUp => _GetCurrentGlyph(Keybinds.Left.Key, Keybinds.Left.Button),
        KeybindId.RightDown => _GetCurrentGlyph(Keybinds.Right.Key, Keybinds.Right.Button),
        _ => _GetCurrentGlyph(this.Key, this.Button)
    };

    private static string _GetCurrentGlyph(Keys key, Buttons button) {
        return InputLib.LastInputSource == InputDevice.Keyboard
            ? key.GetGlyph()
            : button.GetGlyph(InputLib.LastInputSource);
    }

    public string GetName(ThemeColor color, GameMod? mod = null) => color.Str() + this.KeyName.GetLang(mod ?? this.Source);
    public string GetName(GameMod? mod = null) => this.GetName(ThemeColor.White, mod ?? this.Source);
    public string GetDesc(GameMod? mod = null) => this.KeyDesc.GetLang(mod ?? this.Source);
}

public static class Keybinds {
    /// <summary>
    /// Number of <c>Keybinds</c> with standard check behavior
    /// </summary>
    public const int StdKeybindCount = (int) KeybindId.LastBeforeAbnormal;

    /// <summary>
    /// Confirm/Yes. Bottom face button
    /// </summary>
    public static readonly Keybind Confirm = new(null, "KeyConfirm", KeybindId.Confirm, Keys.Z, Buttons.A);

    /// <summary>
    /// Back/Cancel/No. Right face button
    /// </summary>
    public static readonly Keybind Back = new(null, "KeyBack", KeybindId.Back, Keys.X, Buttons.B);

    /// <summary>
    /// Open menu/full log. Top face button
    /// </summary>
    public static readonly Keybind Menu1 = new(null, "KeyMenu1", KeybindId.Menu1, Keys.C, Buttons.Y);

    /// <summary>
    /// Open map/inspect. Left face button
    /// </summary>
    public static readonly Keybind Menu2 = new(null, "KeyMenu2", KeybindId.Menu2, Keys.V, Buttons.X);

    public static readonly Keybind PageL = new(null, "KeyPageL", KeybindId.PageL, Keys.S, Buttons.LeftShoulder);
    public static readonly Keybind PageR = new(null, "KeyPageR", KeybindId.PageR, Keys.D, Buttons.RightShoulder);

    public static readonly Keybind Left = new(null, "KeyLeft", KeybindId.Left, Keys.Left, Buttons.DPadLeft);
    public static readonly Keybind Right = new(null, "KeyRight", KeybindId.Right, Keys.Right, Buttons.DPadRight);
    public static readonly Keybind Up = new(null, "KeyUp", KeybindId.Up, Keys.Up, Buttons.DPadUp);
    public static readonly Keybind Down = new(null, "KeyDown", KeybindId.Down, Keys.Down, Buttons.DPadDown);

    /// <summary>
    /// Used for various hotkeys, including doubling held input repeat speed. Ignores held time restrictions
    /// </summary>
    // todo should it be more than double
    public static readonly Keybind Hotkey1 = new(null, "KeyHotkey1", KeybindId.Hotkey1, Keys.LeftShift, Buttons.LeftTrigger);

    /// <summary>
    /// Used for various hotkeys, including jump to start/end. Ignores held time restrictions
    /// </summary>
    public static readonly Keybind Hotkey2 = new(null, "KeyHotkey2", KeybindId.Hotkey2, Keys.LeftControl, Buttons.RightTrigger);

    public static readonly Keybind DebugInfo = new(null, "KeyDebugInfo", KeybindId.DebugInfo, Keys.F1, Buttons.Back);

    /// <summary>
    /// Keybinds that don't call other keybinds
    /// </summary>
    public static readonly List<Keybind> UniqueKeybinds =
        [Confirm, Back, Menu1, Menu2, PageL, PageR, Left, Right, Up, Down, DebugInfo, Hotkey1, Hotkey2];

    // Merged
    // Acceptable for InputLib.Check(), has no glyph (defers to left/right):
    public static readonly Keybind LeftUp = new(null, "", KeybindId.LeftUp, Keys.None, Buttons.None);
    public static readonly Keybind RightDown = new(null, "", KeybindId.RightDown, Keys.None, Buttons.None);

    // Not acceptable, has glyph:
    public static readonly Keybind LeftRight = new(null, "", KeybindId.LeftRight, Keys.None, Buttons.None);
    public static readonly Keybind UpDown = new(null, "", KeybindId.UpDown, Keys.None, Buttons.None);
    public static readonly Keybind LeftRightUpDown = new(null, "", KeybindId.LeftRightUpDown, Keys.None, Buttons.None);
}