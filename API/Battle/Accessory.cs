using System.Collections.Generic;
using API.Entity;
using API.Modding;

namespace API.Battle;

public sealed class Accessory : ComplexDescriptionEntity, IEquippable, _IModItem {
    public Skill[] Skills { get; init; } = [];
    public Passive[] Passives { get; init; } = [];

    public GameMod? Source { get; }

    public Accessory(GameMod? source, string keyName, string keyDescription, string icon) : base(keyName,
        keyDescription, icon) {
        this.Source = source;
        Core.Accessories.Add(this);
    }

    protected override HashSet<DescriptionEntity> _GetDescriptionInclusions() {
        HashSet<DescriptionEntity> inclusions = [.. this.DescriptionInclusions];

        inclusions.UnionWith(this.Skills);
        inclusions.UnionWith(this.Passives);

        return inclusions;
    }

    public override string GetDescriptionWithInclusions(GameMod? mod = null) =>
        string.Format(Lang.AccessoryDesc, this._GetFormattedDescriptionInclusions(mod));

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