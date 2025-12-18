using System.Collections.Generic;
using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

// todo represent available and equipped skills and equipped item
public sealed class UnitType : _IModItem, IDescribable {

    public Dictionary<Stat, int> Stats { get; }
    internal readonly Dictionary<Element, int> _Affinities; // todo change naming rule
    public Passive[] Passives { get; }

    public GameMod? Source { get; }
    public string KeyName { get; }
    public string KeyDesc { get; }

    public UnitType(GameMod? source, string keyName, string keyDesc, Dictionary<Stat, int> stats,
        Dictionary<Element, int> affinities, params Passive[] passives) {
        this.Source = source;
        this.KeyName = keyName;
        this.KeyDesc = keyDesc;

        this.Stats = stats;
        this._Affinities = affinities;
        this.Passives = passives;

        Core.UnitTypes.Add(this);
    }

    public string GetName(ThemeColor color, GameMod? mod = null) => color.Str() + this.KeyName.GetLang(mod);
    public string GetName(GameMod? mod = null) => this.GetName(ThemeColor.White, mod);
    public string GetDesc(GameMod? mod = null) => this.KeyDesc.GetLang(mod);

}

public static class UnitTypes {
    public static readonly UnitType TestUnitType = new(null, "TestUnitType", "Todo", new Dictionary<Stat, int>() {
        [Stats.Hp] = 100, [Stats.Str] = 100, [Stats.Mag] = 100, [Stats.Fth] = 100,
        [Stats.Amr] = 100, [Stats.Res] = 100, [Stats.Agi] = 100
    }, []);
}