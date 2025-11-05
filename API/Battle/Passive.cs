using API.Battle.BuffEffects;
using API.Entity;
using API.Modding;

namespace API.Battle;

public class Passive : ComplexDescriptionEntity, IModItem {
    public IBuffEffect[] BuffEffects { get; init; }

    public GameMod? Source { get; }

    public Passive(GameMod? source, string keyName, string keyDescription, string icon,
        params IBuffEffect[] buffEffects)
        : base(keyName, keyDescription, icon) {
        this.Source = source;
        this.BuffEffects = buffEffects;
        Core.Passives.Add(this);
    }

    public override string GetDescription() => "todo";
}