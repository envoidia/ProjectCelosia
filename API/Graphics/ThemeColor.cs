using API.Extensions;

namespace API.Graphics;

/// <summary>
/// For when you need to store a color that could change with the <c>Theme</c>.
/// Use <c>Theme.Get()</c> to convert to a <c>ColorCode</c> and <c>ThemeColor.Str</c> to convert to a string
/// </summary>
public enum ThemeColor {
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

public static class ThemeColorExtensions {
    extension(ThemeColor @this) {
        public string Str => _Wrap(@this.ToString().FirstToLower());
    }

    private static string _Wrap(string str) => $"/c[{str}]";
}
