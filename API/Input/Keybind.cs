using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;
using Microsoft.Xna.Framework.Input;

namespace API.Input;

// todo if closed enums get added, use them in various places
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

public sealed class Keybind(string keyName, KeybindId id, Keys key, Buttons button) : IDescribable
{
    public KeybindId Id
    {
        get
        {
            return id;
        }
    }

    public Keys Key = key;
    public Buttons Button = button;

    public string KeyName { get; set; } = keyName;
    public string KeyDesc { get; set; } = $"{keyName}Desc";

    public string GetCurrentGlyph()
    {
        return this.Id switch
        {
            KeybindId.LeftRight or KeybindId.UpDown or KeybindId.LeftRightUpDown =>
                InputLib.LastInputSource.GetMergedGlyph(this.Id),
            KeybindId.LeftUp => _GetCurrentGlyph(Keybinds.Left.Key, Keybinds.Left.Button),
            KeybindId.RightDown => _GetCurrentGlyph(Keybinds.Right.Key, Keybinds.Right.Button),
            _ => _GetCurrentGlyph(this.Key, this.Button)
        };
    }

    private static string _GetCurrentGlyph(Keys key, Buttons button)
    {
        return InputLib.LastInputSource == InputDevice.Keyboard
            ? key.GetGlyph()
            : button.GetGlyph(InputLib.LastInputSource);
    }

    public string GetCurrentGlyphName()
    {
        return InputLib.LastInputSource == InputDevice.Keyboard
            ? this.Key.GetGlyphName()
            : this.Button.GetGlyphName();
    }

    public override string ToString()
    {
        return $"{base.ToString()}: {this.GetName()} -- {this.GetDesc()}";
    }

    public string GetName(ThemeColor color)
    {
        return color.Str + this.KeyName.GetLang();
    }

    public string GetName()
    {
        return this.GetName(ThemeColor.White);
    }

    public string GetDesc()
    {
        return this.KeyDesc.GetLang();
    }
}