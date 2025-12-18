using API.Battle;
using API.Battle.BuffEffects;
using API.Graphics;

namespace Celosia.Battle;

public static class Buffs {
    public static readonly Buff Burn = new(Main.Mod, "BuffBurn", $"{ThemeColor.Ignis.Str()}/i[small-fire]",
        "BuffDesc2PerStackHp", BuffType.Debuff, 5,
        new ChangeHp(-20), new ChangeStat(Stats.Str, -50)) {
        DescArgs = [$"{ThemeColor.Neg.Str()}-2%{ThemeColor.Stat.Str()}", $"{ThemeColor.Neg.Str()}-5% {ThemeColor.Stat.Str()}{Stats.Str.GetName()}"]
    };
}