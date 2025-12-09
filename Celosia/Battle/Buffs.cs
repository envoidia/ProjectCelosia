using API.Battle;
using API.Battle.BuffEffects;
using API.Graphics;

namespace Celosia.Battle;

public static class Buffs {
    public static readonly Buff Burn = new(Main.Mod, "BuffBurn", "/c[orange]/i[small-fire]",
        "BuffDesc2PerStackHp", BuffType.Debuff, 5,
        new ChangeHp(-20), new ChangeStat(Stats.Str, -50)) {
        DescArgs = [$"{Colors.Neg}-2%{Colors.Stat}", $"{Colors.Neg}-5% {Colors.Stat}{Stats.Str.GetName()}"]
    };
}