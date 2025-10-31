using API.Entity;

namespace API.Battle;

public class Accessory : ComplexDescriptionEntity, IEquippable {
    public Skill[] Skills { get; init; }
    public Passive[] Passives { get; init; }

    public override string Description => string.Format(Lang.AccessoryDesc, this.GetPartialDesc());

    public Accessory(string name, string description, string icon) : base(name, description, icon) {
        Core.Accessories.Add(this);
    }

    public void Apply(Unit unit, bool give) {
        if (give) {
            unit.AddSkills(this.Skills);
            unit.AddPassives(this.Passives);
        } else {
            unit.RemoveSkills(this.Skills);
            unit.RemovePassives(this.Passives);
        }
    }
}