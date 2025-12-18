using System.Collections.Generic;
using API.Extensions;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class Weapon : ComplexDescribable, IEquippable {
    public Dictionary<Element, int> Affinities { get; init; }
    public Skill[] Skills { get; init; } = [];
    public Passive[] Passives { get; init; } = [];

    public Weapon(GameMod source, string keyName, string icon, Dictionary<Element, int> affinities)
        : base(source, keyName, icon, $"{keyName}Desc") {
        this.Affinities = affinities;
        Core.Weapons.Add(this);
    }

    public override string GetFullDesc(GameMod? mod = null) =>
        string.Format(Lang.WeaponDesc, this._GetFormattedDescInclusions(mod ?? this.Source));

    protected override HashSet<IDescribable> _GetDescInclusions() =>
        IEquippable.GetDescInclusions(this.DescInclusions, this.Skills, this.Passives);

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