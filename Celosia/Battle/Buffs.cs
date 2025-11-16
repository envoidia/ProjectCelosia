using API.Battle;
using API.Battle.BuffEffects;
using API.Graphics;

namespace Celosia.Battle;

public class Buffs {
    public static readonly Buff Burn = new(Main.ModInstance, "BuffBurn", "BuffDesc2PerStackHp",
        "/c[orange]/i[small-fire]", BuffType.Debuff, 5, new ChangeHp(-20),
        new ChangeStat(Stats.Str, -50)) {
        DescriptionArgs = [$"{Colors.Neg}-2%{Colors.Stat}", $"{Colors.Neg}-5% {Colors.Stat}{Stats.Str.GetName()}"]
    };
}