using API.Battle;

namespace Celosia.Battle;

public static class UnitTypes {
    public static readonly UnitType Johny = new(Main.Id, "UnitTypeJohny", new Dictionary<Stat, int>() {
        [Stats.Hp] = 200, [Stats.Str] = 200, [Stats.Mag] = 200, [Stats.Fth] = 50, [Stats.Amr] = 200,
        [Stats.Res] = 80, [Stats.Agi] = 20
    }, []);
}