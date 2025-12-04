using System.Collections.Frozen;
using System.Collections.Generic;
using API.Entity;
using API.Extensions;
using API.Modding;

namespace API.Battle;

public sealed class Weapon : ComplexDescriptionEntity, IEquippable {
    public required FrozenDictionary<Element, int> Affinities { get; init; }

    public Skill[] Skills { get; init; } = [];
    public Passive[] Passives { get; init; } = [];

    public Weapon(string keyName, string keyDescription, string icon) : base(keyName, keyDescription, icon) {
        Core.Weapons.Add(this);
    }

    protected override HashSet<DescriptionEntity> GetDescriptionInclusions() {
        HashSet<DescriptionEntity> inclusions = [.. this.DescriptionInclusions];

        inclusions.UnionWith(this.Skills);
        inclusions.UnionWith(this.Passives);

        return inclusions;
    }

    public override string GetDescriptionWithInclusions(GameMod? mod = null) =>
        string.Format(Lang.WeaponDesc, this.GetFormattedDescriptionInclusions(mod));

    public void Apply(Unit unit, bool give) {
        int multiplier = give.ToSign();

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