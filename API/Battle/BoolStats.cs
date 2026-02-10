namespace API.Battle;

public static class BoolStats
{
    public static readonly BoolStat EffectBlock = new(Core.Id, "BoolEffectBlock",
        "LogChangeBooleanStatEffectBlock", true, false, true);

    public static readonly BoolStat InfiniteSp = new(Core.Id, "BoolInfiniteSp",
        "LogChangeBooleanStatInfiniteSp", true, true, true);

    public static readonly BoolStat UnableToAct = new(Core.Id, "BoolUnableToAct",
        "LogChangeBooleanStatUnableToAct", false, false, true);

    public static readonly BoolStat UnableToActImmunity = new(Core.Id, "BoolUnableToActImmunity",
        "LogChangeBooleanStatUnableToActImmune", true, false, false);

    public static readonly BoolStat EquipDisabled = new(Core.Id, "BoolEquipDisabled",
        "LogChangeBooleanStatEquipDisabled", false, true, true);

    public static readonly BoolStat EquipDisabledImmunity = new(Core.Id, "BoolEquipDisabledImmunity",
        "LogChangeBooleanStatEquipDisabledImmune", true, false, false);
}