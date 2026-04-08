using API;
using API.Battle;
using API.Battle.BuffEffects;
using API.Battle.SkillEffects;
using API.Graphics;

namespace Celosia.Battle;

public static class Skills
{
    public static readonly Skill Fireball = new(Core.BaseModId, "SkillFireball",
        "__API:SkillDescBuff", Ranges.Other1R, 50)
    {
        SkillEffects = [new Damage(50, SkillTypes.Mag, Elements.Ignis),
            new GiveBuff(Buffs.Burn, 3)
        ],

        DescArgs = [$"{ThemeColor.Neg.Str}+1", Buffs.Burn, "3"]
    };

    public static readonly Skill HeatWave = new(Core.BaseModId, "SkillHeatWave",
        "__API:SkillDescBuff", Ranges.ColumnOf31RNoSelf, 100)
    {
        SkillEffects = [new Damage(35, SkillTypes.Mag, Elements.Ignis),
            new GiveBuff(Buffs.Burn, 1)
        ],

        DescArgs = [$"{ThemeColor.Neg.Str}+1", Buffs.Burn, "1"]
    };

    public static readonly Skill ChainLightning = new(Core.BaseModId, "SkillChainLightning",
    "__Celosia:SkillChainLightningDesc", Ranges.Other1R, 160)
    {
        SkillEffects = [new Damage(50, SkillTypes.Mag, Elements.Fulgur),
            new GiveBuff(Buffs.Shock, 3)
        ],

        DescArgs = [$"{ThemeColor.Neg.Str}+1", Buffs.Shock, "3"]
    };
}