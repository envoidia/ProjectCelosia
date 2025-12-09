using System;
using API.Battle.State;
using API.Extensions;
using API.Graphics;

namespace API.Battle.BuffEffects;

public sealed class ChangeHp(int change, bool isImmediate = false, bool isPercentage = true, bool isPierce = false)
    : IBuffEffect {
    // todo this might need to display the name if immediate
    public void OnGive(Unit self, int stacks) {
        if (!isImmediate) return;
        LogLib.Add(this._Calc(self, stacks));
    }

    public string[] OnTurnEnd(Unit self, int stacks) => !isImmediate ? this._Calc(self, stacks) : [];

    private string[] _Calc(Unit self, int stacks) {
        // Damage
        if (change < 0) {
            float multDoTDmgTaken = isImmediate ? 1 : self.GetMult(Mults.DoTDmgTaken);

            int dmg = isPercentage
                ? (int) Math.Abs(self.GetBaseStat(Stats.Hp) * (change / 1000d) * stacks * self.GetMult(Mults.DmgTaken)
                                  * multDoTDmgTaken * self.GetMult(Mults.PercentageDmgTaken))
                : (int) (change * self.GetMult(Mults.DmgTaken) * multDoTDmgTaken);

            self.OnTakeDamage(self, dmg);

            return self.Damage(dmg, isPierce, false).Messages.ToArray();
        }

        // Healing
        int hpOld = self.Hp;
        int hpMax = self.GetBaseStat(Stats.Hp);
        int heal = (int) (change * (isPercentage ? hpMax : 1) * stacks * self.GetMult(Mults.HealingTaken));
        int hpNew = Math.Max(hpOld, Math.Min(hpOld + heal, hpMax));

        if (hpNew <= hpOld) return [];

        self.OnTakeHeal(self, heal, 0);

        self.Hp = hpNew;
        int changeFull = Math.Max(hpNew - hpOld, 0);

        return [
            string.Format(Lang.LogChangeHp, "", hpOld.Format(ColorCode.Hp), hpNew.Format(ColorCode.Hp),
                hpMax.Format(ColorCode.Hp), changeFull.Format())
        ];
    }
}