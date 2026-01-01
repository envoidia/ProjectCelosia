using System.Collections.Generic;
using API.Extensions;
using API.Modding;
using API.Name;

namespace API.Battle;

/// <summary>
/// todo docs
/// </summary>
public sealed class Accessory : ComplexDescribable, IRegistrable, IEquippable
{
    public Skill[] Skills { get; init; } = [];
    public Passive[] Passives { get; init; } = [];

    public string ItemId { get; init; }

    public Accessory(string modId, string keyName, string icon, string? itemId = null)
        : base(keyName, icon, $"{keyName}Desc")
    {
        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
    }

    public override string GetFullDesc()
    {
        return "AccessoryDesc".FormatLang(this._GetFormattedDescInclusions());
    }

    protected override HashSet<IDescribable> _GetDescInclusions()
    {
        return IEquippable.GetDescInclusions(this.DescInclusions, this.Skills, this.Passives);
    }

    public void Apply(Unit unit, bool give)
    {
        if (give)
        {
            unit.AddSkills(this.Skills);
            unit.AddPassives(this.Passives);
        } else
        {
            unit.RemoveSkills(this.Skills);
            unit.RemovePassives(this.Passives);
        }
    }
}