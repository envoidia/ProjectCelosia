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
            MenuLog.Add(string.Format(Lang.LogChangeShield, self.FormatName()),
                (defend + shieldOld).Format(Colors.Shield), defend.Format(Colors.Shield),
                self.GetBaseStat(Stats.Hp).Format(Colors.Hp), (-shieldOld).Format(Colors.Shield));
        } else {
            MenuLog.Add(string.Format(Lang.LogLoseShield, self.FormatName(false),
                shieldOld.Format(Colors.Shield)));
        }
    }
}