using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;
using Microsoft.Xna.Framework.Input;

namespace API.Input;

public sealed class Keybind(string keyName, KeybindId id, Keys key, Buttons button) : IDescribable
{
    public readonly KeybindId Id = id;
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