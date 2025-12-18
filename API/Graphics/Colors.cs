using Microsoft.Xna.Framework;

namespace API.Graphics;

public static class Colors {
    public static readonly Color Trans = new(0);

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