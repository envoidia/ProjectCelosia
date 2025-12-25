using System.Collections.Generic;
using API.Extensions;
using API.Modding;
using API.Name;

namespace API.Battle;

/// <summary>
/// todo docs
/// </summary>
public sealed class Weapon : ComplexDescribable, IRegistrable, IEquippable {
    public Dictionary<Element, int> Affinities { get; init; }
    public Skill[] Skills { get; init; } = [];
    public Passive[] Passives { get; init; } = [];

    public string ItemId { get; init; }

    public Weapon(string modId, string keyName, string icon, Dictionary<Element, int> affinities, string? itemId = null)
        : base(keyName, icon, $"{keyName}Desc") {
        this.Affinities = affinities;

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
    }

    public override string GetFullDesc() =>
        "WeaponDesc".FormatLang(this._GetFormattedDescInclusions());

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