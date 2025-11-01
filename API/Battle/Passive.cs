using API.Entity;

namespace API.Battle;

public class Passive : ComplexDescriptionEntity {
    public IBuffEffect[] BuffEffects { get; }

    public Passive(string keyName, string keyDescription, string icon, params IBuffEffect[] buffEffects)
        : base(keyName, keyDescription, icon) {
        this.BuffEffects = buffEffects;
        Core.Passives.Add(this);
    }

    // todo getdescription
}