using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// A color code for formatting strings
/// </summary>
public readonly record struct ColorCode(Color Color) {
    public ColorCode(byte r, byte g, byte b) : this(new Color(r, g, b)) { }

    public static implicit operator string(ColorCode c) =>
        $"/c[#{c.Color.R:x2}{c.Color.G:x2}{c.Color.B:x2}]";

    public override string ToString() => this;

    public static readonly ColorCode ElectricBlue = new(74, 176, 231);

    /// <summary>
    /// Positive numbers
    /// </summary>
    public static readonly ColorCode Pos = new(Colors.Pos);

    /// <summary>
    /// Negative numbers
    /// </summary>
    public static readonly ColorCode Neg = new(Colors.Neg);

    /// <summary>
    /// General numbers (turns, stacks)
    /// </summary>
    public static readonly ColorCode Num = new(Colors.Num);

    public static readonly ColorCode White = new(Color.White);
    public static readonly ColorCode Black = new(Color.Black);
    public static readonly ColorCode Ally = new(131, 170, 240); // todo not readable enough
    public static readonly ColorCode Opp = new(255, 116, 116);
    public static readonly ColorCode Turn = new(160, 52, 255);
    public static readonly ColorCode Hp = new(Colors.Hp);
    public static readonly ColorCode Sp = new(187, 0, 255);
    public static readonly ColorCode Shield = new(Colors.Shield);
    public static readonly ColorCode Bloom = new(Color.Fuchsia);
    public static readonly ColorCode Buff = new(198, 161, 255);
    public static readonly ColorCode Skill = new(149, 201, 255);
    public static readonly ColorCode Element = new(Skill.Color);
    public static readonly ColorCode Passive = new(198, 161, 255);
    public static readonly ColorCode Stat = new(Colors.Stat);
    public static readonly ColorCode Cooldown = new(24, 152, 255);
    public static readonly ColorCode Lux = new(255, 251, 183);
}