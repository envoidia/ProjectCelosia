using API.Graphics;

namespace API.Battle;

public class Elements {
    public static readonly Element Vis = new(Lang.ElementVis, Lang.ElementVisDesc, "/c[lightGray]/i[rolling-energy]");
    public static readonly Element Ignis = new(Lang.ElementIgnis, Lang.ElementIgnisDesc, "/c[orange]/i[fire]");

    public static readonly Element Glacies = new(Lang.ElementGlacies, Lang.ElementGlaciesDesc,
        "/c[lightBlue]/i[snowflake-2]");

    public static readonly Element Fulgur = new(Lang.ElementFulgur, Lang.ElementFulgurDesc, "/c[yellow]/i[electric]");
    public static readonly Element Ventus = new(Lang.ElementVentus, Lang.ElementVentusDesc, "/c[green]/i[wind-slap]");
    public static readonly Element Terra = new(Lang.ElementTerra, Lang.ElementTerraDesc, "/c[lightBrown]/i[rock]");
    public static readonly Element Lux = new(Lang.ElementLux, Lang.ElementLuxDesc, $"{Colors.Lux}/i[todo]");
    public static readonly Element Malum = new(Lang.ElementMalum, Lang.ElementMalumDesc, "/c[red]/i[evil-wings]");
}