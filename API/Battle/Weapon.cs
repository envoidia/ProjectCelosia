using System.Collections.Frozen;
using System.Collections.Generic;
using API.Extensions;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class Weapon : ComplexDescribable, IEquippable {
    public Dictionary<Element, int> Affinities { get; init; }
    public Skill[] Skills { get; init; } = [];
    public Passive[] Passives { get; init; } = [];

    public Weapon(string keyName, string keyDesc, string icon, Dictionary<Element, int> affinities) : base(keyName, icon, keyDesc) {
        this.Affinities = affinities;
        Core.Weapons.Add(this);
    }

    public override string GetFullDesc(GameMod? mod = null) =>
        string.Format(Lang.WeaponDesc, this._GetFormattedDescInclusions(mod));

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