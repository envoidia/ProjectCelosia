using Microsoft.Xna.Framework;

namespace API.Graphics;

public static class Colors {
    // https://lospec.com/palette-list/apollo
    public static readonly Color[] Blues = FromRgbs(0x172038, 0x253a5e, 0x3c5e8b, 0x4f8fba, 0x73bed3, 0xa4dddb);
    public static readonly Color[] Greens = FromRgbs(0x19332d, 0x25562e, 0x468232, 0x75a743, 0xa8ca58, 0xd0da91);
    public static readonly Color[] Beiges = FromRgbs(0x4d2b32, 0x7a4841, 0xad7757, 0xc09473, 0xd7b594, 0xe7d5b3);
    public static readonly Color[] Oranges = FromRgbs(0x341c27, 0x602c2c, 0x884b2b, 0xbe772b, 0xde9e41, 0xe8c170);
    public static readonly Color[] RedOranges = FromRgbs(0x241627, 0x411d31, 0x752438, 0xa53030, 0xcf573c, 0xda863e);
    public static readonly Color[] Pinks = FromRgbs(0x1e1d39, 0x402751, 0x7a367b, 0xa23e8c, 0xc65197, 0xdf84a5);
    public static readonly Color[] GrayBlues = FromRgbs(0x090a14, 0x10141f, 0x151d28, 0x202e37, 0x394a50, 0x577277);
    public static readonly Color[] Whites = FromRgbs(0x819796, 0xa8b5b2, 0xc7cfcc, 0xedede9);

    public static readonly Color[] All =
        [.. Blues, .. Greens, .. Beiges, .. Oranges, .. RedOranges, .. Pinks, .. GrayBlues, .. Whites];

    public static readonly Color Trans = new(0);

    public static readonly Color White = Whites[3];
    public static readonly Color Black = GrayBlues[0];
    public static readonly Color TransBlack = new(Black, 0.6f);

    /// <summary>
    /// Positive numbers
    /// </summary>
    public static readonly Color Pos = Greens[2];

    /// <summary>
    /// Negative numbers
    /// </summary>
    public static readonly Color Neg = RedOranges[4];

    /// <summary>
    /// General numbers (turns, stacks)
    /// </summary>
    public static readonly Color Num = Greens[5];

    public static readonly Color Ally = Blues[3];
    public static readonly Color Opp = RedOranges[3];
    public static readonly Color Turn = Pinks[4];
    public static readonly Color Hp = Greens[3];
    public static readonly Color Sp = Pinks[3];
    public static readonly Color Shield = Blues[4];
    public static readonly Color Bloom = Pinks[5];
    public static readonly Color Buff = Pinks[5];
    public static readonly Color Skill = Blues[4];
    public static readonly Color Element = Blues[4];
    public static readonly Color Passive = Pinks[5];
    public static readonly Color Stat = Greens[5];
    public static readonly Color Cooldown = Blues[3];

    public static readonly Color SpBack = Pinks[5];
    public static readonly Color Overheal = Pinks[4];

    public static readonly Color Bg = GrayBlues[1];
    public static readonly Color Fg = Whites[3];
    public static readonly Color Accent = Blues[1];

    // Debug outlines for actors
    public static readonly Color ActorOutline = Color.Blue;
    public static readonly Color ActorOutlineProg0 = Color.Yellow;
    public static readonly Color ActorOutlineProg1 = Color.Fuchsia;
    public static readonly Color ActorPadding = Color.Cyan;
    public static readonly Color ActorOrigin = Color.Lime;
    public static readonly Color ActorMarked = new(1f, 0f, 0f, 0.1f);
    public static readonly Color ActorDisabledInput = new(0f, 0f, 1f, 0.1f);

    // Icons
    // todo public const string ShieldIcon = $"{Shield}[+vibrating-shield]";

    /// <returns>
    /// A array of <c>Color</c>s made from the given hexes (0xRRGGBB)
    /// </returns>
    public static Color[] FromRgbs(params uint[] hexes) {
        Color[] colors = new Color[hexes.Length];
        for (int i = 0; i < hexes.Length; i++) colors[i] = FromRgb(hexes[i]);
        return colors;
    }

    /// <returns>
    /// A <c>Color</c> made from the given hex (<c>0xRRGGBB</c>)
    /// </returns>
    // Xna.Framework.Color is stored as AGBR
    public static Color FromRgb(uint rgb) =>
        new(0xff000000     // A
            | ((rgb & 0xff) << 16)       // R
            | (((rgb >> 8) & 0xff) << 8) // G
            | ((rgb >> 16) & 0xff));     // B
}