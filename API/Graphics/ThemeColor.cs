using System;
using API.Util;

namespace API.Graphics;

/// <summary>
/// For when you need to store a color that could change with the <c>Theme</c>.
/// Use <c>Theme.Get()</c> to convert to a <c>ColorCode</c> and <c>ThemeColor.Str()</c> to convert to a string
/// </summary>
public enum ThemeColor {
    White,
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

public static class ThemeColorExtensions {
    extension(ThemeColor @this) {
        public string Str() => @this switch {
            ThemeColor.White => _Wrap("white"),
            ThemeColor.Black => _Wrap("black"),
            ThemeColor.TransBlack => _Wrap("transblack"),

            ThemeColor.Fg => _Wrap("fg"),
            ThemeColor.Bg => _Wrap("bg"),
            ThemeColor.Accent => _Wrap("accent"),

            ThemeColor.Pos => _Wrap("pos"),
            ThemeColor.Neg => _Wrap("neg"),
            ThemeColor.Imp => _Wrap("imp"),
            ThemeColor.Ally => _Wrap("ally"),
            ThemeColor.Opp => _Wrap("opp"),
            ThemeColor.Turn => _Wrap("turn"),
            ThemeColor.Hp => _Wrap("hp"),
            ThemeColor.Sp => _Wrap("sp"),
            ThemeColor.Shield => _Wrap("shield"),
            ThemeColor.Bloom => _Wrap("bloom"),
            ThemeColor.Buff => _Wrap("buff"),
            ThemeColor.Skill => _Wrap("skill"),
            ThemeColor.Element => _Wrap("element"),
            ThemeColor.Passive => _Wrap("passive"),
            ThemeColor.Stat => _Wrap("stat"),
            ThemeColor.Cooldown => _Wrap("cooldown"),

            ThemeColor.SpBack => _Wrap("spback"),
            ThemeColor.Overheal => _Wrap("overheal"),
            ThemeColor.StatBarLayer4 => _Wrap("statbarlayer4"),
            ThemeColor.StatBarLayer5 => _Wrap("statbarlayer5"),

            ThemeColor.Atk => _Wrap("atk"),
            ThemeColor.Def => _Wrap("def"),
            ThemeColor.Fth => _Wrap("fth"),
            ThemeColor.Agi => _Wrap("agi"),

            ThemeColor.Vis => _Wrap("vis"),
            ThemeColor.Ignis => _Wrap("ignis"),
            ThemeColor.Glacies => _Wrap("glacies"),
            ThemeColor.Fulgur => _Wrap("fulgur"),
            ThemeColor.Ventus => _Wrap("ventus"),
            ThemeColor.Terra => _Wrap("terra"),
            ThemeColor.Lux => _Wrap("lux"),
            ThemeColor.Malum => _Wrap("malum"),

            _ => throw new ClosedEnumsWhenException()
        };
    }

    private static string _Wrap(string str) => $"/c[{str}]";
}
