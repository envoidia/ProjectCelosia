using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using API.Debug;
using API.Extensions;
using API.Modding;
using API.Name;
using API.Save;
using API.Util;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Color theme
/// </summary>
// todo load themes from props
public sealed class Theme : IDescribable, IRegistrable
{
    /// <summary>
    /// Notified when the current <c>Theme</c> changes
    /// </summary>
    public static event Action? OnChange;

    /// <summary>
    /// Whether the name and desc should be treated as keys
    /// </summary>
    public readonly bool UseLangKey;

    public string KeyName { get; }
    public string KeyDesc { get; }

    public string ModId { get; }
    public string ItemId { get; init; }

    /// <summary>
    /// Alpha to use for translucent colors
    /// </summary>
    public const int TransAlpha = 160;

    public Theme(string modId, string keyName, string? keyDesc = null, string? itemId = null)
    {
        this.KeyName = keyName;
        this.KeyDesc = keyDesc ?? $"{keyName}Desc";

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
    }

    #region Default Themes

    // todo not props
    public static Theme Apollo { get; set; } = null!;
    public static Theme Void { get; set; } = null!;
    // public static Theme VSCode { get; set; } = null!;
    public static Theme HighContrast { get; set; } = null!;
    public static Theme MikuMikuTheme { get; set; } = null!;
    public static Theme RedMode { get; set; } = null!;

