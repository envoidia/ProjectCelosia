using API;
using API.Battle;
using API.Battle.BuffEffects;
using API.Graphics;

namespace Celosia.Battle;

public static class Buffs
{
    public static readonly Buff Burn = new(Core.BaseModId, "BuffBurn", $"{ThemeColor.Ignis.Str}/i[small-fire]",
        "__API:BuffDesc2PerStackHp", BuffType.Debuff, 5,
        [new ChangeHp(-20), new ChangeStat(Stats.Str, -50)])
    {
        DescArgs = [$"{ThemeColor.Neg.Str}-2%{ThemeColor.Stat.Str}", $"{ThemeColor.Neg.Str}-5% {ThemeColor.Stat.Str}{Stats.Str.GetName()}"]
    };

    public static readonly Buff Shock = new(Core.BaseModId, "BuffShock", $"{ThemeColor.Fulgur.Str}/i[power-lightning]",
        "__API:BuffDesc2PerStackHp", BuffType.Debuff, 5,
        [new ChangeHp(-20), new ChangeStat(Stats.Agi, -50)])
    {
        DescArgs = [$"{ThemeColor.Neg.Str}-2%{ThemeColor.Stat.Str}", $"{ThemeColor.Neg.Str}-5% {ThemeColor.Stat.Str}{Stats.Agi.GetName()}"]
    };

    // public static readonly Buff GlaciesAffUp = new(Core.BaseModId, "BuffBurn", "/i[snowflake-2]",
    //     "__API:BuffDesc2PerStackHp", BuffType.Buff, 3,
    //     [new ChangeAffinity(Elements.Glacies, 1)])
    // {
    //     DescArgs = [$"{ThemeColor.Neg.Str}-545%{ThemeColor.Stat.Str}", $"{ThemeColor.Neg.Str}-545% {ThemeColor.Stat.Str}{Stats.Str.GetName()}"]
    // };
}