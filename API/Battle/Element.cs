using API.Entity;
using API.Graphics;
using API.Modding;

namespace API.Battle;

public sealed class Element : IconEntity, _IModItem {
    public Mult? MultDmgDealt { get; }
    public Mult? MultDmgTaken { get; }

    public GameMod? Source { get; }

    public Element(GameMod? source, string keyName, string keyDescription, string icon,
        Mult? multDmgDealt = null, Mult? multDmgTaken = null) : base(keyName, keyDescription, icon) {
        this.Source = source;
        this.MultDmgDealt = multDmgDealt;
        this.MultDmgTaken = multDmgTaken;
        Core.Elements.Add(this);
    }

    public override string GetName(GameMod? mod = null) => this.GetName(Colors.Element);
}

public static class Elements {
    public static readonly Element Vis = new(null, "ElementVis", "ElementVisDesc",
        "/c[lightGray]/i[rolling-energy]");
}