    internal static void _Init()
    {
        _LoadThemes();

        // https://lospec.com/palette-list/apollo
        #region Apollo

        {
            Span<Color> blues = Color.FromRgbs(0x172038, 0x253a5e, 0x3c5e8b, 0x4f8fba, 0x73bed3, 0xa4dddb);
            Span<Color> greens = Color.FromRgbs(0x19332d, 0x25562e, 0x468232, 0x75a743, 0xa8ca58, 0xd0da91);
            Span<Color> beiges = Color.FromRgbs(0x4d2b32, 0x7a4841, 0xad7757, 0xc09473, 0xd7b594, 0xe7d5b3);
            Span<Color> oranges = Color.FromRgbs(0x341c27, 0x602c2c, 0x884b2b, 0xbe772b, 0xde9e41, 0xe8c170);
            Span<Color> redOranges = Color.FromRgbs(0x241627, 0x411d31, 0x752438, 0xa53030, 0xcf573c, 0xda863e);
            Span<Color> pinks = Color.FromRgbs(0x1e1d39, 0x402751, 0x7a367b, 0xa23e8c, 0xc65197, 0xdf84a5);
            Span<Color> grayBlues = Color.FromRgbs(0x090a14, 0x10141f, 0x151d28, 0x202e37, 0x394a50, 0x577277);
            Span<Color> whites = Color.FromRgbs(0x819796, 0xa8b5b2, 0xc7cfcc, 0xedede9);

            Apollo = new(Core.Id, "ThemeApollo")
            {
                BgTrans = new(grayBlues[0], TransAlpha),
                Midtone = whites[0],
                Fg = whites[3],
                Bg = grayBlues[1],
                Accent = blues[1],
                AccentDeemphasized = Color.Blue,

                BgSecondary = Color.Red,
                Reticle = Color.Red,
                Positive = greens[2],
                Negative = redOranges[4],
                Emphasis = greens[5],
                Ally = blues[3],
                Opponent = redOranges[3],
                Turn = pinks[4],
                Hp = greens[3],
                Sp = pinks[3],
                Shield = blues[4],
                Bloom = pinks[5],
                Buff = pinks[5],
                Skill = blues[4],
                Element = blues[4],
                Passive = pinks[5],
                Stat = greens[5],
                Cooldown = blues[3],

                SpBack = pinks[5],
                Overheal = pinks[4],
                StatBarLayer4 = blues[5],
                StatBarLayer5 = pinks[5],

                Atk = redOranges[3],
                Def = blues[3],
                Fth = pinks[1],
                Agi = greens[2],

                Vis = whites[1],
                Ignis = redOranges[5],
                Glacies = blues[4],
                Fulgur = oranges[5],
                Ventus = greens[2],
                Terra = oranges[2],
                Lux = beiges[5],
                Malum = redOranges[4]
            };
        }

        #endregion

        #region Void

        {
            Color white = Color.FromRgba(0xc0c0c0);
            Color gray = Color.FromRgba(0x545454);
            Color black = Color.FromRgba(0x1f1f1f);

            Color paleYellow = Color.FromRgba(0xDCDCAA);

            Color green = Color.FromRgba(0x76b668);
            Color paleBlueGreen = Color.FromRgba(0x66c8cc);
            Color blueGreen = Color.FromRgba(0x32bb99);

            Color paleBlue = Color.FromRgba(0xb5c0ff);
            Color electricBlue = Color.FromRgba(0x4ab0e7);
            Color blue = Color.FromRgba(0x6798ff);
            Color bluePurple = Color.FromRgba(0x8189eb);

            Color palePurple = Color.FromRgba(0xdac9ff);
            Color lightPurple = Color.FromRgba(0xbe8ff9);

            Color salmon = Color.FromRgba(0xed94bb);
            Color lightPink = Color.FromRgba(0xe9aee4);
            Color darkPink = Color.FromRgba(0xda86d1);

            Void = new(Core.Id, "ThemeVoid")
            {
                BgTrans = new(black, TransAlpha), Midtone = gray,

                Fg = white,
                Bg = black,
                Accent = gray,
                AccentDeemphasized = Color.Blue,

                BgSecondary = Color.Red,
                Reticle = Color.Red,
                Positive = blueGreen,
                Negative = salmon,
                Emphasis = paleYellow,
                Ally = electricBlue,
                Opponent = darkPink,
                Turn = bluePurple,
                Hp = green,
                Sp = bluePurple,
                Shield = electricBlue,
                Bloom = lightPink,
                Buff = lightPurple,
                Skill = blueGreen,
                Element = blue,
                Passive = lightPurple,
                Stat = paleBlueGreen,
                Cooldown = electricBlue,

                SpBack = palePurple,
                Overheal = paleBlue,
                StatBarLayer4 = bluePurple,
                StatBarLayer5 = palePurple,

                Atk = darkPink,
                Def = paleBlue,
                Fth = lightPurple,
                Agi = blueGreen,

                Vis = white,
                Ignis = salmon,
                Glacies = electricBlue,
                Fulgur = paleYellow,
                Ventus = green,
                Terra = darkPink,
                Lux = paleYellow,
                Malum = lightPurple
            };
        }

        #endregion

        // #region VSCode

        // {
        //     Color white = Color.FromRgb(0xfef5f7);
        //     Color gray = Color.FromRgb(0xa0a0a0);
        //     Color black = Color.FromRgb(0x1f1f1f);

        //     Color red = Color.FromRgb(0xd16969);
        //     Color orange = Color.FromRgb(0xCE9178);

        //     Color paleYellow = Color.FromRgb(0xDCDCAA);
        //     Color yellow = Color.FromRgb(0xffd606);

        //     Color paleGreen = Color.FromRgb(0xb5cea8);
        //     Color blueGreen = Color.FromRgb(0x4EC9B0);

        //     Color paleBlue = Color.FromRgb(0x9CDCFE);
        //     Color lightBlue = Color.FromRgb(0x4FC1FF);
        //     Color electricBlue = Color.FromRgb(0x1e99f5);
        //     Color darkElectricBlue = Color.FromRgb(0x0877d3);
        //     Color dirtyBlue = Color.FromRgb(0x5798d2);

        //     Color darkPink = Color.FromRgb(0xC586C0);
        //     Color magenta = Color.FromRgb(0xd96fd5);

        //     VSCode = new(Core.Id, "ThemeVSCode")
        //     {
        //         BgTrans = new(black, TransAlpha), Midtone = gray,

        //         Fg = white,
        //         Bg = black,
        //         Accent = darkElectricBlue,
        //         AccentDeemphasized = Color.Red,

        //         BgSecondary = Color.Red,
        //         Reticle = Color.Red,
        //         Positive = blueGreen,
        //         Negative = red,
        //         Emphasis = paleYellow,
        //         Ally = paleBlue,
        //         Opponent = red,
        //         Turn = darkPink,
        //         Hp = paleGreen,
        //         Sp = paleBlue,
        //         Shield = paleBlue,
        //         Bloom = magenta,
        //         Buff = dirtyBlue,
        //         Skill = blueGreen,
        //         Element = blueGreen,
        //         Passive = dirtyBlue,
        //         Stat = paleYellow,
        //         Cooldown = electricBlue,

        //         SpBack = magenta,
        //         Overheal = darkPink,
        //         StatBarLayer4 = electricBlue,
        //         StatBarLayer5 = magenta,

        //         Atk = red,
        //         Def = lightBlue,
        //         Fth = darkPink,
        //         Agi = blueGreen,

        //         Vis = gray,
        //         Ignis = red,
        //         Glacies = lightBlue,
        //         Fulgur = yellow,
        //         Ventus = paleGreen,
        //         Terra = orange,
        //         Lux = paleYellow,
        //         Malum = darkPink

        //     };
        // }

        // #endregion

        #region HighContrast

        // todo rework
        {
            Color lightRed = new(255, 81, 81);
            Color elecBlue = new(24, 152, 255);

            HighContrast = new(Core.Id, "ThemeHighContrast")
            {
                BgTrans = new(Color.Black, TransAlpha), Midtone = Color.Gray,

                Fg = Color.White,
                Bg = Color.Black,
                Accent = new(160, 32, 240),
                AccentDeemphasized = Color.Blue,

                BgSecondary = Color.Red,
                Reticle = Color.Red,
                Positive = Color.Lime,
                Negative = lightRed,
                Emphasis = Color.Yellow,
                Ally = new(131, 170, 240), // todo not readable enough
                Opponent = new(255, 116, 116),
                Turn = new(160, 52, 255),
                Hp = new(26, 225, 50),
                Sp = new(187, 0, 255),
                Shield = Color.Cyan,
                Bloom = Color.Fuchsia,
                Buff = new(198, 161, 255),
                Skill = new(149, 201, 255),
                Element = new(149, 201, 255), // todo should these be different
                Passive = new(198, 161, 255),
                Stat = new(222, 255, 129),
                Cooldown = elecBlue,

                SpBack = Color.FromRgba(0xd78bff),
                Overheal = new(238, 130, 239),
                StatBarLayer4 = Color.Cyan,
                StatBarLayer5 = Color.Pink,

                Atk = lightRed,
                Def = elecBlue,
                Fth = new(181, 86, 238),
                Agi = Color.LightGreen,

                Vis = Color.LightGray,
                Ignis = Color.Orange,
                Glacies = Color.LightBlue,
                Fulgur = Color.Yellow,
                Ventus = Color.Lime,
                Terra = Color.SandyBrown,
                Lux = new(255, 251, 183),
                Malum = Color.Red
            };
        }

        #endregion

        #region MikuMikuTheme

        {
            Color white = Color.FromRgba(0xfef5f7);
            Color gray = Color.FromRgba(0xa0a0a0);
            Color black = Color.FromRgba(0x1f1f1f);

            Color paleBeige = Color.FromRgba(0xd3d3cc);
            Color darkBeige = Color.FromRgba(0x9f9294);

            Color paleGreen = Color.FromRgba(0xb6dbca);
            Span<Color> hair = Color.FromRgbs(0x89cdc6, 0x51acb6, 0x338397);

            Color paleBlue = Color.FromRgba(0xd6ecf9);
            Color darkBlue = Color.FromRgba(0x336699);

            Color pink = Color.FromRgba(0xec83a8);
            Color hotPink = Color.FromRgba(0xe8418f);
            Color redPink = Color.FromRgba(0xe3004f);

            MikuMikuTheme = new(Core.Id, "ThemeMikuMikuTheme")
            {
                BgTrans = new(black, TransAlpha),

                Fg = paleBlue,
                Midtone = gray,
                Bg = black,
                Accent = darkBlue,
                AccentDeemphasized = Color.Blue,

                BgSecondary = Color.Red,
                Reticle = Color.Red,
                Positive = hair[1],
                Negative = pink,
                Emphasis = paleGreen,
                Ally = hair[0],
                Opponent = redPink,
                Turn = hotPink,
                Hp = paleGreen,
                Sp = hotPink,
                Shield = hair[0],
                Bloom = hotPink,
                Buff = hair[2],
                Skill = hair[2],
                Element = hair[2],
                Passive = hair[2],
                Stat = paleBeige,
                Cooldown = darkBlue,

                SpBack = pink,
                Overheal = pink,
                StatBarLayer4 = hair[0],
                StatBarLayer5 = pink,

                Atk = redPink,
                Def = darkBlue,
                Fth = hotPink,
                Agi = paleGreen,

                Vis = gray,
                Ignis = pink,
                Glacies = hair[1],
                Fulgur = paleBeige,
                Ventus = paleGreen,
                Terra = darkBeige,
                Lux = hair[0],
                Malum = redPink
            };
        }

        #endregion

        #region RED MODE!!!

        {
            Span<Color> r = new Color[10];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = new((i * 23) + 26, 0, 0);
            }

            Color white = new(255, 100, 100);

            RedMode = new(Core.Id, "ThemeRedMode")
            {
                BgTrans = new(r[0], TransAlpha),

                Fg = white,
                Midtone = r[9],
                Bg = r[0],
                Accent = r[2],
                AccentDeemphasized = Color.Blue,

                BgSecondary = Color.Red,
                Reticle = Color.Red,
                Positive = r[9],
                Negative = r[7],
                Emphasis = r[8],
                Ally = r[9],
                Opponent = r[7],
                Turn = r[8],
                Hp = r[9],
                Sp = r[6],
                Shield = r[7],
                Bloom = r[8],
                Buff = r[7],
                Skill = r[7],
                Element = r[7],
                Passive = r[7],
                Stat = r[8],
                Cooldown = r[8],

                SpBack = r[9],
                Overheal = r[5],
                StatBarLayer4 = r[8],
                StatBarLayer5 = r[6],

                Atk = r[7],
                Def = r[7],
                Fth = r[7],
                Agi = r[7],

                Vis = r[9],
                Ignis = r[6],
                Glacies = r[7],
                Fulgur = r[7],
                Ventus = r[7],
                Terra = r[5],
                Lux = r[8],
                Malum = r[6]
            };
        }

