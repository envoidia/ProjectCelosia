namespace API.Battle;

public class BoolStat {
    public string KeyName { get; }
    public string LogMsgKey { get; }
    public bool IsPositive { get; }
    public bool PossessiveNameInLogMsg { get; }
    public bool IsVisible { get; }

    public BoolStat(string keyName, string logMsgKey, bool isPositive, bool possessiveNameInLogMsg, bool isVisible) {
        this.KeyName = keyName;
        this.LogMsgKey = logMsgKey;
        this.IsPositive = isPositive;
        this.PossessiveNameInLogMsg = possessiveNameInLogMsg;
        this.IsVisible = isVisible;
        Core.BoolStats.Add(this);
    }

    public override int GetHashCode() => this.KeyName.GetHashCode();
}

public static class BoolStats {
    public static readonly BoolStat EffectBlock = new("BoolEffectBlock",
        "LogChangeBooleanStatEffectBlock", true, false, true);

    public static readonly BoolStat InfiniteSp = new("BoolInfiniteSp",
        "LogChangeBooleanStatInfiniteSp", true, true, true);

    public static readonly BoolStat UnableToAct = new("BoolUnableToAct",
        "LogChangeBooleanStatUnableToAct", false, false, true);

    public static readonly BoolStat UnableToActImmunity = new("BoolUnableToActImmunity",
        "LogChangeBooleanStatUnableToActImmune", true, false, false);

    public static readonly BoolStat EquipDisabled = new("BoolEquipDisabled",
        "LogChangeBooleanStatEquipDisabled", false, true, true);

    public static readonly BoolStat EquipDisabledImmunity = new("BoolEquipDisabledImmunity",
        "LogChangeBooleanStatEquipDisabledImmune", true, false, false);
}