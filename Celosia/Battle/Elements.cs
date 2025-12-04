using API.Battle;
using API.Graphics;

namespace Celosia.Battle;

public static class Elements {
    public static readonly Element Ignis = new(Main.Mod, "ElementIgnis",
        "ElementIgnisDesc", "/c[orange]/i[fire]", Mults.IgnisDmgDealt, Mults.IgnisDmgTaken);

    public static readonly Element Glacies = new(Main.Mod, "ElementGlacies",
        "ElementGlaciesDesc",
        "/c[lightBlue]/i[snowflake-2]", Mults.GlaciesDmgDealt, Mults.GlaciesDmgTaken);

    public static readonly Element Fulgur = new(Main.Mod, "ElementFulgur",
        "ElementFulgurDesc",
        "/c[yellow]/i[electric]", Mults.FulgurDmgDealt, Mults.FulgurDmgTaken);

    public static readonly Element Ventus = new(Main.Mod, "ElementVentus",
        "ElementVentusDesc",
        "/c[green]/i[wind-slap]", Mults.VentusDmgDealt, Mults.VentusDmgTaken);

    public static readonly Element Terra = new(Main.Mod, "ElementTerra",
        "ElementTerraDesc",
        "/c[lightBrown]/i[rock]", Mults.TerraDmgDealt, Mults.TerraDmgTaken);

    public static readonly Element Lux = new(Main.Mod, "ElementLux",
        "ElementLuxDesc",
        $"{Colors.Lux}/i[todo]", Mults.LuxDmgDealt, Mults.LuxDmgTaken);

    public static readonly Element Malum = new(Main.Mod, "ElementMalum",
        "ElementMalumDesc",
        "/c[red]/i[evil-wings]", Mults.MalumDmgDealt, Mults.MalumDmgTaken);
}