        #endregion
    }

    private static void _LoadThemes()
    {
        string[] themeFiles;
        try
        {
            themeFiles = Directory.GetFiles("Themes");
        }
        catch (Exception e)
        {
            Core._LogOrEarlyLog($"Failed to read theme files: {e.Message}",
                nameof(_LoadThemes), LogLevel.Err);
            return;
        }

        foreach (string path in themeFiles)
        {
            try
            {
                Dictionary<string, string> themeDict = Properties.Parse(path);

                bool hasName = themeDict.TryGetValue("Name", out string? name);
                bool hasDesc = themeDict.TryGetValue("Desc", out string? desc);

                bool useLangKey;

                if (themeDict.TryGetValue(nameof(UseLangKey), out string? useLangKeyStr))
                {
                    if (!bool.TryParse(useLangKeyStr, out bool useLangKey1))
                    {
                        Core._LogOrEarlyLog("\"UseLangKey\" is present but invalid. Defaulting to false",
                            nameof(_LoadThemes), LogLevel.Err);
                    }

                    useLangKey = useLangKey1;
                }
                else
                {
                    useLangKey = false;
                }

                string nameOrMissing = name ?? "MISSING NAME";

                Theme t = new(Core.Id, nameOrMissing,
                    desc is null && !useLangKey ? "MISSING DESC" : desc, $"Theme{nameOrMissing}")
                {
                    Fg = parseColor(nameof(Fg)),
                    Midtone = parseColor(nameof(Midtone)),
                    Bg = parseColor(nameof(Bg)),
                    BgSecondary = parseColor(nameof(BgSecondary)),
                    BgTrans = parseColor(nameof(BgTrans)),

                    Accent = parseColor(nameof(Accent)),
                    AccentDeemphasized = parseColor(nameof(AccentDeemphasized)),

                    Positive = parseColor(nameof(Positive)),
                    Negative = parseColor(nameof(Negative)),
                    Emphasis = parseColor(nameof(Emphasis)),

                    Ally = parseColor(nameof(Ally)),
                    Opponent = parseColor(nameof(Opponent)),
                    Turn = parseColor(nameof(Turn)),
                    Hp = parseColor(nameof(Hp)),
                    Sp = parseColor(nameof(Sp)),
                    Shield = parseColor(nameof(Shield)),
                    Bloom = parseColor(nameof(Bloom)),
                    Buff = parseColor(nameof(Buff)),
                    Skill = parseColor(nameof(Skill)),
                    Element = parseColor(nameof(Element)),
                    Passive = parseColor(nameof(Passive)),
                    Stat = parseColor(nameof(Stat)),
                    Reticle = parseColor(nameof(Reticle)),
                    Cooldown = parseColor(nameof(Cooldown)),

                    SpBack = parseColor(nameof(SpBack)),
                    Overheal = parseColor(nameof(Overheal)),
                    StatBarLayer4 = parseColor(nameof(StatBarLayer4)),
                    StatBarLayer5 = parseColor(nameof(StatBarLayer5)),

                    Atk = parseColor(nameof(Atk)),
                    Def = parseColor(nameof(Def)),
                    Fth = parseColor(nameof(Fth)),
                    Agi = parseColor(nameof(Agi)),

                    Vis = parseColor(nameof(Vis)),
                    Ignis = parseColor(nameof(Ignis)),
                    Glacies = parseColor(nameof(Glacies)),
                    Fulgur = parseColor(nameof(Fulgur)),
                    Ventus = parseColor(nameof(Ventus)),
                    Terra = parseColor(nameof(Terra)),
                    Lux = parseColor(nameof(Lux)),
                    Malum = parseColor(nameof(Malum)),
                };

                Color parseColor(string key)
                {
                    if (!themeDict.TryGetValue(key, out string? colorStr))
                    {
                        Core._LogOrEarlyLog($"\"{key}\" is missing. Defaulting to red",
                            nameof(_LoadThemes), LogLevel.Err);
                        return Color.Red;
                    }

                    Assert.NotNull(colorStr);

                    string colorStrFormatted = colorStr.Replace("#", "");

                    if (colorStrFormatted.Length == 6)
                    {
                        colorStrFormatted += "ff";
                    }
                    else if (colorStrFormatted.Length != 8)
                    {
                        return invalidColor();
                    }

                    if (!uint.TryParse(colorStrFormatted, NumberStyles.HexNumber,
                        CultureInfo.InvariantCulture, out uint colorVal))
                    {
                        return invalidColor();
                    }

                    return Color.FromRgba(colorVal);

                    Color invalidColor()
                    {
                        Core._LogOrEarlyLog(
                            $"\"{key}\" should be a 6 or 8 digit hex color (#rrggbb or #rrggbbaa), but was {colorStr}. Defaulting to red",
                            nameof(_LoadThemes), LogLevel.Err);
                        return Color.Red;
                    }
                }
            }
            catch (Exception e)
            {
                Core._LogOrEarlyLog($"Failed to read theme file at {path}: {e.Message}",
                    nameof(_LoadThemes), LogLevel.Err);
            }
        }

    }

    // private static

    #endregion

    #region Colors

    #region General

    /// <summary>
    /// Allocates
    /// </summary>
    public Color[] AllColors
    {
        get
        {
            return [this.Fg, this.Midtone, this.Bg, this.BgSecondary, this.BgTrans, this.Accent,
                this.AccentDeemphasized, this.Positive, this.Negative, this.Emphasis, this.Ally, this.Opponent,
                this.Turn, this.Hp, this.Sp, this.Shield, this.Bloom, this.Buff, this.Skill, this.Element,
                this.Passive, this.Stat, this.Reticle, this.Cooldown, this.SpBack, this.Overheal, this.StatBarLayer4,
                this.StatBarLayer5, this.Atk, this.Def, this.Fth, this.Agi, this.Vis, this.Ignis, this.Glacies,
                this.Fulgur, this.Ventus, this.Terra, this.Lux, this.Malum];
        }
    }

    /// <summary>
    /// Main foreground
    /// </summary>
    public required Color Fg { get; init; }

    /// <summary>
    /// Middle point between <c>Fg</c> and <c>Bg</c>
    /// </summary>
    public required Color Midtone { get; init; }

    /// <summary>
    /// Main background
    /// </summary>
    public required Color Bg { get; init; }

    /// <summary>
    /// Secondary background
    /// </summary>
    public required Color BgSecondary { get; init; }

    /// <summary>
    /// Partially transparent version of bg
    /// </summary>
    public required Color BgTrans { get; init; }

    /// <summary>
    /// Main accent
    /// </summary>
    public required Color Accent { get; init; }

    /// <summary>
    /// Less emphasized accent (eg for mouse hover)
    /// </summary>
    public required Color AccentDeemphasized { get; init; }

    /// <summary>
    /// Positive/good stuff
    /// </summary>
    public required Color Positive { get; init; }

    /// <summary>
    /// Negative/bad stuff
    /// </summary>
    public required Color Negative { get; init; }

    /// <summary>
    /// Neutral emphasis
    /// </summary>
    public required Color Emphasis { get; init; }

    #endregion

    #region Battle

    /// <summary>
    /// Ally names
    /// </summary>
    public required Color Ally { get; init; }

    /// <summary>
    /// Opponent names
    /// </summary>
    public required Color Opponent { get; init; }

    /// <summary>
    /// Current turn text
    /// </summary>
    public required Color Turn { get; init; }

    /// <summary>
    /// HP text and bar
    /// </summary>
    public required Color Hp { get; init; }

    /// <summary>
    /// SP text and bar
    /// </summary>
    public required Color Sp { get; init; }

    /// <summary>
    /// Shield text and bar
    /// </summary>
    public required Color Shield { get; init; }

    /// <summary>
    /// Bloom text and bar
    /// </summary>
    public required Color Bloom { get; init; }

    /// <summary>
    /// Buff names
    /// </summary>
    public required Color Buff { get; init; }

    /// <summary>
    /// Skill names
    /// </summary>
    public required Color Skill { get; init; }

    /// <summary>
    /// Element names
    /// </summary>
    public required Color Element { get; init; }

    /// <summary>
    /// Passive names
    /// </summary>
    public required Color Passive { get; init; }

    /// <summary>
    /// Stat names
    /// </summary>
    public required Color Stat { get; init; }

    /// <summary>
    /// Targeting menu reticle
    /// </summary>
    public required Color Reticle { get; init; }

    /// <summary>
    /// Cooldown text
    /// </summary>
    public required Color Cooldown { get; init; }

    /// <summary>
    /// Back layer of SP bar
    /// </summary>
    public required Color SpBack { get; init; }

    /// <summary>
    /// Overheal bar
    /// </summary>
    public required Color Overheal { get; init; }

    /// <summary>
    /// 4th layer of stat bars (201-300%)
    /// </summary>
    public required Color StatBarLayer4 { get; init; }

    /// <summary>
    /// 5th layer of stat bars (301-400%)
    /// </summary>
    public required Color StatBarLayer5 { get; init; }

    #region StageTypes

    /// <summary>
    /// Attack stage
    /// </summary>
    public required Color Atk { get; init; }

    /// <summary>
    /// Defense stage
    /// </summary>
    public required Color Def { get; init; }

    /// <summary>
    /// Faith stage
    /// </summary>
    public required Color Fth { get; init; }

    /// <summary>
    /// Agility stage
    /// </summary>
    public required Color Agi { get; init; }

    #endregion

    #region Elements

    /// <summary>
    /// Vis (neutral) element
    /// </summary>
    public required Color Vis { get; init; }

    /// <summary>
    /// Ignis (fire) element
    /// </summary>
    public required Color Ignis { get; init; }

    /// <summary>
    /// Glacies (ice) element
    /// </summary>
    public required Color Glacies { get; init; }

    /// <summary>
    /// Fulgur (electric) element
    /// </summary>
    public required Color Fulgur { get; init; }

    /// <summary>
    /// Ventus (wind) element
    /// </summary>
    public required Color Ventus { get; init; }

    /// <summary>
    /// Terra (earth) element
    /// </summary>
    public required Color Terra { get; init; }

    /// <summary>
    /// Lux (light) element
    /// </summary>
    public required Color Lux { get; init; }

    /// <summary>
    /// Malum (evil) element
    /// </summary>
    public required Color Malum { get; init; }

    #endregion

    #endregion

    #endregion

    #region Methods

    public Color Get(ThemeColor tc)
    {
        return tc switch
        {
            ThemeColor.Fg => this.Fg,
            ThemeColor.Midtone => this.Midtone,
            ThemeColor.Bg => this.Bg,
            ThemeColor.BgSecondary => this.BgSecondary,
            ThemeColor.BgTrans => this.BgTrans,

            ThemeColor.Accent => this.Accent,
            ThemeColor.AccentDeemphasized => this.AccentDeemphasized,

            ThemeColor.Positive => this.Positive,
            ThemeColor.Negative => this.Negative,
            ThemeColor.Emphasis => this.Emphasis,
            ThemeColor.Ally => this.Ally,
            ThemeColor.Opponent => this.Opponent,
            ThemeColor.Turn => this.Turn,
            ThemeColor.Hp => this.Hp,
            ThemeColor.Sp => this.Sp,
            ThemeColor.Shield => this.Shield,
            ThemeColor.Bloom => this.Bloom,
            ThemeColor.Buff => this.Buff,
            ThemeColor.Skill => this.Skill,
            ThemeColor.Element => this.Element,
            ThemeColor.Passive => this.Passive,
            ThemeColor.Stat => this.Stat,
            ThemeColor.Reticle => this.Reticle,
            ThemeColor.Cooldown => this.Cooldown,

            ThemeColor.SpBack => this.SpBack,
            ThemeColor.Overheal => this.Overheal,
            ThemeColor.StatBarLayer4 => this.StatBarLayer4,
            ThemeColor.StatBarLayer5 => this.StatBarLayer5,

            ThemeColor.Atk => this.Atk,
            ThemeColor.Def => this.Def,
            ThemeColor.Fth => this.Fth,
            ThemeColor.Agi => this.Agi,

            ThemeColor.Vis => this.Vis,
            ThemeColor.Ignis => this.Ignis,
            ThemeColor.Glacies => this.Glacies,
            ThemeColor.Fulgur => this.Fulgur,
            ThemeColor.Ventus => this.Ventus,
            ThemeColor.Terra => this.Terra,
            ThemeColor.Lux => this.Lux,
            ThemeColor.Malum => this.Malum,

            _ => throw new ClosedEnumsWhenException()
        };
    }

    internal void _DrawPalette()
    {
        const int Size = 64;

        int y = -Size;
        for (int i = 0; i < this.AllColors.Length; i++)
        {
            int iMod = i % 8;

            if (iMod == 0)
            {
                y += Size;
            }

            int x = iMod * Size;

            Core.ShapeBatch.FillRectangle(new(x, y), new(Size, Size), this.AllColors[i]);
        }
    }

    internal static void _Change()
    {
        _ChangeFSSColors();
        OnChange?.Invoke();
    }

    /// <summary>
    /// Add custom color aliases to FSS's text processing for the given palette
    /// </summary>
    internal static void _ChangeFSSColors()
    {
        Dictionary<string, Color> colorMap = [];

        foreach (ThemeColor tc in Enum.GetValues<ThemeColor>())
        {
            colorMap[tc.ToString().ToLower()] = Settings.Theme.Get(tc);
        }
        foreach (KeyValuePair<string, Color> kvp in colorMap)
        {
            ColorStorage.Colors[kvp.Key] = new()
            {
                Color = kvp.Value
            };
        }
    }

    public override string ToString()
    {
        return $"{base.ToString()}: {this.GetName()} -- {this.GetDesc()}";
    }

    public string ToDetailedString(bool renderColor)
    {
        const int Cap = 1500;
        StringBuilder sb = new(this.ToString(), Cap);

        ThemeColor[] tcs = Enum.GetValues<ThemeColor>();
        for (int i = 0; i < tcs.Length; i++)
        {
            sb.Append($"{tcs[i]} = {(renderColor ? tcs[i].Str : null)}{this.Get(tcs[i])
                .ToRgbaStr()}{(renderColor ? ThemeColor.Fg.Str : null)}");

            if (i != tcs.Length - 1)
            {
                sb.Append('\n');
            }
        }

        Assert.CapIs(sb, Cap); // todo remove before final release
        return sb.ToString();
    }

    public string GetName(ThemeColor color)
    {
        if (this.UseLangKey)
        {
            return color.Str + this.GetLang();
        }

        return color.Str + this.KeyName;
    }

    public string GetName()
    {
        return this.GetName(ThemeColor.Fg);
    }

    public string GetDesc()
    {
        if (this.UseLangKey)
        {
            return this.KeyDesc.GetLang(this.ModId);
        }

        return this.KeyDesc;
    }

    #endregion
}
