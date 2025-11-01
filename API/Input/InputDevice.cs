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
            InputDevice.Keyboard => Keybinds.Left.Key.GetGlyph() + Keybinds.Right.Key.GetGlyph(),
            InputDevice.NintendoController => "/i[NDX]",
            InputDevice.PlaystationController => "/i[PDX]",
            InputDevice.XboxController => "/i[XDX]"
        };

        public string GetGlyphUpDown => inputDevice switch {
            InputDevice.Keyboard => Keybinds.Up.Key.GetGlyph() + Keybinds.Down.Key.GetGlyph(),
            InputDevice.NintendoController => "/i[NDY]",
            InputDevice.PlaystationController => "/i[PDY]",
            InputDevice.XboxController => "/i[XDY]"
        };

        public string GetGlyphLeftRightUpDown => inputDevice switch {
            InputDevice.Keyboard => Keybinds.Left.Key.GetGlyph() + Keybinds.Right.Key.GetGlyph() +
                                    Keybinds.Up.Key.GetGlyph() + Keybinds.Down.Key.GetGlyph(),
            InputDevice.NintendoController => "/i[ND]",
            InputDevice.PlaystationController => "/i[PD]",
            InputDevice.XboxController => "/i[XD]"
        };
    }
}