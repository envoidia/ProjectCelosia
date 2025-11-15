using Microsoft.Xna.Framework;

namespace API.Graphics;

public static class Colors {
    public static readonly Color TransBlack = new(0f, 0f, 0f, 0.6f);
    public static readonly Color VPurple = new(160, 32, 240);

    // Text colors
    public const string White = "/c[white]";
    public const string Ally = "/c[#2d74f5]";
    public const string AllyL = "/c[#528cf5]";
    public const string Opp = "/c[#ff4545]";
    public const string OppL = "/c[#ff6060]";
    public const string Turn = "/c[#a034ff]";
    public const string Pos = "/c[#00ff00]"; // Positive numbers
    public const string Neg = "/c[#ff5151]"; // Negative numbers
    public const string Num = "/c[#ffff00]"; // General numbers (turns, stacks)
    public const string Hp = "/c[#1ae132]";
    public const string Sp = "/c[#bb00ff]";
    public const string Shield = "/c[#00ffff]";
    public const string Bloom = "/c[#ff00ff]";
    public const string Buff = "/c[#c6a1ff]";
    public const string Skill = "/c[#95c9ff]";
    public const string Element = "/c[#95c9ff]";
    public const string Passive = "/c[#c6a1ff]";
    public const string Stat = "/c[#deff81]";
    public const string Cd = "/c[#1898ff]"; // Cooldown
    public const string Lux = "/c[#fffbb7]";

    // Icons
    public const string ShieldIcon = Shield + "[+vibrating-shield]";
}