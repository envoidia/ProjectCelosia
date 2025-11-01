using API.Entity;
using API.Graphics;

namespace API.Battle;

public class Element : IconEntity {
    public Element(string keyName, string keyDescription, string icon) : base(keyName, keyDescription, icon) {
        Core.Elements.Add(this);
    }

    public override int GetHashCode() => this.KeyName.GetHashCode();
}

public static class Elements {
    public static readonly Element Vis = new("ElementVis", "ElementVisDesc", "/c[lightGray]/i[rolling-energy]");
    public static readonly Element Ignis = new("ElementIgnis", "ElementIgnisDesc", "/c[orange]/i[fire]");

    public static readonly Element
        Glacies = new("ElementGlacies", "ElementGlaciesDesc", "/c[lightBlue]/i[snowflake-2]");

    public static readonly Element Fulgur = new("ElementFulgur", "ElementFulgurDesc", "/c[yellow]/i[electric]");
    public static readonly Element Ventus = new("ElementVentus", "ElementVentusDesc", "/c[green]/i[wind-slap]");
    public static readonly Element Terra = new("ElementTerra", "ElementTerraDesc", "/c[lightBrown]/i[rock]");
    public static readonly Element Lux = new("ElementLux", "ElementLuxDesc", $"{Colors.Lux}/i[todo]");
    public static readonly Element Malum = new("ElementMalum", "ElementMalumDesc", "/c[red]/i[evil-wings]");
}