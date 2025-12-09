using API.Battle.State;
using API.Extensions;
using API.Graphics;

namespace API.Battle.BuffEffects;

public sealed class ChangeShield : IBuffEffect {
    // Actual value is set in SkillEffects/GiveBuff.Apply() (todo)

    public void OnRemove(Unit self, int stacks) {
        int shieldOld = self.Shield;
        self.Shield = 0;
        int defend = self.Defend;

        if (self.Defend > 0) {
            LogLib.Add(string.Format(Lang.LogChangeShield, self.FormatName()),
                (defend + shieldOld).Format(ColorCode.Shield), defend.Format(ColorCode.Shield),
                self.GetBaseStat(Stats.Hp).Format(ColorCode.Hp), (-shieldOld).Format(ColorCode.Shield));
        } else {
            LogLib.Add(string.Format(Lang.LogLoseShield, self.FormatName(false),
                shieldOld.Format(ColorCode.Shield)));
        }
    }
}