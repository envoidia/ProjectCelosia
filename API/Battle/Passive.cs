using API.Battle.BuffEffects;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class Passive : ComplexDescribable, _IModItem {
    public IBuffEffect[] BuffEffects { get; init; }

    public GameMod? Source { get; }

    public Passive(GameMod? source, string keyName, string keyDescription, string icon,
        params IBuffEffect[] buffEffects)
        : base(keyName, keyDescription, icon) {
        this.Source = source;
        this.BuffEffects = buffEffects;
        Core.Passives.Add(this);
    }

    public override string GetName(GameMod? mod = null) => this.GetName(Colors.Passive, mod);

    public override string GetFullDesc(GameMod? mod = null) => "todo";
}