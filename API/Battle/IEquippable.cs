using System.Collections.Generic;
using API.Name;

namespace API.Battle;

/// <summary>
/// An equippable item
/// </summary>
public interface IEquippable : IDescribable
{
    void Apply(Unit unit, bool giving);

    void Equip(Unit unit)
    {
        this.Apply(unit, true);
    }

    void Unequip(Unit unit)
    {
        this.Apply(unit, false);
    }

    static HashSet<IDescribable> GetDescInclusions(HashSet<IDescribable> inclusions, Skill[] skills, Passive[] passives)
    {
        HashSet<IDescribable> inclusionsCopy = [.. inclusions];

        inclusionsCopy.UnionWith(skills);
        inclusionsCopy.UnionWith(passives);

        return inclusionsCopy;
    }

}