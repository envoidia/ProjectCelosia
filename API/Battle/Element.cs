using API.Entity;
using API.Modding;

namespace API.Battle;

public class Element : IconEntity, IModItem {
    public GameMod? Source { get; }

    public Mult? MultDmgDealt { get; }
    public Mult? MultDmgTaken { get; }

    public Element(GameMod? source, string keyName, string keyDescription, string icon,
        Mult? multDmgDealt = null, Mult? multDmgTaken = null) : base(keyName, keyDescription, icon) {
        this.Source = source;
        this.MultDmgDealt = multDmgDealt;
        this.MultDmgTaken = multDmgTaken;
        Core.Elements.Add(this);
    }

    public override int GetHashCode() => this.KeyName.GetHashCode();
}