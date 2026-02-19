using API.Battle.State;
using API.Extensions;
using API.Graphics;

namespace API.Battle.BuffEffects;

public sealed class ChangeDefend(int change) : IBuffEffect
{
    public void OnGive(Unit self, int stacks)
    {
        int hpMax = self.GetBaseStat(Stats.Hp);

        // Add defend (shield + defend cannot exceed max HP)
        int defendOld = self.Defend;
        int defendNew = (int) ((self.Shield + defendOld + ((change / 1000d) * hpMax * stacks)) > hpMax
            ? hpMax - self.Shield
            : (change / 1000d) * hpMax * stacks);

        self.Defend = defendNew;

        int shield = self.Shield;

        LogLib.Add("LogChangeShield".FormatLang([self.FormatName(),
            (shield + defendOld).Format(ThemeColor.Shield), (shield + defendNew).Format(ThemeColor.Shield),
            hpMax.Format(ThemeColor.Hp), ((shield + defendNew) - (shield + defendOld)).Format(ThemeColor.Shield)]));
    }

    public void OnRemove(Unit self, int stacks)
    {
        int defendOld = self.Defend;
        self.Defend = 0;
        int shield = self.Shield;

        if (self.Shield > 0)
        {
            LogLib.Add("LogChangeShield".FormatLang([self.FormatName(),
                (shield + defendOld).Format(ThemeColor.Shield), shield.Format(ThemeColor.Shield),
                self.GetBaseStat(Stats.Hp).Format(ThemeColor.Hp), (-defendOld).Format(ThemeColor.Shield)]));
        }
        else
        { //# 0 = name, 1 = old shield, 2 = new shield, 3 = max HP, 4 = shield change
            LogLib.Add("LogChangeShield".FormatLang([self.FormatName(), defendOld, 0, self.GetBaseStat(Stats.Hp),
                defendOld.Format(ThemeColor.Shield)]));
        }
    }
}