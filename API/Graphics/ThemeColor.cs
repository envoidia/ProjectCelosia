using API.Extensions;
using API.Util;

namespace API.Graphics;

/// <summary>
/// For when you need to store a color that could change with the <c>Theme</c>.
/// Use <c>Theme.Get()</c> to convert to a <c>ColorCode</c> and <c>ThemeColor.Str</c> to convert to a string
/// </summary>
public enum ThemeColor
{
    White,
    Gray,
    Black,
    TransBlack,

    Fg,
    Bg,
    Accent,

    Pos,
    Neg,
    Imp,
    Ally,
    Opp,
    Turn,
    Hp,
    Sp,
    Shield,
    Bloom,
    Buff,
    Skill,
    Element,
    Passive,
    Stat,
    Cooldown,

    SpBack,
    Overheal,
    StatBarLayer4,
    StatBarLayer5,

    Atk,
    Def,
    Fth,
    Agi,

    Vis,
    Ignis,
    Glacies,
    Fulgur,
    Ventus,
    Terra,
    Lux,
    Malum
}

public static class ThemeColorExtensions
{
    extension(ThemeColor @this)
    {
        public string Str
        {
            get
            {
                return @this switch
                {
                    ThemeColor.White => "/c[white]",
                    ThemeColor.Gray => "/c[gray]",
                    ThemeColor.Black => "/c[black]",
                    ThemeColor.TransBlack => "/c[transBlack]",
                    ThemeColor.Fg => "/c[fg]",
                    ThemeColor.Bg => "/c[bg]",
                    ThemeColor.Accent => "/c[accent]",
                    ThemeColor.Pos => "/c[pos]",
                    ThemeColor.Neg => "/c[neg]",
                    ThemeColor.Imp => "/c[imp]",
                    ThemeColor.Ally => "/c[ally]",
                    ThemeColor.Opp => "/c[opp]",
                    ThemeColor.Turn => "/c[turn]",
                    ThemeColor.Hp => "/c[hp]",
                    ThemeColor.Sp => "/c[sp]",
                    ThemeColor.Shield => "/c[shield]",
                    ThemeColor.Bloom => "/c[bloom]",
                    ThemeColor.Buff => "/c[buff]",
                    ThemeColor.Skill => "/c[skill]",
                    ThemeColor.Element => "/c[element]",
                    ThemeColor.Passive => "/c[passive]",
                    ThemeColor.Stat => "/c[stat]",
                    ThemeColor.Cooldown => "/c[cooldown]",
                    ThemeColor.SpBack => "/c[spBack]",
                    ThemeColor.Overheal => "/c[overheal]",
                    ThemeColor.StatBarLayer4 => "/c[statBarLayer4]",
                    ThemeColor.StatBarLayer5 => "/c[statBarLayer5]",
                    ThemeColor.Atk => "/c[atk]",
                    ThemeColor.Def => "/c[def]",
                    ThemeColor.Fth => "/c[fth]",
                    ThemeColor.Agi => "/c[agi]",
                    ThemeColor.Vis => "/c[vis]",
                    ThemeColor.Ignis => "/c[ignis]",
                    ThemeColor.Glacies => "/c[glacies]",
                    ThemeColor.Fulgur => "/c[fulgur]",
                    ThemeColor.Ventus => "/c[ventus]",
                    ThemeColor.Terra => "/c[terra]",
                    ThemeColor.Lux => "/c[lux]",
                    ThemeColor.Malum => "/c[malum]",
                    _ => throw new ClosedEnumsWhenException()
                };
            }
        }
    }
}
