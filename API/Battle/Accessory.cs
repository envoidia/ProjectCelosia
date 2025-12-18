using System.Collections.Generic;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class Accessory : ComplexDescribable, IEquippable {
    public Skill[] Skills { get; init; } = [];
    public Passive[] Passives { get; init; } = [];

    public Accessory(GameMod? source, string keyName, string icon) :
        base(source, keyName, icon, $"{keyName}Desc") {
        Core.Accessories.Add(this);
    }

    public override string GetFullDesc(GameMod? mod = null) =>
        string.Format(Lang.AccessoryDesc, this._GetFormattedDescInclusions(mod ?? this.Source));

    protected override HashSet<IDescribable> _GetDescInclusions() =>
       IEquippable.GetDescInclusions(this.DescInclusions, this.Skills, this.Passives);

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