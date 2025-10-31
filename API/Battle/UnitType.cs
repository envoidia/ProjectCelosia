using System.Collections.Generic;
using API.Entity;

namespace API.Battle;

public class UnitType : NamedEntity {
    public Stats StatsBase { get; }
    public Dictionary<Element, int> AffinitiesBase { get; }
    public Passive[] Passives { get; }

    public UnitType(string name, string description, Stats statsBase, Dictionary<Element, int> affinitiesBase,
        params Passive[] passives) : base(name, description) {
        this.StatsBase = statsBase;
        this.AffinitiesBase = affinitiesBase;
        this.Passives = passives;
        Core.UnitTypes.Add(this);
    }

    public string FormatName(uint pos, bool possessive) => "todo";
}