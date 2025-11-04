using API.Battle;
using API.Graphics;
using API.Modding;

namespace Celosia.Battle;

public static class Elements {
    public static Element Vis;
    public static Element Ignis;
    public static Element Glacies;
    public static Element Fulgur;
    public static Element Ventus;
    public static Element Terra;
    public static Element Lux;
    public static Element Malum;
    
    public static void Initialize(GameMod mod) {
        Vis = new Element(mod, "ElementVis", "ElementVisDesc", "/c[lightGray]/i[rolling-energy]");
        Ignis = new Element(mod, "ElementIgnis", "ElementIgnisDesc",
            "/c[orange]/i[fire]", Mults.IgnisDmgDealt, Mults.IgnisDmgTaken);
        Glacies = new Element(mod, "ElementGlacies", "ElementGlaciesDesc",
            "/c[lightBlue]/i[snowflake-2]", Mults.GlaciesDmgDealt, Mults.GlaciesDmgTaken);
        Fulgur = new Element(mod, "ElementFulgur", "ElementFulgurDesc",
            "/c[yellow]/i[electric]", Mults.FulgurDmgDealt, Mults.FulgurDmgTaken);
        Ventus = new Element(mod, "ElementVentus", "ElementVentusDesc",
            "/c[green]/i[wind-slap]", Mults.VentusDmgDealt, Mults.VentusDmgTaken);
        Terra = new Element(mod, "ElementTerra", "ElementTerraDesc",
            "/c[lightBrown]/i[rock]", Mults.TerraDmgDealt, Mults.TerraDmgTaken);
        Lux = new Element(mod, "ElementLux", "ElementLuxDesc",
            $"{Colors.Lux}/i[todo]", Mults.LuxDmgDealt, Mults.LuxDmgTaken);
        Malum = new Element(mod, "ElementMalum", "ElementMalumDesc",
            "/c[red]/i[evil-wings]", Mults.MalumDmgDealt, Mults.MalumDmgTaken);
    }
}