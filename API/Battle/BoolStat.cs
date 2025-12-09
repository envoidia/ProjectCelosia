using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class BoolStat : _IModItem, INameable {
    public string LogMsgKey { get; }
    public bool IsPositive { get; }
    public bool PossessiveNameInLogMsg { get; }
    public bool IsVisible { get; }

    public GameMod? Source { get; }
    public string KeyName { get; }

    public BoolStat(GameMod? source, string keyName, string logMsgKey, bool isPositive, bool possessiveNameInLogMsg,
        bool isVisible) {
        this.Source = source;
        this.KeyName = keyName;

        this.LogMsgKey = logMsgKey;
        this.IsPositive = isPositive;
        this.PossessiveNameInLogMsg = possessiveNameInLogMsg;
        this.IsVisible = isVisible;

        Core.BoolStats.Add(this);
    }

    public string GetName(string color, GameMod? mod = null) => color + this.KeyName.GetLang(mod);
    public string GetName(GameMod? mod = null) => this.GetName(Colors.Stat, mod);

    // todo format?
}

public static class BoolStats {
    public static readonly BoolStat EffectBlock = new(null, "BoolEffectBlock",
        "LogChangeBooleanStatEffectBlock", true, false, true);

    public static readonly BoolStat InfiniteSp = new(null, "BoolInfiniteSp",
        "LogChangeBooleanStatInfiniteSp", true, true, true);

    public static readonly BoolStat UnableToAct = new(null, "BoolUnableToAct",
        "LogChangeBooleanStatUnableToAct", false, false, true);

    public static readonly BoolStat UnableToActImmunity = new(null, "BoolUnableToActImmunity",
        "LogChangeBooleanStatUnableToActImmune", true, false, false);

    public static readonly BoolStat EquipDisabled = new(null, "BoolEquipDisabled",
        "LogChangeBooleanStatEquipDisabled", false, true, true);

    public static readonly BoolStat EquipDisabledImmunity = new(null, "BoolEquipDisabledImmunity",
        "LogChangeBooleanStatEquipDisabledImmune", true, false, false);
}