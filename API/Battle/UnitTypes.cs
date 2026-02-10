using System.Collections.Generic;

namespace API.Battle;

public static class UnitTypes
{
    public static readonly UnitType TestUnitType = new(Core.Id, "TestUnitType", new Dictionary<Stat, int>()
    {
        [Stats.Hp] = 100, [Stats.Str] = 100, [Stats.Mag] = 100, [Stats.Fth] = 100,
        [Stats.Amr] = 100, [Stats.Res] = 100, [Stats.Agi] = 100
    }, [], []);
}