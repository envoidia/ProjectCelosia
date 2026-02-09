using System;
using Microsoft.Xna.Framework.Input;

namespace API.Input;

public static class ButtonsExtensions
{
    extension(Buttons @this)
    {
        public string GetGlyph(InputDevice inputDevice)
        {
            return inputDevice.FormatSingleGlyph(@this.GetGlyphName());
        }

        // public string[] GetGlyphs()
        // {
        //     return _FormatGlyphArray(@this._GetSingleGlyph());
        // }

        public string GetGlyphName()
        {
            return @this switch
            {
                Buttons.DPadUp => "DU",
                Buttons.DPadDown => "DD",
                Buttons.DPadLeft => "DL",
                Buttons.DPadRight => "DR",
                Buttons.A => "A",
                Buttons.B => "B",
                Buttons.X => "X",
                Buttons.Y => "Y",
                Buttons.LeftShoulder => "LB",
                Buttons.RightShoulder => "RB",
                Buttons.RightTrigger => "RT",
                Buttons.LeftTrigger => "LT",
                Buttons.Back => "LM",
                Buttons.Start => "RM",
                _ => throw new ArgumentOutOfRangeException(nameof(@this), @this, "Invalid button")
            };
        }
    }

    // private static string[] _FormatGlyphArray(string name)
    // {
    //     return [$"/i[N{name}]", $"/i[P{name}]", $"/i[X{name}]"];
    // }
}