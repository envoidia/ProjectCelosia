using API.Extensions;
using API.Graphics;

namespace API.Battle.BuffEffects;

public class ChangeDefend(int change) : IBuffEffect {
    public void OnGive(Unit self, uint stacks) {
        uint hpMax = self.GetBaseStat(Stats.Hp);

        // Add defend (shield + defend cannot exceed max HP)
        uint defendOld = self.Defend;
        uint defendNew = (uint) ((self.Shield + defendOld + ((change / 1000d) * hpMax * stacks)) > hpMax
            ? hpMax - self.Shield
            : (change / 1000d) * hpMax * stacks);

        self.Defend = defendNew;

        uint shield = self.Shield;

        BattleHandlerLib.AppendToLog(string.Format(Lang.LogChangeShield, self.FormatName(),
            (shield + defendOld).Format(Colors.Shield), (shield + defendNew).Format(Colors.Shield),
            hpMax.Format(Colors.Hp), ((shield + defendNew) - (shield + defendOld)).Format(Colors.Shield)));
    }

    public void OnRemove(Unit self, uint stacks) {
        uint defendOld = self.Defend;
        self.Defend = 0;
        uint shield = self.Shield;

        if (self.Shield > 0) {
            BattleHandlerLib.AppendToLog(string.Format(Lang.LogChangeShield, self.FormatName(),
                (shield + defendOld).Format(Colors.Shield), shield.Format(Colors.Shield),
                self.GetBaseStat(Stats.Hp).Format(Colors.Hp), ((int) -defendOld).Format(Colors.Shield)));
        } else {
            BattleHandlerLib.AppendToLog(string.Format(Lang.LogChangeShield, self.FormatName(), false),
                defendOld.Format(Colors.Shield));
        }
    }
}