using System;

namespace API.Input;

public enum InputDevice {
    Keyboard,
    NintendoController,
    PlaystationController,
    XboxController
}

public static class InputDeviceExtensions {
    extension(InputDevice inputDevice) {
        public string GetGlyphLeftRight => inputDevice switch {
            InputDevice.Keyboard => Keybind.Left.Key.GetGlyph() + Keybind.Right.Key.GetGlyph(),
            InputDevice.NintendoController => "/i[NDX]",
            InputDevice.PlaystationController => "/i[PDX]",
            InputDevice.XboxController => "/i[XDX]",
            _ => throw new ArgumentOutOfRangeException(nameof(inputDevice))
        };

        public string GetGlyphUpDown => inputDevice switch {
            InputDevice.Keyboard => Keybind.Up.Key.GetGlyph() + Keybind.Down.Key.GetGlyph(),
            InputDevice.NintendoController => "/i[NDY]",
            InputDevice.PlaystationController => "/i[PDY]",
            InputDevice.XboxController => "/i[XDY]",
            _ => throw new ArgumentOutOfRangeException(nameof(inputDevice))
        };

        public string GetGlyphLeftRightUpDown => inputDevice switch {
            InputDevice.Keyboard => Keybind.Left.Key.GetGlyph() + Keybind.Right.Key.GetGlyph() +
                                    Keybind.Up.Key.GetGlyph() + Keybind.Down.Key.GetGlyph(),
            InputDevice.NintendoController => "/i[ND]",
            InputDevice.PlaystationController => "/i[PD]",
            InputDevice.XboxController => "/i[XD]",
            _ => throw new ArgumentOutOfRangeException(nameof(inputDevice))
        };
    }
}