using System;
using Microsoft.Xna.Framework.Input;

namespace API.Input;

public static class KeysExtensions
{
    extension(Keys @this)
    {
        public string GetGlyph()
        {
            return FormatGlyph(@this switch
            {
                Keys.Back => "Backspace",
                Keys.Enter => "Enter",
                Keys.Escape => "Esc",
                Keys.Space => "Space",
                Keys.PageUp => "PU",
                Keys.PageDown => "PD",
                Keys.End => "End",
                Keys.Home => "Home",
                Keys.Left => "Left",
                Keys.Up => "Up",
                Keys.Right => "Right",
                Keys.Down => "Down",
                Keys.Insert => "Ins",
                Keys.A => "A",
                Keys.B => "B",
                Keys.C => "C",
                Keys.D => "D",
                Keys.E => "E",
                Keys.F => "F",
                Keys.G => "G",
                Keys.H => "H",
                Keys.I => "I",
                Keys.J => "J",
                Keys.K => "K",
                Keys.L => "L",
                Keys.M => "M",
                Keys.N => "N",
                Keys.O => "O",
                Keys.P => "P",
                Keys.Q => "Q",
                Keys.R => "R",
                Keys.S => "S",
                Keys.T => "T",
                Keys.U => "U",
                Keys.V => "V",
                Keys.W => "W",
                Keys.X => "X",
                Keys.Y => "Y",
                Keys.Z => "Z",
                Keys.F1 => "F1",
                Keys.F2 => "F2",
                Keys.F3 => "F3",
                Keys.F4 => "F4",
                Keys.F5 => "F5",
                Keys.F6 => "F6",
                Keys.F7 => "F7",
                Keys.F8 => "F8",
                Keys.F9 => "F9",
                Keys.F10 => "F10",
                Keys.F11 => "F11",
                Keys.F12 => "F12",
                Keys.LeftShift => "Shift",
                Keys.LeftControl => "Ctrl",
                Keys.LeftAlt => "Alt",
                Keys.OemSemicolon => ";",
                Keys.OemPlus => "+",
                Keys.OemComma => ",",
                Keys.OemMinus => "-",
                Keys.OemPeriod => ".",
                Keys.OemQuestion => "QM",
                Keys.OemTilde => "~",
                Keys.OemOpenBrackets => "[",
                Keys.OemCloseBrackets => "]",
                Keys.OemQuotes => "Quot",
                _ => throw new ArgumentOutOfRangeException(nameof(@this), @this, "") // todo
            });
        }

        // todo handle caps lock
        /// <returns>The pressed key converted to a char based on if shift is held, or null</returns>
        public char? Type()
        {
            if (InputLib.IsKeyPressed(Keys.LeftShift) || InputLib.IsKeyPressed(Keys.RightShift))
            {
                return @this switch
                {
                    Keys.A => 'A',
                    Keys.B => 'B',
                    Keys.C => 'C',
                    Keys.D => 'D',
                    Keys.E => 'E',
                    Keys.F => 'F',
                    Keys.G => 'G',
                    Keys.H => 'H',
                    Keys.I => 'I',
                    Keys.J => 'J',
                    Keys.K => 'K',
                    Keys.L => 'L',
                    Keys.M => 'M',
                    Keys.N => 'N',
                    Keys.O => 'O',
                    Keys.P => 'P',
                    Keys.Q => 'Q',
                    Keys.R => 'R',
                    Keys.S => 'S',
                    Keys.T => 'T',
                    Keys.U => 'U',
                    Keys.V => 'V',
                    Keys.W => 'W',
                    Keys.X => 'X',
                    Keys.Y => 'Y',
                    Keys.Z => 'Z',
                    Keys.D0 => ')',
                    Keys.D1 => '!',
                    Keys.D2 => '@',
                    Keys.D3 => '#',
                    Keys.D4 => '$',
                    Keys.D5 => '%',
                    Keys.D6 => '^',
                    Keys.D7 => '&',
                    Keys.D8 => '*',
                    Keys.D9 => '(',
                    Keys.OemSemicolon => ':',
                    Keys.OemPlus => '+',
                    Keys.OemComma => '<',
                    Keys.OemMinus => '_',
                    Keys.OemPeriod => '>',
                    Keys.OemQuestion => '?',
                    Keys.OemTilde => '~',
                    Keys.OemOpenBrackets => '{',
                    Keys.OemCloseBrackets => '}',
                    Keys.OemQuotes => '"',
                    Keys.OemBackslash => '|',
                    Keys.Space => ' ',
                    _ => null
                };
            }

            return @this switch
            {
                Keys.A => 'a',
                Keys.B => 'b',
                Keys.C => 'c',
                Keys.D => 'd',
                Keys.E => 'e',
                Keys.F => 'f',
                Keys.G => 'g',
                Keys.H => 'h',
                Keys.I => 'i',
                Keys.J => 'j',
                Keys.K => 'k',
                Keys.L => 'l',
                Keys.M => 'm',
                Keys.N => 'n',
                Keys.O => 'o',
                Keys.P => 'p',
                Keys.Q => 'q',
                Keys.R => 'r',
                Keys.S => 's',
                Keys.T => 't',
                Keys.U => 'u',
                Keys.V => 'v',
                Keys.W => 'w',
                Keys.X => 'x',
                Keys.Y => 'y',
                Keys.Z => 'z',
                Keys.D0 => '0',
                Keys.D1 => '1',
                Keys.D2 => '2',
                Keys.D3 => '3',
                Keys.D4 => '4',
                Keys.D5 => '5',
                Keys.D6 => '6',
                Keys.D7 => '7',
                Keys.D8 => '8',
                Keys.D9 => '9',
                Keys.OemSemicolon => ';',
                Keys.OemPlus => '=',
                Keys.OemComma => ',',
                Keys.OemMinus => '-',
                Keys.OemPeriod => '.',
                Keys.OemQuestion => '/',
                Keys.OemTilde => '`',
                Keys.OemOpenBrackets => '[',
                Keys.OemCloseBrackets => ']',
                Keys.OemQuotes => '\'',
                Keys.OemBackslash => '\\',
                Keys.Space => ' ',
                _ => null
            };
        }
    }

    public static string FormatGlyph(string name)
    {
        return $"/i[K{name}]";
    }
}