using System;
using System.Collections.Generic;
using API.Extensions;
using API.Graphics;

namespace API.Battle.SkillEffects;

public class Heal(uint pow) : SkillEffect(pow, SkillTypes.Fth) {
    public uint Overheal { get; init; } = 0;

    public override ResultType Apply(Unit self, Unit target, bool isMainTarget, ResultType prevResultType) {
        if (this.MainTargetOnly && !isMainTarget) return ResultType.PseudoSuccess;

        List<string> msg = [];

        Unit unit = this.GiveToSelf ? self : target;

        // Heals by pow% of user's Fth
        uint heal = (uint) (self.GetStat(Stats.Fth) * (this.Pow / 100d) *
                            self.GetMult(Mults.HealingDealt) * unit.GetMult(Mults.HealingTaken));

        self.OnDealHeal(unit, heal, this.Overheal);
        unit.OnTakeHeal(self, heal, this.Overheal);

        uint hpOld = unit.Hp;
        uint hpMax = unit.GetBaseStat(Stats.Hp);

        // Picks the lower of (current HP + heal amount) and (maximum allowed overHeal
        // of this skill), and then the higher between that and current HP
        uint hpNew = Math.Max(hpOld, Math.Min(hpOld + heal, (uint) (hpMax * (1 + (this.Overheal / 1000d)))));

        if (hpNew > hpOld) {
            unit.Hp = hpNew;

            msg.Add(Lang.LogChangeHp.FormatLang(unit.FormatName(), hpOld.Format(Colors.Hp),
                hpNew.Format(Colors.Hp), hpMax.Format(Colors.Hp),
                ((int) (hpNew - hpOld)).Format(Colors.Hp)));
        }

        BattleHandlerLib.AppendToLog(msg);
        return ResultType.PseudoSuccess;
    }
}