using System;
using API.Extensions;
using API.Graphics;

namespace API.Battle;

public static class CalcLib
{
    public static string ChangeSp(Unit unit, int change)
    {
        int spOld = unit.Sp;
        int spNew = (int) Math.Clamp(spOld + (change * unit.GetMult(Mults.SpGain)), 0, 1000);

        if (spNew != spOld)
        {
            unit.Sp = spNew;
            return "LogChangeSp".FormatLang([unit.FormatName(), spOld.Format(ThemeColor.Sp),
                spNew.Format(ThemeColor.Sp), (spNew - spOld).Format()]);
        }

        return "";
    }

    public static string ChangeBloom(Team team, Side side, int change)
    {
        int bloomOld = team.Bloom;
        int bloomNew = Math.Clamp(bloomOld + change, 0, 1000);

        if (bloomNew != bloomOld)
        {
            team.Bloom = bloomNew;
            return "LogChangeBloom".IcuFormatLang([(int) side, bloomOld.Format(ThemeColor.Bloom),
                bloomNew.Format(ThemeColor.Bloom), (bloomNew - bloomOld).Format()]);
        }

        return "";
    }
}