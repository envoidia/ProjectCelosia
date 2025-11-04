using API.Entity;
using API.Graphics;

namespace API.Battle;

public class Element : IconEntity {
    public Mult? MultDmgDealt { get; }
    public Mult? MultDmgTaken { get; }

    public Element(string keyName, string keyDescription, string icon,
        Mult? multDmgDealt = null, Mult? multDmgTaken = null) : base(keyName, keyDescription, icon) {
        this.MultDmgDealt = multDmgDealt;
        this.MultDmgTaken = multDmgTaken;
        Core.Elements.Add(this);
    }

    public override int GetHashCode() => this.KeyName.GetHashCode();
}

public static class Elements {
    public static readonly Element Vis = new("ElementVis", "ElementVisDesc", "/c[lightGray]/i[rolling-energy]");

    public static readonly Element Ignis = new("ElementIgnis", "ElementIgnisDesc",
        "/c[orange]/i[fire]", Mults.IgnisDmgDealt, Mults.IgnisDmgTaken);

    public static readonly Element Glacies = new("ElementGlacies", "ElementGlaciesDesc",
        "/c[lightBlue]/i[snowflake-2]", Mults.GlaciesDmgDealt, Mults.GlaciesDmgTaken);

    public static readonly Element Fulgur = new("ElementFulgur", "ElementFulgurDesc",
        "/c[yellow]/i[electric]", Mults.FulgurDmgDealt, Mults.FulgurDmgTaken);

    public static readonly Element Ventus = new("ElementVentus", "ElementVentusDesc",
        "/c[green]/i[wind-slap]", Mults.VentusDmgDealt, Mults.VentusDmgTaken);

    public static readonly Element Terra = new("ElementTerra", "ElementTerraDesc",
        "/c[lightBrown]/i[rock]", Mults.TerraDmgDealt, Mults.TerraDmgTaken);

    public static readonly Element Lux = new("ElementLux", "ElementLuxDesc",
        $"{Colors.Lux}/i[todo]", Mults.LuxDmgDealt, Mults.LuxDmgTaken);

    public static readonly Element Malum = new("ElementMalum", "ElementMalumDesc",
        "/c[red]/i[evil-wings]", Mults.MalumDmgDealt, Mults.MalumDmgTaken);
}