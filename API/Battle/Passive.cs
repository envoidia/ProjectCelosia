using API.Battle.BuffEffects;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class Passive : ComplexDescribable, _IModItem {
    public IBuffEffect[] BuffEffects { get; init; }

    public GameMod? Source { get; }

    public Passive(GameMod? source, string keyName, string keyDesc, string icon, params IBuffEffect[] buffEffects)
        : base(keyName, icon, keyDesc) {
        this.Source = source;
        this.BuffEffects = buffEffects;
        Core.Passives.Add(this);
    }

    public override string GetName(GameMod? mod = null) => this.GetName(ThemeColor.Passive, mod);

    public override string GetFullDesc(GameMod? mod = null) => "todo";
}