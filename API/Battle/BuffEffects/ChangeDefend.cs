using API.Extensions;
using API.Graphics;

namespace API.Battle.BuffEffects;

public sealed class ChangeDefend(int change) : IBuffEffect {
    public void OnGive(Unit self, int stacks) {
        int hpMax = self.GetBaseStat(Stats.Hp);

        // Add defend (shield + defend cannot exceed max HP)
        int defendOld = self.Defend;
        int defendNew = (int) ((self.Shield + defendOld + ((change / 1000d) * hpMax * stacks)) > hpMax
            ? hpMax - self.Shield
            : (change / 1000d) * hpMax * stacks);

        self.Defend = defendNew;

        int shield = self.Shield;

        BattleHandler.AppendToLog(string.Format(Lang.LogChangeShield, self.FormatName(),
            (shield + defendOld).Format(Colors.Shield), (shield + defendNew).Format(Colors.Shield),
            hpMax.Format(Colors.Hp), ((shield + defendNew) - (shield + defendOld)).Format(Colors.Shield)));
    }

    public void OnRemove(Unit self, int stacks) {
        int defendOld = self.Defend;
        self.Defend = 0;
        int shield = self.Shield;

        if (self.Shield > 0) {
            BattleHandler.AppendToLog(string.Format(Lang.LogChangeShield, self.FormatName(),
                (shield + defendOld).Format(Colors.Shield), shield.Format(Colors.Shield),
                self.GetBaseStat(Stats.Hp).Format(Colors.Hp), (-defendOld).Format(Colors.Shield)));
        } else {
            BattleHandler.AppendToLog(string.Format(Lang.LogChangeShield, self.FormatName(), false),
                defendOld.Format(Colors.Shield));
        }
    }
}