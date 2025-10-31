namespace API.Battle;

public class StageTypes {
    public static readonly StageType Atk =
        new(Lang.StageAtk, "todo", "/c[lightRed]/i[energy-sword]", StageTypeId.Atk, Stat.Str, Stat.Mag);

    public static readonly StageType Def =
        new(Lang.StageDef, "todo", "/c[#006eff]/i[rosa-shield]", StageTypeId.Def, Stat.Amr, Stat.Res);

    public static readonly StageType Fth =
        new(Lang.StatFth, "todo", "/c[lightPurple]/i[star-altar]", StageTypeId.Fth, Stat.Fth);

    public static readonly StageType Agi =
        new(Lang.StatAgi, "todo", "/c[lightGreen]/i[walking-boot]", StageTypeId.Agi, Stat.Agi);
}