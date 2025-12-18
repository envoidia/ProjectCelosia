using System.Collections.Generic;
using API.Extensions;
using API.Modding;
using API.Name;
using API.Util;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Color theme
/// </summary>
public class Theme : _IModItem, INameable {
    public delegate void ThemeChange(Theme prevTheme, Theme newTheme);

    public static event ThemeChange? Change;

    public GameMod? Source { get; }

    public string KeyName { get; }

    public Theme(GameMod? source, string keyName) {
        this.Source = source;
        this.KeyName = keyName;

        Core.Themes.Add(this);
    }

    #region Default Themes

    public static Theme Apollo { get; }
    public static Theme Void { get; }
    public static Theme VSCode { get; }
    public static Theme HighContrast { get; }
    public static Theme MikuMikuTheme { get; }
    public static Theme RedMode { get; }

    static Theme() {
        const int TransAmt = 150;

        #region Apollo

        {
            // https://lospec.com/palette-list/apollo
            Color[] blues = Colors.FromRgbs(0x172038, 0x253a5e, 0x3c5e8b, 0x4f8fba, 0x73bed3, 0xa4dddb);
            Color[] greens = Colors.FromRgbs(0x19332d, 0x25562e, 0x468232, 0x75a743, 0xa8ca58, 0xd0da91);
            Color[] beiges = Colors.FromRgbs(0x4d2b32, 0x7a4841, 0xad7757, 0xc09473, 0xd7b594, 0xe7d5b3);
            Color[] oranges = Colors.FromRgbs(0x341c27, 0x602c2c, 0x884b2b, 0xbe772b, 0xde9e41, 0xe8c170);
            Color[] redOranges = Colors.FromRgbs(0x241627, 0x411d31, 0x752438, 0xa53030, 0xcf573c, 0xda863e);
            Color[] pinks = Colors.FromRgbs(0x1e1d39, 0x402751, 0x7a367b, 0xa23e8c, 0xc65197, 0xdf84a5);
            Color[] grayBlues = Colors.FromRgbs(0x090a14, 0x10141f, 0x151d28, 0x202e37, 0x394a50, 0x577277);
            Color[] whites = Colors.FromRgbs(0x819796, 0xa8b5b2, 0xc7cfcc, 0xedede9);

            Apollo = new(null, "Apollo") {
                White = whites[3],
                Black = grayBlues[0],
                TransBlack = new(grayBlues[0], TransAmt),

                Fg = whites[3],
                Bg = grayBlues[1],
                Accent = blues[1],

                Pos = greens[2],
                Neg = redOranges[4],
                Imp = greens[5],
                Ally = blues[3],
                Opp = redOranges[3],
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
                Lux = beiges[2],
                Malum = redOranges[4]
            };
        }

        #endregion

        #region Void

        {
            Color white = Colors.FromRgb(0xc0c0c0);
            Color gray = Colors.FromRgb(0x545454);
            Color black = Colors.FromRgb(0x1f1f1f);

            Color green = Colors.FromRgb(0x76b668);
            Color paleBlueGreen = Colors.FromRgb(0x66c8cc);
            Color blueGreen = Colors.FromRgb(0x32bb99);

            Color paleBlue = Colors.FromRgb(0xb5c0ff);
            Color electricBlue = Colors.FromRgb(0x4ab0e7);
            Color blue = Colors.FromRgb(0x6798ff);
            Color bluePurple = Colors.FromRgb(0x8189eb);

            Color palePurple = Colors.FromRgb(0xdac9ff);
            Color lightPurple = Colors.FromRgb(0xbe8ff9);
            //Color brightPurple = Colors.FromRgb(0xb747ff);

            Color salmon = Colors.FromRgb(0xed94bb);
            Color lightPink = Colors.FromRgb(0xe9aee4);
            Color darkPink = Colors.FromRgb(0xda86d1);

            Void = new(null, "Void") {
                White = white,
                Black = black,
                TransBlack = new(black, TransAmt),

                Fg = white,
                Bg = black,
                Accent = gray,

                Pos = blueGreen,
                Neg = salmon,
                Imp = blue,
                Ally = electricBlue,
                Opp = darkPink,
                Turn = bluePurple,
                Hp = green,
                Sp = lightPurple,
                Shield = electricBlue,
                Bloom = lightPink,
                Buff = lightPurple,
                Skill = blue,
                Element = blue,
                Passive = lightPurple,
                Stat = paleBlueGreen,
                Cooldown = electricBlue,

                SpBack = palePurple,
                Overheal = paleBlue,
                StatBarLayer4 = paleBlue,
                StatBarLayer5 = palePurple,

                Atk = darkPink,
                Def = paleBlue,
                Fth = lightPurple,
                Agi = blueGreen,

                Vis = white,
                Ignis = salmon,
                Glacies = electricBlue,
                Fulgur = lightPink,
                Ventus = paleBlueGreen,
                Terra = darkPink,
                Lux = palePurple,
                Malum = lightPurple
            };
        }

        #endregion

        #region VSCode

        {
            Color white = Colors.FromRgb(0xfef5f7);
            Color gray = Colors.FromRgb(0xa0a0a0);
            Color black = Colors.FromRgb(0x1f1f1f);

            Color red = Colors.FromRgb(0xd16969);
            Color orange = Colors.FromRgb(0xCE9178);

            Color paleYellow = Colors.FromRgb(0xDCDCAA);
            Color yellow = Colors.FromRgb(0xffd606);

            Color paleGreen = Colors.FromRgb(0xb5cea8);
            Color blueGreen = Colors.FromRgb(0x4EC9B0);

            Color paleBlue = Colors.FromRgb(0x9CDCFE);
            Color lightBlue = Colors.FromRgb(0x4FC1FF);
            Color electricBlue = Colors.FromRgb(0x1e99f5);
            Color darkElectricBlue = Colors.FromRgb(0x0877d3);
            Color dirtyBlue = Colors.FromRgb(0x5798d2);

            Color darkPink = Colors.FromRgb(0xC586C0);
            Color magenta = Colors.FromRgb(0xd96fd5);

            VSCode = new(null, "VSCode") {
                White = white,
                Black = black,
                TransBlack = new(black, TransAmt),

                Fg = white,
                Bg = black,
                Accent = dirtyBlue,

                Pos = blueGreen,
                Neg = red,
                Imp = paleYellow,
                Ally = darkElectricBlue,
                Opp = red,
                Turn = darkPink,
                Hp = paleGreen,
                Sp = darkPink,
                Shield = paleBlue,
                Bloom = magenta,
                Buff = darkElectricBlue,
                Skill = blueGreen,
                Element = blueGreen,
                Passive = darkElectricBlue,
                Stat = paleYellow,
                Cooldown = electricBlue,

                SpBack = magenta,
                Overheal = darkPink,
                StatBarLayer4 = electricBlue,
                StatBarLayer5 = magenta,

                Atk = red,
                Def = lightBlue,
                Fth = darkPink,
                Agi = blueGreen,

                Vis = gray,
                Ignis = red,
                Glacies = lightBlue,
                Fulgur = yellow,
                Ventus = paleGreen,
                Terra = orange,
                Lux = paleYellow,
                Malum = darkPink

            };
        }

        #endregion

        #region HighContrast

        {
            ColorCode lightRed = new(255, 81, 81);
            ColorCode elecBlue = new(24, 152, 255);

            HighContrast = new(null, "HighContrast") {
                White = Color.White,
                Black = Color.Black,
                TransBlack = new(Color.Black, TransAmt),

                Fg = Color.White,
                Bg = Color.Black,
                Accent = new(160, 32, 240),

                Pos = Color.Lime,
                Neg = lightRed,
                Imp = Color.Yellow,
                Ally = new(131, 170, 240), // todo not readable enough
                Opp = new(255, 116, 116),
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

                SpBack = new(155, 45, 255),
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
            Color white = Colors.FromRgb(0xfef5f7);
            Color gray = Colors.FromRgb(0xa0a0a0);
            Color black = Colors.FromRgb(0x1f1f1f);

            Color paleBeige = Colors.FromRgb(0xd3d3cc);
            Color darkBeige = Colors.FromRgb(0x9f9294);

            Color paleGreen = Colors.FromRgb(0xb6dbca);
            Color[] hair = Colors.FromRgbs(0x89cdc6, 0x51acb6, 0x338397);

            Color paleBlue = Colors.FromRgb(0xd6ecf9);
            Color darkBlue = Colors.FromRgb(0x336699);

            Color pink = Colors.FromRgb(0xec83a8);
            Color hotPink = Colors.FromRgb(0xe8418f);
            Color redPink = Colors.FromRgb(0xe3004f);

            MikuMikuTheme = new(null, "MikuMikuTheme") {
                White = white,
                Black = black,
                TransBlack = new(black, TransAmt),

                Fg = paleBlue,
                Bg = black,
                Accent = darkBlue,

                Pos = hair[1],
                Neg = pink,
                Imp = paleGreen,
                Ally = hair[0],
                Opp = redPink,
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
            Color[] r = new Color[10];
            for (int i = 0; i < r.Length; i++) r[i] = new((i * 23) + 26, 0, 0);


            RedMode = new(null, "RedMode") {
                White = r[9],
                Black = r[0],
                TransBlack = new(r[0], TransAmt),

                Fg = r[9],
                Bg = r[0],
                Accent = r[2],

                Pos = r[4],
                Neg = r[2],
                Imp = r[3],
                Ally = r[4],
                Opp = r[5],
                Turn = r[6],
                Hp = r[4],
                Sp = r[5],
                Shield = r[6],
                Bloom = r[6],
                Buff = r[3],
                Skill = r[4],
                Element = r[4],
                Passive = r[3],
                Stat = r[7],
                Cooldown = r[4],

                SpBack = r[4],
                Overheal = r[5],
                StatBarLayer4 = r[5],
                StatBarLayer5 = r[7],

                Atk = r[8],
                Def = r[4],
                Fth = r[5],
                Agi = r[3],

                Vis = r[7],
                Ignis = r[6],
                Glacies = r[5],
                Fulgur = r[4],
                Ventus = r[4],
                Terra = r[3],
                Lux = r[8],
                Malum = r[7]
            };
        }

        #endregion
    }

    #endregion

    #region Colors

    #region General

    public ColorCode[] AllColors => [this.White, this.Black, this.TransBlack, this.Fg, this.Bg, this.Accent,
        this.Pos, this.Neg, this.Imp, this.Ally, this.Opp, this.Turn, this.Hp, this.Sp, this.Shield, this.Bloom,
        this.Buff, this.Skill, this.Element, this.Passive, this.Stat, this.Cooldown, this.SpBack, this.Overheal,
        this.StatBarLayer4, this.StatBarLayer5, this.Atk, this.Def, this.Fth, this.Agi, this.Vis, this.Ignis,
        this.Glacies, this.Fulgur, this.Ventus, this.Terra, this.Lux, this.Malum];

    /// <summary>
    /// Not necessarily actually white, but (probably) close
    /// </summary>
    public required ColorCode White { get; init; }

    /// <summary>
    /// Not necessarily actually black, but (probably) close
    /// </summary>
    public required ColorCode Black { get; init; }

    /// <summary>
    /// Partially transparent version of black
    /// </summary>
    public required ColorCode TransBlack { get; init; }

    /// <summary>
    /// Main foreground
    /// </summary>
    public required ColorCode Fg { get; init; }

    /// <summary>
    /// Main background
    /// </summary>
    public required ColorCode Bg { get; init; }

    /// <summary>
    /// Main accent
    /// </summary>
    public required ColorCode Accent { get; init; }

    /// <summary>
    /// Positive/good stuff
    /// </summary>
    public required ColorCode Pos { get; init; }

    /// <summary>
    /// Negative/bad stuff
    /// </summary>
    public required ColorCode Neg { get; init; }

    /// <summary>
    /// General important stuff that isn't neccessarily positive or negative
    /// </summary>
    public required ColorCode Imp { get; init; }

    #endregion

    #region Battle

    /// <summary>
    /// Ally names
    /// </summary>
    public required ColorCode Ally { get; init; }

    /// <summary>
    /// Opponent names
    /// </summary>
    public required ColorCode Opp { get; init; }

    /// <summary>
    /// Current turn text
    /// </summary>
    public required ColorCode Turn { get; init; }

    /// <summary>
    /// HP text and bar
    /// </summary>
    public required ColorCode Hp { get; init; }

    /// <summary>
    /// SP text and bar
    /// </summary>
    public required ColorCode Sp { get; init; }

    /// <summary>
    /// Shield text and bar
    /// </summary>
    public required ColorCode Shield { get; init; }

    /// <summary>
    /// Bloom text and bar
    /// </summary>
    public required ColorCode Bloom { get; init; }

    /// <summary>
    /// Buff names
    /// </summary>
    public required ColorCode Buff { get; init; }

    /// <summary>
    /// Skill names
    /// </summary>
    public required ColorCode Skill { get; init; }

    /// <summary>
    /// Element names
    /// </summary>
    public required ColorCode Element { get; init; }

    /// <summary>
    /// Passive names
    /// </summary>
    public required ColorCode Passive { get; init; }

    /// <summary>
    /// Stat names
    /// </summary>
    public required ColorCode Stat { get; init; }

    /// <summary>
    /// Cooldown text
    /// </summary>
    public required ColorCode Cooldown { get; init; }

    /// <summary>
    /// Back layer of SP bar
    /// </summary>
    public required ColorCode SpBack { get; init; }

    /// <summary>
    /// Overheal bar
    /// </summary>
    public required ColorCode Overheal { get; init; }

    /// <summary>
    /// 4th layer of stat bars (201-300%)
    /// </summary>
    public required ColorCode StatBarLayer4 { get; init; }

    /// <summary>
    /// 5th layer of stat bars (301-400%)
    /// </summary>
    public required ColorCode StatBarLayer5 { get; init; }

    #region StageTypes

    /// <summary>
    /// Atk stage
    /// </summary>
    public required ColorCode Atk { get; init; }

    /// <summary>
    /// Def stage
    /// </summary>
    public required ColorCode Def { get; init; }

    /// <summary>
    /// Fth stage
    /// </summary>
    public required ColorCode Fth { get; init; }

    /// <summary>
    /// Agi stage
    /// </summary>
    public required ColorCode Agi { get; init; }

    #endregion

    #region Elements

    /// <summary>
    /// Vis (neutral) element
    /// </summary>
    public required ColorCode Vis { get; init; }

    /// <summary>
    /// Ignis (fire) element
    /// </summary>
    public required ColorCode Ignis { get; init; }

    /// <summary>
    /// Glacies (ice) element
    /// </summary>
    public required ColorCode Glacies { get; init; }

    /// <summary>
    /// Fulgur (electric) element
    /// </summary>
    public required ColorCode Fulgur { get; init; }

    /// <summary>
    /// Ventus (wind) element
    /// </summary>
    public required ColorCode Ventus { get; init; }

    /// <summary>
    /// Terra (earth) element
    /// </summary>
    public required ColorCode Terra { get; init; }

    /// <summary>
    /// Lux (light) element
    /// </summary>
    public required ColorCode Lux { get; init; }

    /// <summary>
    /// Malum (evil) element
    /// </summary>
    public required ColorCode Malum { get; init; }

    #endregion

    #endregion

    #endregion

    public ColorCode Get(ThemeColor tc) => tc switch {
        ThemeColor.White => this.White,
        ThemeColor.Black => this.Black,
        ThemeColor.TransBlack => this.TransBlack,

        ThemeColor.Fg => this.Fg,
        ThemeColor.Bg => this.Bg,
        ThemeColor.Accent => this.Accent,

        ThemeColor.Pos => this.Pos,
        ThemeColor.Neg => this.Neg,
        ThemeColor.Imp => this.Imp,
        ThemeColor.Ally => this.Ally,
        ThemeColor.Opp => this.Opp,
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

    internal void _DrawPalette() {
        const int Size = 64;

        int y = -Size;
        for (int i = 0; i < this.AllColors.Length; i++) {
            int iMod = i % 8;
            int x = iMod * Size;
            if (iMod == 0) y += Size;
            Core.ShapeBatch.FillRectangle(new(x, y), new(Size, Size), this.AllColors[i]);
        }
    }

    internal static void _Change(Theme prevTheme, Theme newTheme) {
        _ChangeFSSColors(newTheme);
        Change?.Invoke(prevTheme, newTheme);
    }

    /// <summary>
    /// Add custom color aliases to FSS's text processing for the given palette
    /// </summary>
    // todo decide which colors should be part of this
    internal static void _ChangeFSSColors(Theme newTheme) {
        Dictionary<string, Color> colorMap = new() {
            ["white"] = newTheme.White,
            ["black"] = newTheme.Black,
            ["transBlack"] = newTheme.TransBlack,

            ["fg"] = newTheme.Fg,
            ["bg"] = newTheme.Bg,
            ["accent"] = newTheme.Accent,

            ["pos"] = newTheme.Pos,
            ["neg"] = newTheme.Neg,
            ["imp"] = newTheme.Imp,
            ["ally"] = newTheme.Ally,
            ["opp"] = newTheme.Opp,
            ["turn"] = newTheme.Turn,
            ["hp"] = newTheme.Hp,
            ["sp"] = newTheme.Sp,
            ["shield"] = newTheme.Shield,
            ["bloom"] = newTheme.Bloom,
            ["buff"] = newTheme.Buff,
            ["skill"] = newTheme.Skill,
            ["element"] = newTheme.Element,
            ["passive"] = newTheme.Passive,
            ["stat"] = newTheme.Stat,
            ["cooldown"] = newTheme.Cooldown,

            ["spBack"] = newTheme.SpBack,
            ["overheal"] = newTheme.Overheal,
            ["statBarLayer4"] = newTheme.StatBarLayer4,
            ["statBarLayer5"] = newTheme.StatBarLayer5,

            ["atk"] = newTheme.Atk,
            ["def"] = newTheme.Def,
            ["fth"] = newTheme.Fth,
            ["agi"] = newTheme.Agi,

            ["vis"] = newTheme.Vis,
            ["ignis"] = newTheme.Ignis,
            ["glacies"] = newTheme.Glacies,
            ["fulgur"] = newTheme.Fulgur,
            ["ventus"] = newTheme.Ventus,
            ["terra"] = newTheme.Terra,
            ["lux"] = newTheme.Lux,
            ["malum"] = newTheme.Malum,
        };

        foreach (KeyValuePair<string, Color> kvp in colorMap) {
            ColorStorage.Colors[kvp.Key] = new() { Color = kvp.Value };
        }
    }

    public string GetName(ThemeColor color, GameMod? mod = null) => color.Str() + this.KeyName.GetLang(mod);
    public string GetName(GameMod? mod = null) => this.GetName(ThemeColor.White, mod);
}
