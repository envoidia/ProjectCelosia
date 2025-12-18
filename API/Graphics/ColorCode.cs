using System.Collections.Generic;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// A color code for formatting strings
/// </summary>
public readonly record struct ColorCode {
    public static ColorCode White => new(Colors.White);
    public static ColorCode Black => new(Colors.Black);

    /// <inheritdoc cref="Colors.Pos" />
    public static ColorCode Pos => new(Colors.Pos);

    /// <inheritdoc cref="Colors.Neg" />
    public static ColorCode Neg => new(Colors.Neg);

    /// <inheritdoc cref="Colors.Num" />
    public static ColorCode Num => new(Colors.Num);

    public static ColorCode Ally => new(Colors.Ally);
    public static ColorCode Opp => new(Colors.Opp);
    public static ColorCode Turn => new(Colors.Turn);
    public static ColorCode Hp => new(Colors.Hp);
    public static ColorCode Sp => new(Colors.Sp);
    public static ColorCode Shield => new(Colors.Shield);
    public static ColorCode Bloom => new(Colors.Bloom);
    public static ColorCode Buff => new(Colors.Buff);
    public static ColorCode Skill => new(Colors.Skill);
    public static ColorCode Element => new(Colors.Element);
    public static ColorCode Passive => new(Colors.Passive);
    public static ColorCode Stat => new(Colors.Stat);
    public static ColorCode Cooldown => new(Colors.Cooldown);

    private readonly Color _c;

    /// <summary>
    /// Add custom color aliases
    /// </summary>
    static ColorCode() {
        Dictionary<string, Color> colorMap = new() {
            ["white"] = Colors.White,
            ["black"] = Colors.Black,
            ["pos"] = Colors.Pos,
            ["neg"] = Colors.Neg,
            ["num"] = Colors.Num,
            ["ally"] = Colors.Ally,
            ["opp"] = Colors.Opp,
            ["turn"] = Colors.Turn,
            ["hp"] = Colors.Hp,
            ["sp"] = Colors.Sp,
            ["shield"] = Colors.Shield,
            ["bloom"] = Colors.Bloom,
            ["buff"] = Colors.Buff,
            ["skill"] = Colors.Skill,
            ["element"] = Colors.Element,
            ["passive"] = Colors.Passive,
            ["stat"] = Colors.Stat,
            ["cooldown"] = Colors.Cooldown
        };

        foreach (KeyValuePair<string, Color> kvp in colorMap) {
            ColorStorage.Colors[kvp.Key] = new() { Color = kvp.Value };
        }
    }

    public ColorCode(Color c) => this._c = c;

    public static implicit operator string(ColorCode c) => $"/c[#{c._c.R:x2}{c._c.G:x2}{c._c.B:x2}]";

    public override string ToString() => this;
}