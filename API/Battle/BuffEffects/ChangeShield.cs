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
                (defend + shieldOld).Format(ThemeColor.Shield), defend.Format(ThemeColor.Shield),
                self.GetBaseStat(Stats.Hp).Format(ThemeColor.Hp), (-shieldOld).Format(ThemeColor.Shield));
        } else {
            LogLib.Add(string.Format(Lang.LogLoseShield, self.FormatName(false),
                shieldOld.Format(ThemeColor.Shield)));
        }
    }
}