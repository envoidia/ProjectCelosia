using System;
using Microsoft.Xna.Framework.Input;

namespace API.Input;

public static class ButtonsExtensions {
    extension(Buttons button) {
        public string GetGlyph(InputDevice inputDevice) => FormatSingleGlyph(button.GetSingleGlyph(), inputDevice);

        public string[] GetGlyphs() => FormatGlyphArray(button.GetSingleGlyph());

        public string GetSingleGlyph() => button switch {
            Buttons.DPadUp => "DU",
            Buttons.DPadDown => "DD",
            Buttons.DPadLeft => "DL",
            Buttons.DPadRight => "DR",
            Buttons.Start => "RM",
            Buttons.LeftShoulder => "LB",
            Buttons.RightShoulder => "RB",
            Buttons.A => "A",
            Buttons.B => "B",
            Buttons.X => "X",
            Buttons.Y => "Y",
            Buttons.RightTrigger => "RT",
            Buttons.LeftTrigger => "LT",
            _ => throw new ArgumentOutOfRangeException(nameof(button))
        };
    }

    public static string FormatSingleGlyph(string name, InputDevice inputDevice) => inputDevice switch {
        InputDevice.NintendoController => $"/i[N{name}]",
        InputDevice.PlaystationController => $"/i[P{name}]",
        InputDevice.XboxController => $"/i[X{name}]",
        _ => throw new ArgumentOutOfRangeException(nameof(inputDevice))
    };

    public static string[] FormatGlyphArray(string name) => [$"/i[N{name}]", $"/i[P{name}]", $"/i[X{name}]"];
}