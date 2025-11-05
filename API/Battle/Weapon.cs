using System.Collections.Frozen;
using API.Entity;

namespace API.Battle;

public class Weapon : ComplexDescriptionEntity, IEquippable {
    public required FrozenDictionary<Element, int> Affinities { get; init; }

    public Skill[] Skills { get; init; } = [];
    public Passive[] Passives { get; init; } = [];

    public Weapon(string keyName, string keyDescription, string icon) : base(keyName, keyDescription, icon) {
        Core.Weapons.Add(this);
    }

    public override string GetDescription() => string.Format(Lang.WeaponDesc, this.GetPartialDescription());

    public void Apply(Unit unit, bool give) {
        int multiplier = give ? 1 : -1;

        // Merge affinity maps
        foreach ((Element element, int value) in this.Affinities) {
            unit.SetAffinity(element, unit.GetAffinity(element) + (value * multiplier));
        }

        if (give) {
            unit.AddSkills(this.Skills);
            unit.AddPassives(this.Passives);
        } else {
            unit.RemoveSkills(this.Skills);
            unit.RemovePassives(this.Passives);
        }
    }
}