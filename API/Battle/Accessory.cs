using API.Entity;

namespace API.Battle;

public class Accessory : ComplexDescriptionEntity, IEquippable {
    public Skill[] Skills { get; init; } = [];
    public Passive[] Passives { get; init; } = [];

    // todo this should be a key as the name implies
    public override string KeyDescription => string.Format(Lang.AccessoryDesc, this.GetPartialDesc());

    public Accessory(string keyName, string keyDescription, string icon) : base(keyName, keyDescription, icon) {
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