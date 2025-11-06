using System.Collections.Generic;
using API.Entity;
using API.Modding;

namespace API.Battle;

// todo represent available and equipped skills and equipped item
public class UnitType : DescriptionEntity, IModItem {
    public Dictionary<Stat, uint> Stats { get; }
    internal readonly Dictionary<Element, int> _affinities;
    public Passive[] Passives { get; }

    public IGameMod? Source { get; }

    public UnitType(IGameMod? source, string keyName, string keyDescription, Dictionary<Stat, uint> stats,
        Dictionary<Element, int> affinities, params Passive[] passives) : base(keyName, keyDescription) {
        this.Source = source;
        this.Stats = stats;
        this._affinities = affinities;
        this.Passives = passives;
        Core.UnitTypes.Add(this);
    }
}