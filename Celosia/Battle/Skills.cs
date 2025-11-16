using API.Battle;
using API.Battle.SkillEffects;
using API.Graphics;

namespace Celosia.Battle;

public static class Skills {
    public static readonly Skill Fireball = new(Main.ModInstance, "SkillFireball", "SkillDescBuff",
        Ranges.Other1R, 50) {
        SkillEffects = [
            new Damage(50, SkillTypes.Mag, Elements.Ignis),
            new GiveBuff(Buffs.Burn, 3)
        ],
        DescriptionArgs = [$"{Colors.Neg}+1 ", Buffs.Burn, "2"]
    };
}