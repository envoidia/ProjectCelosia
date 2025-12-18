using API.Battle;
using API.Graphics;

namespace Celosia.Battle;

public static class Elements {
    public static readonly Element Ignis = new(Main.Mod, "ElementIgnis",
        $"{new ColorCode(Colors.RedOranges[5])}/i[fire]", Mults.IgnisDmgDealt, Mults.IgnisDmgTaken);

    public static readonly Element Glacies = new(Main.Mod, "ElementGlacies",
        $"{new ColorCode(Colors.Blues[4])}/i[snowflake-2]", Mults.GlaciesDmgDealt, Mults.GlaciesDmgTaken);

    public static readonly Element Fulgur = new(Main.Mod, "ElementFulgur",
        $"{new ColorCode(Colors.Oranges[5])}/i[electric]", Mults.FulgurDmgDealt, Mults.FulgurDmgTaken);

    public static readonly Element Ventus = new(Main.Mod, "ElementVentus",
        $"{new ColorCode(Colors.Greens[2])}/i[wind-slap]", Mults.VentusDmgDealt, Mults.VentusDmgTaken);

    public static readonly Element Terra = new(Main.Mod, "ElementTerra",
        $"{new ColorCode(Colors.Oranges[2])}/i[rock]", Mults.TerraDmgDealt, Mults.TerraDmgTaken);

    public static readonly Element Lux = new(Main.Mod, "ElementLux",
        $"{new ColorCode(Colors.Beiges[5])}/i[sparkles]", Mults.LuxDmgDealt, Mults.LuxDmgTaken);

    public static readonly Element Malum = new(Main.Mod, "ElementMalum",
        $"{new ColorCode(Colors.RedOranges[4])}/i[evil-wings]", Mults.MalumDmgDealt, Mults.MalumDmgTaken);
}