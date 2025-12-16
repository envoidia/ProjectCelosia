using Microsoft.Xna.Framework;

namespace API.Graphics;

public static class Colors {
    public static readonly Color Trans = new(0f, 0f, 0f, 0f);
    public static readonly Color TransBlack = new(0f, 0f, 0f, 0.6f);

    public static readonly Color LightPurple = new(155, 45, 255);

    public static readonly Color Pos = Color.Lime;
    public static readonly Color Neg = new(255, 81, 81);
    public static readonly Color Num = Color.Yellow;

    public static readonly Color Hp = new(26, 225, 50);
    public static readonly Color Shield = Color.Cyan;
    public static readonly Color Overheal = new(238, 130, 239);
    public static readonly Color Stat = new(222, 255, 129);

    // todo pick better colors
    public static readonly Color Bg = new(0, 0, 0);
    public static readonly Color Fg = new(255, 255, 255);
    public static readonly Color Accent = new(160, 32, 240);

    // Debug outlines for actors
    public static readonly Color ActorOutline = Color.Fuchsia;
    public static readonly Color ActorOutlineInvis = new(Color.Fuchsia, 0.25f);
    public static readonly Color ActorPadding = Color.Cyan;
    public static readonly Color ActorPaddingInvis = new(Color.Cyan, 0.25f);
    public static readonly Color ActorOrigin = Color.Lime;
    public static readonly Color ActorMarked = new(1f, 0f, 0f, 0.1f);
    public static readonly Color ActorDisabledInput = new(0f, 0f, 1f, 0.1f);

    // Icons
    // todo public const string ShieldIcon = $"{Shield}[+vibrating-shield]";
}