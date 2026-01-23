using System;
using System.Collections.Generic;
using API.Battle.State;
using API.Extensions;
using API.Graphics;

namespace API.Battle.SkillEffects;

public sealed class Heal(int pow) : SkillEffect(pow, SkillTypes.Fth)
{
    public int Overheal { get; init; } = 0;

    public override SkillResultType Apply(Unit self, Unit target, bool isMainTarget, SkillResultType prevResultType)
    {
        if (this.MainTargetOnly && !isMainTarget)
        {
            return SkillResultType.PseudoSuccess;
        }

        Unit unit = this.GiveToSelf ? self : target;

        // Heals by pow% of user's Fth
        int heal = (int) (self.GetStat(Stats.Fth) * (this.Pow / 100d) *
                            self.GetMult(Mults.HealingDealt) * unit.GetMult(Mults.HealingTaken));

        self.OnDealHeal(unit, heal, this.Overheal);
        unit.OnTakeHeal(self, heal, this.Overheal);

        int hpOld = unit.Hp;
        int hpMax = unit.GetBaseStat(Stats.Hp);

        // Picks the lower of (current HP + heal amount) and (maximum allowed overHeal
        // of this skill), and then the higher between that and current HP
        int hpNew = Math.Max(hpOld, Math.Min(hpOld + heal, (int) (hpMax * (1 + (this.Overheal / 1000d)))));

        if (hpNew > hpOld)
        {
            unit.Hp = hpNew;

            LogLib.Add("LogChangeHp".FormatLang([unit.FormatName(), hpOld.Format(ThemeColor.Hp),
                hpNew.Format(ThemeColor.Hp), hpMax.Format(ThemeColor.Hp),
                (hpNew - hpOld).Format(ThemeColor.Hp)]));
        }

        return SkillResultType.PseudoSuccess;
    }
}