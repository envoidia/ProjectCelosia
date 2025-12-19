using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class StageType : IDescribable, IRegistrable {
    public Stat[] Stats { get; }

    public string KeyName { get; }

    public string Icon { get; }
    public string KeyDesc { get; }

    public string ModId { get; }
    public string ItemId { get; init; }

    /// <summary>
    /// todo docs
    /// </summary>
    /// <param name="modId"></param>
    /// <param name="keyName"></param>
    /// <param name="icon"></param>
    /// <param name="itemId">Item ID. If not provided, will use <c>keyName</c></param>
    /// <param name="stats"></param>
    public StageType(string modId, string keyName, string icon, Stat[] stats, string? itemId = null) {
        this.Stats = stats;

        this.KeyName = keyName;
        this.KeyDesc = $"{keyName}Desc";
        this.Icon = icon;

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
    }

    public string GetName(ThemeColor color) =>
        $"{this.Icon} {color.Str()}{this.GetLang()}";
    public string GetName() => this.GetName(ThemeColor.Buff);

    public string GetNameWithSign(int stage) => $"{this.GetName()} {(stage > 0 ? "Up" : "Down")}";

    public string GetDesc() => this.KeyDesc.GetLang(this.ModId);
}

public static class StageTypes {
    public static readonly StageType Atk = new(Core.Id, "StageAtk",
        $"{ThemeColor.Atk.Str()}/i[energy-sword]", [Stats.Str, Stats.Mag]);

    public static readonly StageType Def = new(Core.Id, "StageDef",
        $"{ThemeColor.Def.Str()}/i[rosa-shield]", [Stats.Amr, Stats.Res]);

    public static readonly StageType Fth = new(Core.Id, "StatFth",
        $"{ThemeColor.Fth.Str()}/i[star-altar]", [Stats.Fth], "StageTypeFth");

    public static readonly StageType Agi = new(Core.Id, "StatAgi",
        $"{ThemeColor.Agi.Str()}/i[walking-boot]", [Stats.Agi], "StageTypeAgi");
}