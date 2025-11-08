using API.Battle;
using API.Graphics;

namespace Celosia.Battle;

public static class Elements {
    public static readonly Element Ignis = new(Main.ModInstance, "ElementIgnis",
        "ElementIgnisDesc", "/c[orange]/i[fire]", Mults.IgnisDmgDealt, Mults.IgnisDmgTaken);

    public static readonly Element Glacies = new(Main.ModInstance, "ElementGlacies",
        "ElementGlaciesDesc",
        "/c[lightBlue]/i[snowflake-2]", Mults.GlaciesDmgDealt, Mults.GlaciesDmgTaken);

    public static readonly Element Fulgur = new(Main.ModInstance, "ElementFulgur",
        "ElementFulgurDesc",
        "/c[yellow]/i[electric]", Mults.FulgurDmgDealt, Mults.FulgurDmgTaken);

    public static readonly Element Ventus = new(Main.ModInstance, "ElementVentus",
        "ElementVentusDesc",
        "/c[green]/i[wind-slap]", Mults.VentusDmgDealt, Mults.VentusDmgTaken);

    public static readonly Element Terra = new(Main.ModInstance, "ElementTerra",
        "ElementTerraDesc",
        "/c[lightBrown]/i[rock]", Mults.TerraDmgDealt, Mults.TerraDmgTaken);

    public static readonly Element Lux = new(Main.ModInstance, "ElementLux",
        "ElementLuxDesc",
        $"{Colors.Lux}/i[todo]", Mults.LuxDmgDealt, Mults.LuxDmgTaken);

    public static readonly Element Malum = new(Main.ModInstance, "ElementMalum",
        "ElementMalumDesc",
        "/c[red]/i[evil-wings]", Mults.MalumDmgDealt, Mults.MalumDmgTaken);
}