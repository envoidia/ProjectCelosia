using API.Battle;
using API.Graphics;

namespace Celosia.Battle;

public static class Elements {
    public static readonly Element Ignis = new(Main.Id, "ElementIgnis",
        $"{ThemeColor.Ignis.Str()}/i[fire]", Mults.IgnisDmgDealt, Mults.IgnisDmgTaken);

    public static readonly Element Glacies = new(Main.Id, "ElementGlacies",
        $"{ThemeColor.Glacies.Str()}/i[snowflake-2]", Mults.GlaciesDmgDealt, Mults.GlaciesDmgTaken);

    public static readonly Element Fulgur = new(Main.Id, "ElementFulgur",
        $"{ThemeColor.Fulgur.Str()}/i[electric]", Mults.FulgurDmgDealt, Mults.FulgurDmgTaken);

    public static readonly Element Ventus = new(Main.Id, "ElementVentus",
        $"{ThemeColor.Ventus.Str()}/i[wind-slap]", Mults.VentusDmgDealt, Mults.VentusDmgTaken);

    public static readonly Element Terra = new(Main.Id, "ElementTerra",
        $"{ThemeColor.Terra.Str()}/i[rock]", Mults.TerraDmgDealt, Mults.TerraDmgTaken);

    public static readonly Element Lux = new(Main.Id, "ElementLux",
        $"{ThemeColor.Lux.Str()}/i[sparkles]", Mults.LuxDmgDealt, Mults.LuxDmgTaken);

    public static readonly Element Malum = new(Main.Id, "ElementMalum",
        $"{ThemeColor.Malum.Str()}/i[evil-wings]", Mults.MalumDmgDealt, Mults.MalumDmgTaken);
}