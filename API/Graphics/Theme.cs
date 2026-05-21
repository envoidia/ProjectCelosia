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

    private const string _SeleneAbyssId = $"{Core.Id}:ThemeSeleneAbyss";
    public static Theme SeleneAbyss { get; set; } = null!;

    public Theme(string modId, string keyName, string? keyDesc = null, string? itemId = null)
    {
        this.KeyName = keyName;
        this.KeyDesc = keyDesc ?? $"{keyName}Desc";

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
    }

    internal static void _Init()
    {
        _LoadThemes();
    }

    private static void _LoadThemes()
    {
        string[] themeFiles = _TryRegenerateSeleneAbyss(_LoadThemeFiles());

        foreach (string path in themeFiles)
        {
            _LoadTheme(path);
        }

        IRegistrable? seleneAbyss = Registry.Get(_SeleneAbyssId);
        Assert.NotNull(seleneAbyss);
        SeleneAbyss = (Theme) seleneAbyss!;
    }

    private static void _LoadTheme(string path)
    {
        Exception? parseError = Properties.TryParse(path, out Dictionary<string, string>? themeDict);

        if (parseError is not null)
        {
            Core._LogOrEarlyLog($"Failed to read theme file at {path}: {parseError.Message}",
                nameof(_LoadTheme), LogLevel.Err);
        }

        Assert.NotNull(themeDict);

        themeDict!.TryGetValue("Name", out string? name);
        themeDict.TryGetValue("Desc", out string? desc);

        bool useLangKey;

        if (themeDict.TryGetValue(nameof(UseLangKey), out string? useLangKeyStr))
        {
            if (!bool.TryParse(useLangKeyStr, out bool useLangKeyInner))
            {
                Core._LogOrEarlyLog("\"UseLangKey\" is present but invalid. Defaulting to false",
                    nameof(_LoadTheme), LogLevel.Err);
            }

            useLangKey = useLangKeyInner;
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
                    nameof(_LoadTheme), LogLevel.Err);
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
                    nameof(_LoadTheme), LogLevel.Err);
                return Color.Red;
            }
        }
    }

    private static string[] _LoadThemeFiles()
    {
        try
        {
            return Directory.GetFiles("Themes");
        }
        catch (Exception e)
        {
            Core._LogOrEarlyLog($"Failed to read theme files: {e.Message}",
                nameof(_LoadThemes), LogLevel.Err);
            return [];
        }
    }

    private static string[] _TryRegenerateSeleneAbyss(string[] themeFiles)
    {
        const string SeleneAbyssPath = "Themes/SeleneAbyss.properties";

        if (File.Exists(SeleneAbyssPath))
        {
            return themeFiles;
        }

        Core._LogOrEarlyLog($"File {SeleneAbyssPath} is missing, regenerating",
            nameof(_LoadThemes), LogLevel.Err);

        try
        {
            Directory.CreateDirectory("Themes");

            File.WriteAllText(SeleneAbyssPath, """
                Name=SeleneAbyss
                Desc=SeleneAbyss
                UseLangKey=true

                Fg=#cccac2
                Midtone=#9a9a98
                Bg=#161b29
                BgSecondary=#141721
                BgTrans=#161b29a0

                Accent=#24556f
                AccentDeemphasized=#253a5e

                Positive=#4fc1a5
                Negative=#f7768e
                Emphasis=#e9aee4
                Ally=#6798ff
                Opponent=#ff899d
                Turn=#a673d3
                Hp=#76b668
                Sp=#a673d3
                Shield=#66c8cc
                Bloom=#e48cfa
                Buff=#32bb99
                Skill=#d3c4f3
                Element=#d3c4f3
                Passive=#32bb99
                Stat=#4ab0e7
                Reticle=#ff0000
                Cooldown=#6798ff

                SpBack=#b5c0ff
                Overheal=#e48cfa
                StatBarLayer4=#6798ff
                StatBarLayer5=#c0caf5

                Atk=#f7768e
                Def=#6798ff
                Fth=#a673d3
                Agi=#32bb99

                Vis=#9a9a98
                Ignis=#ff899d
                Glacies=#8db0ff
                Fulgur=#dcdcaa
                Ventus=#31d475
                Terra=#765a29
                Lux=#c0caf5
                Malum=#662e2d
                """);
        }
        catch (Exception e)
        {
            // todo how do i even handle this scenario properly
            Core._LogOrEarlyLog($"Default theme ({_SeleneAbyssId}) was missing, failed to recreate: {e.Message}",
                nameof(_LoadThemes), LogLevel.Err);
            throw e; // i dont like this
        }

        return _LoadThemeFiles();
    }

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
