using API.Extensions;
using API.Util;

namespace API.Graphics;

/// <summary>
/// For when you need to store a color that could change with the <c>Theme</c>.
/// Use <c>Theme.Get</c> to convert to a <c>Color</c> and <c>ThemeColor.Str</c> to convert to a string
/// </summary>
public enum ThemeColor
{
    /// <inheritdoc cref="Theme.Fg"/>
    Fg,
    /// <inheritdoc cref="Theme.Midtone"/>
    Midtone,
    /// <inheritdoc cref="Theme.Bg"/>
    Bg,
    /// <inheritdoc cref="Theme.BgSecondary"/>
    BgSecondary,
    /// <inheritdoc cref="Theme.BgTrans"/>
    BgTrans,

    /// <inheritdoc cref="Theme.Accent"/>
    Accent,
    /// <inheritdoc cref="Theme.AccentDeemphasized"/>
    AccentDeemphasized,

    /// <inheritdoc cref="Theme.Positive"/>
    Positive,
    /// <inheritdoc cref="Theme.Negative"/>
    Negative,
    /// <inheritdoc cref="Theme.Emphasis"/>
    Emphasis,

    /// <inheritdoc cref="Theme.Ally"/>
    Ally,
    /// <inheritdoc cref="Theme.Opponent"/>
    Opponent,
    /// <inheritdoc cref="Theme.Turn"/>
    Turn,
    /// <inheritdoc cref="Theme.Hp"/>
    Hp,
    /// <inheritdoc cref="Theme.Sp"/>
    Sp,
    /// <inheritdoc cref="Theme.Shield"/>
    Shield,
    /// <inheritdoc cref="Theme.Bloom"/>
    Bloom,
    /// <inheritdoc cref="Theme.Buff"/>
    Buff,
    /// <inheritdoc cref="Theme.Skill"/>
    Skill,
    /// <inheritdoc cref="Theme.Element"/>
    Element,
    /// <inheritdoc cref="Theme.Passive"/>
    Passive,
    /// <inheritdoc cref="Theme.Stat"/>
    Stat,
    /// <inheritdoc cref="Theme.Reticle"/>
    Reticle,
    /// <inheritdoc cref="Theme.Cooldown"/>
    Cooldown,

    /// <inheritdoc cref="Theme.SpBack"/>
    SpBack,
    /// <inheritdoc cref="Theme.Overheal"/>
    Overheal,
    /// <inheritdoc cref="Theme.StatBarLayer4"/>
    StatBarLayer4,
    /// <inheritdoc cref="Theme.StatBarLayer5"/>
    StatBarLayer5,

    /// <inheritdoc cref="Theme.Atk"/>
    Atk,
    /// <inheritdoc cref="Theme.Def"/>
    Def,
    /// <inheritdoc cref="Theme.Fth"/>
    Fth,
    /// <inheritdoc cref="Theme.Agi"/>
    Agi,

    /// <inheritdoc cref="Theme.Vis"/>
    Vis,
    /// <inheritdoc cref="Theme.Ignis"/>
    Ignis,
    /// <inheritdoc cref="Theme.Glacies"/>
    Glacies,
    /// <inheritdoc cref="Theme.Fulgur"/>
    Fulgur,
    /// <inheritdoc cref="Theme.Ventus"/>
    Ventus,
    /// <inheritdoc cref="Theme.Terra"/>
    Terra,
    /// <inheritdoc cref="Theme.Lux"/>
    Lux,
    /// <inheritdoc cref="Theme.Malum"/>
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
                return $"/c[{@this.ToString().ToLower()}]";
            }
        }
    }
}
