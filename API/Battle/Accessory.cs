using System.Collections.Generic;
using API.Entity;
using API.Modding;

namespace API.Battle;

public class Accessory : ComplexDescriptionEntity, IEquippable, IModItem {
    public Skill[] Skills { get; init; } = [];
    public Passive[] Passives { get; init; } = [];

    public IGameMod? Source { get; }

    public Accessory(IGameMod? source, string keyName, string keyDescription, string icon) : base(keyName,
        keyDescription, icon) {
        this.Source = source;
        Core.Accessories.Add(this);
    }

    protected override HashSet<DescriptionEntity> GetDescriptionInclusions() {
        HashSet<DescriptionEntity> inclusions = new(this.DescriptionInclusions);

        inclusions.UnionWith(this.Skills);
        inclusions.UnionWith(this.Passives);

        return inclusions;
    }

    public override string GetDescriptionWithInclusions(IGameMod? mod = null) =>
        string.Format(Lang.AccessoryDesc, this.GetFormattedDescriptionInclusions(mod));

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