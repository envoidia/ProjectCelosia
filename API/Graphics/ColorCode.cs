using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// A color code for formatting strings
/// </summary>
public readonly record struct ColorCode {
    private readonly Color _c;

    public ColorCode(Color c) => this._c = c;
    public ColorCode(uint rgb) => this._c = Colors.FromRgb(rgb);
    public ColorCode(Color c, byte a = 255) : this(new Color(c, a)) { }
    public ColorCode(byte r, byte g, byte b, byte a = 255) : this(new Color(r, g, b, a)) { }

    public string ToRgbaStr() => $"#{this._c.R:x2}{this._c.G:x2}{this._c.B:x2}{this._c.A:x2}";

    public static implicit operator Color(ColorCode c) => c._c;
    public static implicit operator ColorCode(Color c) => new(c);
}