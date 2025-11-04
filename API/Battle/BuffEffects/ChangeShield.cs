using API.Extensions;
using API.Graphics;

namespace API.Battle.BuffEffects;

public class ChangeShield : IBuffEffect {
    
    // Actual value is set in SkillEffects/GiveBuff.Apply() (todo)
    
    public void OnRemove(Unit self, int stacks) {
        uint shieldOld = self.Shield;
        self.Shield = 0;
        uint defend = self.Defend;

        if (self.Defend > 0) {
            BattleHandlerLib.AppendToLog(string.Format(Lang.LogChangeShield, self.FormatName()),
                (defend + shieldOld).Format(Colors.Shield), defend.Format(Colors.Shield),
                self.GetBaseStat(Stats.Hp).Format(Colors.Hp), ((int) (shieldOld * -1)).Format(Colors.Shield));
        } else {
            BattleHandlerLib.AppendToLog(string.Format(Lang.LogLoseShield, self.FormatName(false),
                shieldOld.Format(Colors.Shield)));
        }
    }
}