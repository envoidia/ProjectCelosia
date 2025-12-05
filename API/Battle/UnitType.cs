using System.Collections.Generic;
using API.Entity;
using API.Modding;

namespace API.Battle;

// todo represent available and equipped skills and equipped item
public sealed class UnitType : DescriptionEntity, IModItem {

    public Dictionary<Stat, int> Stats { get; }
    internal readonly Dictionary<Element, int> _Affinities; // todo change naming rule
    public Passive[] Passives { get; }

    public GameMod? Source { get; }

    public UnitType(GameMod? source, string keyName, string keyDescription, Dictionary<Stat, int> stats,
        Dictionary<Element, int> affinities, params Passive[] passives) : base(keyName, keyDescription) {
        this.Source = source;
        this.Stats = stats;
        this._Affinities = affinities;
        this.Passives = passives;
        Core.UnitTypes.Add(this);
    }
}

public static class UnitTypes {
    public static readonly UnitType TestUnitType = new(null, "TestUnitType", "Todo", new Dictionary<Stat, int>() {
        [Stats.Hp] = 100, [Stats.Str] = 100, [Stats.Mag] = 100, [Stats.Fth] = 100, [Stats.Amr] = 100, [Stats.Res] = 100, [Stats.Agi] = 100
    }, []);
}