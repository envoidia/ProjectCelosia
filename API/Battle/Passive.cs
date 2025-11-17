using API.Battle.BuffEffects;
using API.Entity;
using API.Graphics;
using API.Modding;

namespace API.Battle;

public sealed class Passive : ComplexDescriptionEntity, IModItem {
    public IBuffEffect[] BuffEffects { get; init; }

    public IGameMod? Source { get; }

    public Passive(IGameMod? source, string keyName, string keyDescription, string icon,
        params IBuffEffect[] buffEffects)
        : base(keyName, keyDescription, icon) {
        this.Source = source;
        this.BuffEffects = buffEffects;
        Core.Passives.Add(this);
    }

    public override string GetName(IGameMod? mod = null) => this.GetName(Colors.Passive);

    public override string GetDescriptionWithInclusions(IGameMod? mod = null) => "todo";
}