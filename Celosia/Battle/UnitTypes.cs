using API;
using API.Battle;

namespace Celosia.Battle;

public static class UnitTypes
{
    // todo shorthand factories for stats and affs
    public static readonly UnitType Johny = new(Core.BaseModId, "UnitTypeJohny", new Dictionary<Stat, int>()
    {
        [Stats.Hp] = 200, [Stats.Str] = 200, [Stats.Mag] = 200, [Stats.Fth] = 50, [Stats.Amr] = 200,
        [Stats.Res] = 80, [Stats.Agi] = 20
    }, new Dictionary<Element, int>()
    {
        [Elements.Ignis] = -2, [Elements.Fulgur] = 4
    }, []);

    public static readonly UnitType Jane = new(Core.BaseModId, "UnitTypeJane", new Dictionary<Stat, int>()
    {
        [Stats.Hp] = 100, [Stats.Str] = 50, [Stats.Mag] = 100, [Stats.Fth] = 70, [Stats.Amr] = 40,
        [Stats.Res] = 70, [Stats.Agi] = 120
    }, new Dictionary<Element, int>()
    {
        [Elements.Ignis] = 5, [Elements.Glacies] = 1
    }, []);
}