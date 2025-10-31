using API.Entity;

namespace API.Battle;

public class Passive : ComplexDescriptionEntity {
    public IBuffEffect[] BuffEffects { get; }

    public Passive(string name, string description, string icon, params IBuffEffect[] buffEffects)
        : base(name, description, icon) {
        this.BuffEffects = buffEffects;
        Core.Passives.Add(this);
    }

    // todo getdescription
}