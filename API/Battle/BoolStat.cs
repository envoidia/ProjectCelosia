using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class BoolStat : INameable, IRegistrable {
    public string LogMsgKey { get; }
    public bool IsPositive { get; }
    public bool PossessiveNameInLogMsg { get; }
    public bool IsVisible { get; }

    public string KeyName { get; }

    public string ModId { get; }
    public string ItemId { get; init; }

    public BoolStat(string modId, string keyName, string logMsgKey, bool isPositive, bool possessiveNameInLogMsg,
        bool isVisible, string? itemId = null) {
        this.LogMsgKey = logMsgKey;
        this.IsPositive = isPositive;
        this.PossessiveNameInLogMsg = possessiveNameInLogMsg;
        this.IsVisible = isVisible;

        this.KeyName = keyName;

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
    }

    public override string ToString() => $"{base.ToString()}: {this.GetName()}";

    public string GetName(ThemeColor color) => color.Str() + this.GetLang();
    public string GetName() => this.GetName(ThemeColor.Stat);

    // todo format?
}

public static class BoolStats {
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