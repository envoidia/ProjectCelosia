using API.Battle.BuffEffects;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class Passive : ComplexDescribable{
    public IBuffEffect[] BuffEffects { get; init; }

    public Passive(GameMod? source, string keyName, string keyDesc, string icon, params IBuffEffect[] buffEffects)
        : base(source, keyName, icon, keyDesc) {
        this.BuffEffects = buffEffects;
        Core.Passives.Add(this);
    }

    public override string GetName(GameMod? mod = null) => this.GetName(ThemeColor.Passive, mod ?? this.Source);

    public override string GetFullDesc(GameMod? mod = null) => "todo";
}