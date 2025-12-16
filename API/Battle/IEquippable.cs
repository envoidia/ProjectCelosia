using System.Collections.Generic;
using API.Name;

namespace API.Battle;

/// <summary>
/// An equippable item. Must also be IDescribable
/// </summary>
public interface IEquippable {
    void Apply(Unit unit, bool giving);

    void Equip(Unit unit) => this.Apply(unit, true);

    void Unequip(Unit unit) => this.Apply(unit, false);

    static HashSet<IDescribable> GetDescInclusions(HashSet<IDescribable> inclusions, Skill[] skills, Passive[] passives) {
        HashSet<IDescribable> inclusionsCopy = [.. inclusions];

        inclusionsCopy.UnionWith(skills);
        inclusionsCopy.UnionWith(passives);

        return inclusionsCopy;
    }
    
}