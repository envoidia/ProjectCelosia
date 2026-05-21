using System.Collections.Generic;
using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

// todo represent available and equipped skills and equipped item
public sealed class UnitType : IDescribable, IRegistrable
{

    public Dictionary<Stat, int> Stats { get; }
    internal readonly Dictionary<Element, int> _Affinities;
    public Passive[] Passives { get; init; }

    public string KeyName { get; }
    public string KeyDesc { get; }

    public string ModId { get; }
    public string ItemId { get; init; }

    public UnitType(string modId, string keyName, Dictionary<Stat, int> stats,
        Dictionary<Element, int> affinities, Passive[] passives, string? itemId = null)
    {
        this.Stats = stats;
        this._Affinities = affinities;
        this.Passives = passives;

        this.KeyName = keyName;
        this.KeyDesc = $"{keyName}Desc";

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
    }

    public override string ToString()
    {
        return $"{base.ToString()}: {this.GetName()} -- {this.GetDesc()}";
    }

    public string GetName(ThemeColor color)
    {
        return color.Str + this.GetLang();
    }

    public string GetName()
    {
        return this.GetName(ThemeColor.Fg);
    }

    public string GetDesc()
    {
        return this.KeyDesc.GetLang(this.ModId);
    }
}
