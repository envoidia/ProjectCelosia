namespace API.Battle;

public static class StageTypes
{
    public static readonly StageType Atk = new(Core.Id, "StageAtk",
        $"{ThemeColor.Atk.Str}/i[energy-sword]", [Stats.Str, Stats.Mag]);

    public static readonly StageType Def = new(Core.Id, "StageDef",
        $"{ThemeColor.Def.Str}/i[rosa-shield]", [Stats.Amr, Stats.Res]);

    public static readonly StageType Fth = new(Core.Id, "StatFth",
        $"{ThemeColor.Fth.Str}/i[star-altar]", [Stats.Fth], "StageTypeFth");

    public static readonly StageType Agi = new(Core.Id, "StatAgi",
        $"{ThemeColor.Agi.Str}/i[walking-boot]", [Stats.Agi], "StageTypeAgi");
}
