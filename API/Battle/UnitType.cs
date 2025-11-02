using System.Collections.Generic;
using API.Entity;

namespace API.Battle;

// todo represent available and equipped skills and equipped item
public class UnitType : DescriptionEntity {
    public Dictionary<Stat, uint> Stats { get; }
    internal readonly Dictionary<Element, int> _affinities;
    public Passive[] Passives { get; }

    public UnitType(string keyName, string keyDescription, Dictionary<Stat, uint> stats,
        Dictionary<Element, int> affinities, params Passive[] passives) : base(keyName, keyDescription) {
        this.Stats = stats;
        this._affinities = affinities;
        this.Passives = passives;
        Core.UnitTypes.Add(this);
    }

    public string FormatName(uint pos, bool possessive) => "todo";
    public string FormatName(uint pos) => this.FormatName(pos, true);
}