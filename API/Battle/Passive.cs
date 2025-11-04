using API.Battle.BuffEffects;
using API.Entity;

namespace API.Battle;

public class Passive : ComplexDescriptionEntity {
    public IBuffEffect[] BuffEffects { get; init; }

    public Passive(string keyName, string keyDescription, string icon, params IBuffEffect[] buffEffects)
        : base(keyName, keyDescription, icon) {
        this.BuffEffects = buffEffects;
        Core.Passives.Add(this);
    }

    public override string GetDescription() => "todo";
}