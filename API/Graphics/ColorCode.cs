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

    public static readonly ColorCode Red = new(Color.Red);
    public static readonly ColorCode Orange = new(Color.Orange);
    public static readonly ColorCode Yellow = new(Color.Yellow);
    public static readonly ColorCode Lime = new(Color.Lime);
    public static readonly ColorCode Green = new(Color.Green);
    public static readonly ColorCode ElectricBlue = new(74, 176, 231);
    public static readonly ColorCode Cyan = new(Color.Cyan);
    public static readonly ColorCode Blue = new(Color.Blue);
    public static readonly ColorCode Purple = new(Color.Purple);
    public static readonly ColorCode Fuchsia = new(Color.Fuchsia);

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
    public static readonly ColorCode Ally = new(131, 170, 240);
    public static readonly ColorCode Opp = new(255, 116, 116);
    public static readonly ColorCode Turn = new(160, 52, 255);
    public static readonly ColorCode Hp = new(26, 225, 50);
    public static readonly ColorCode Sp = new(187, 0, 255);
    public static readonly ColorCode Shield = new(Color.Cyan);
    public static readonly ColorCode Bloom = new(Color.Fuchsia);
    public static readonly ColorCode Buff = new(198, 161, 255);
    public static readonly ColorCode Skill = new(149, 201, 255);
    public static readonly ColorCode Element = new(Skill.Color);
    public static readonly ColorCode Passive = new(198, 161, 255);
    public static readonly ColorCode Stat = new(222, 255, 129);
    public static readonly ColorCode Cooldown = new(24, 152, 255);
    public static readonly ColorCode Lux = new(255, 251, 183);
}