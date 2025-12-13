using Microsoft.Xna.Framework;

namespace API.Graphics;

public static class Colors {
    public static readonly Color Trans = new(0f, 0f, 0f, 0f);
    public static readonly Color TransBlack = new(0f, 0f, 0f, 0.6f);

    public static readonly Color Bg = new(17, 0, 17);
    public static readonly Color Fg = new(231, 199, 231);
    public static readonly Color Accent = new(160, 32, 240);

    // Debug outlines for actors
    public static readonly Color ActorOutline = Color.Fuchsia;
    public static readonly Color ActorOutlineInvis = new(Color.Fuchsia, 0.5f);
    public static readonly Color ActorPadding = Color.Cyan;
    public static readonly Color ActorPaddingInvis = new(Color.Cyan, 0.5f);
    public static readonly Color ActorOrigin = Color.Lime;
    public static readonly Color ActorMarked = new(1f, 0f, 0f, 0.1f);
    public static readonly Color ActorDisabledInput = new(0f, 0f, 1f, 0.1f);

    // Icons
    // todo public const string ShieldIcon = $"{Shield}[+vibrating-shield]";
}