using API.Entity;
using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class Element : _IModItem, INameable {
    public Mult? MultDmgDealt { get; }
    public Mult? MultDmgTaken { get; }

    public GameMod? Source { get; }
    public string KeyName { get; }
    public string Icon { get; }

    public Element(GameMod? source, string keyName, string icon,
        Mult? multDmgDealt = null, Mult? multDmgTaken = null) {
        this.Source = source;
        this.KeyName = keyName;
        this.Icon = icon;

        this.MultDmgDealt = multDmgDealt;
        this.MultDmgTaken = multDmgTaken;

        Core.Elements.Add(this);
    }

    public string GetName(string color = Colors.Element, GameMod? mod = null) =>
        $"{this.Icon} {color}{this.KeyName.GetLang(mod)}";
}

public static class Elements {
    public static readonly Element Vis = new(null, "ElementVis", "/c[lightGray]/i[rolling-energy]");
}