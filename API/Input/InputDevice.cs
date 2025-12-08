using System;

namespace API.Input;

public enum InputDevice {
    Keyboard,
    NintendoController,
    PlaystationController,
    XboxController
}

public static class InputDeviceExtensions {
    // todo support controller remapping
    extension(InputDevice @this) {
        private string GetGlyphIdentifier() => @this switch {
            InputDevice.NintendoController => "N",
            InputDevice.PlaystationController => "P",
            InputDevice.XboxController => "X"
        };

        public string FormatSingleGlyph(string glyphType) =>
            $"/i[{@this.GetGlyphIdentifier()}{glyphType}]";

        public string GetMergedGlyph(KeybindId id) {
            switch (id) {
                case KeybindId.LeftRight:
                    if (@this == InputDevice.Keyboard) {
                        return $"{Keybinds.Left.Key.GetGlyph()}//{Keybinds.Right.Key.GetGlyph()}";
                    }

                    return $"{Keybinds.Left.Button.GetGlyph(@this)}//{Keybinds.Right.Button.GetGlyph(@this)}"
                        .Replace($"/i[{@this.GetGlyphIdentifier()}DL]//i[{@this.GetGlyphIdentifier()}DR]",
                            $"/i[{@this.GetGlyphIdentifier()}DX]");
                case KeybindId.UpDown:
                    if (@this == InputDevice.Keyboard) {
                        return $"{Keybinds.Up.Key.GetGlyph()}//{Keybinds.Down.Key.GetGlyph()}";
                    }

                    return $"{Keybinds.Up.Button.GetGlyph(@this)}//{Keybinds.Down.Button.GetGlyph(@this)}"
                        .Replace($"/i[{@this.GetGlyphIdentifier()}DU]//i[{@this.GetGlyphIdentifier()}DD]",
                            $"/i[{@this.GetGlyphIdentifier()}DY]");
                case KeybindId.LeftRightUpDown:
                    if (@this == InputDevice.Keyboard) {
                        return $"{Keybinds.Left.Key.GetGlyph()}//{Keybinds.Right.Key.GetGlyph()}//{Keybinds.Up.Key.GetGlyph()}//{Keybinds.Down.Key.GetGlyph()}";
                    }

                    return $"{Keybinds.Left.Button.GetGlyph(@this)}//{Keybinds.Right.Button.GetGlyph(@this)}//{Keybinds.Up.Button.GetGlyph(@this)}//{Keybinds.Down.Button.GetGlyph(@this)}"
                        .Replace(
                            $"/i[{@this.GetGlyphIdentifier()}DL]//i[{@this.GetGlyphIdentifier()}DR]//i[{@this.GetGlyphIdentifier()}DU]//i[{@this.GetGlyphIdentifier()}DD]",
                            $"/i[{@this.GetGlyphIdentifier()}D]");
                default:
                    throw new ArgumentOutOfRangeException(nameof(id), id, Lang.ErrGetMergedGlyphKeybindId);
            }
        }
    }
}