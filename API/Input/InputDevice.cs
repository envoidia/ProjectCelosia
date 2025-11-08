using System;
using Microsoft.Xna.Framework.Input;

namespace API.Input;

public enum InputDevice {
    Keyboard,
    NintendoController,
    PlaystationController,
    XboxController
}

public static class InputDeviceExtensions {
    // todo support controller remapping
    extension(InputDevice inputDevice) {
        private string GetGlyphIdentifier() => inputDevice switch {
            InputDevice.NintendoController => "N",
            InputDevice.PlaystationController => "P",
            InputDevice.XboxController => "X"
        };

        public string FormatSingleGlyph(string glyphType) =>
            $"/i[{inputDevice.GetGlyphIdentifier()}{glyphType}]";

        public string GetMergedGlyph(KeybindId id) {
            switch (id) {
                case KeybindId.LeftRight:
                    if (inputDevice == InputDevice.Keyboard) {
                        return Keybinds.Left.Key.GetGlyph() + Keybinds.Right.Key.GetGlyph();
                    }

                    return (Keybinds.Left.Button.GetGlyph(inputDevice) + "/" +
                            Keybinds.Right.Button.GetGlyph(inputDevice))
                        .Replace($"/i[{inputDevice.GetGlyphIdentifier()}DL]//i[{inputDevice.GetGlyphIdentifier()}DR]",
                            $"/i[{inputDevice.GetGlyphIdentifier()}DX]");
                case KeybindId.UpDown:
                    if (inputDevice == InputDevice.Keyboard) {
                        return Keybinds.Up.Key.GetGlyph() + Keybinds.Down.Key.GetGlyph();
                    }

                    return (Keybinds.Up.Button.GetGlyph(inputDevice) + "/" +
                            Keybinds.Down.Button.GetGlyph(inputDevice))
                        .Replace($"/i[{inputDevice.GetGlyphIdentifier()}DU]//i[{inputDevice.GetGlyphIdentifier()}DD]",
                            $"/i[{inputDevice.GetGlyphIdentifier()}DY]");
                case KeybindId.LeftRightUpDown:
                    if (inputDevice == InputDevice.Keyboard) {
                        return Keybinds.Left.Key.GetGlyph() + Keybinds.Right.Key.GetGlyph() +
                               Keybinds.Up.Key.GetGlyph() + Keybinds.Down.Key.GetGlyph();
                    }

                    return (Keybinds.Left.Button.GetGlyph(inputDevice) + "/" +
                            Keybinds.Right.Button.GetGlyph(inputDevice) + "/" +
                            Keybinds.Up.Button.GetGlyph(inputDevice) + "/" +
                            Keybinds.Down.Button.GetGlyph(inputDevice))
                        .Replace(
                            $"/i[{inputDevice.GetGlyphIdentifier()}DL]//i[{inputDevice.GetGlyphIdentifier()}DR]//i[{inputDevice.GetGlyphIdentifier()}DU]//i[{inputDevice.GetGlyphIdentifier()}DD]",
                            $"/i[{inputDevice.GetGlyphIdentifier()}D]");
                default:
                    throw new ArgumentOutOfRangeException(nameof(id), id, Lang.ErrGetMergedGlyphKeybindId);
            }
        }
    }
}