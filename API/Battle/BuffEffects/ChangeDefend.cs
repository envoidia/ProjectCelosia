using API.Battle.State;
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

        LogLib.Add(string.Format(Lang.LogChangeShield, self.FormatName(),
            (shield + defendOld).Format(ColorCode.Shield), (shield + defendNew).Format(ColorCode.Shield),
            hpMax.Format(ColorCode.Hp), ((shield + defendNew) - (shield + defendOld)).Format(ColorCode.Shield)));
    }

    public void OnRemove(Unit self, int stacks) {
        int defendOld = self.Defend;
        self.Defend = 0;
        int shield = self.Shield;

        if (self.Shield > 0) {
            LogLib.Add(string.Format(Lang.LogChangeShield, self.FormatName(),
                (shield + defendOld).Format(ColorCode.Shield), shield.Format(ColorCode.Shield),
                self.GetBaseStat(Stats.Hp).Format(ColorCode.Hp), (-defendOld).Format(ColorCode.Shield)));
        } else {
            LogLib.Add(string.Format(Lang.LogChangeShield, self.FormatName(), false),
                defendOld.Format(ColorCode.Shield));
        }
    }
}