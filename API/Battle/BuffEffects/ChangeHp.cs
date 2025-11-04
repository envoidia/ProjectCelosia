using System;
using API.Extensions;
using API.Graphics;

namespace API.Battle.BuffEffects;

public class ChangeHp(int change, bool isImmediate = false, bool isPercentage = true, bool isPierce = false)
    : IBuffEffect {
    // todo this might need to display the name if immediate
    public void OnGive(Unit self, uint stacks) {
        if (!isImmediate) return;
        BattleHandlerLib.AppendToLog(this.Calc(self, stacks));
    }

    public string[] OnTurnEnd(Unit self, uint stacks) => !isImmediate ? this.Calc(self, stacks) : [];

    private string[] Calc(Unit self, uint stacks) {
        // Damage
        if (change < 0) {
            double multDoTDmgTaken = isImmediate ? 1 : self.GetMult(Mults.DoTDmgTaken);

            uint dmg = isPercentage
                ? (uint) Math.Abs(self.GetBaseStat(Stats.Hp) * (change / 1000d) * stacks * self.GetMult(Mults.DmgTaken)
                                  * multDoTDmgTaken * self.GetMult(Mults.PercentageDmgTaken))
                : (uint) (change * self.GetMult(Mults.DmgTaken) * multDoTDmgTaken);

            self.OnTakeDamage(self, dmg);

            return self.Damage(dmg, isPierce, false).Messages.ToArray();
        }

        // Healing
        uint hpOld = self.Hp;
        uint hpMax = self.GetBaseStat(Stats.Hp);
        uint heal = (uint) (change * (isPercentage ? hpMax : 1) * stacks * self.GetMult(Mults.HealingTaken));
        uint hpNew = Math.Max(hpOld, Math.Min(hpOld + heal, hpMax));

        if (hpNew <= hpOld) return [];

        self.OnTakeHeal(self, heal, 0);

        self.Hp = hpNew;
        uint changeFull = Math.Max(hpNew - hpOld, 0);

        return [
            string.Format(Lang.LogChangeHp, "", hpOld.Format(Colors.Hp), hpNew.Format(Colors.Hp),
                hpMax.Format(Colors.Hp), changeFull.Format())
        ];
    }
}