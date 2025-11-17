using System;
using API.Extensions;
using API.Graphics;

namespace API.Battle;

public static class CalcLib {
    public static string ChangeSp(Unit unit, int change) {
        int spOld = unit.Sp;
        int spNew = (int) Math.Clamp(spOld + (change * unit.GetMult(Mults.SpGain)), 0, 1000);

        if (spNew != spOld) {
            unit.Sp = spNew;
            return string.Format(Lang.LogChangeSp, unit.FormatName(), spOld.Format(Colors.Sp),
                spNew.Format(Colors.Sp), (spNew - spOld).Format());
        }

        return "";
    }

    public static string ChangeBloom(Team team, Side side, int change) {
        int bloomOld = team.Bloom;
        int bloomNew = Math.Clamp(bloomOld + change, 0, 1000);

        if (bloomNew != bloomOld) {
            team.Bloom = bloomNew;
            return Lang.LogChangeBloom.FormatIcu((int) side, bloomOld.Format(Colors.Bloom),
                bloomNew.Format(Colors.Bloom), (bloomNew - bloomOld).Format());
        }

        return "";
    }
}