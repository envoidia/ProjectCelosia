using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class StageType : _IModItem, IDescribable {
    public Stat[] Stats { get; }

    public GameMod? Source { get; }
    public string KeyName { get; }

    public string Icon { get; }
    public string KeyDesc { get; }

    public StageType(GameMod? source, string keyName, string icon, params Stat[] stats) {
        this.Source = source;
        this.KeyName = keyName;
        this.KeyDesc = $"{keyName}Desc";
        this.Icon = icon;

        this.Stats = stats;
        Core.StageTypes.Add(this);
    }

    public string GetName(ThemeColor color, GameMod? mod = null) =>
        $"{this.Icon} {color.Str()}{this.KeyName.GetLang(mod)}";

    public string GetName(GameMod? mod = null) => this.GetName(ThemeColor.Buff, mod);

    public string GetNameWithSign(int stage) => $"{this.GetName()} {(stage > 0 ? "Up" : "Down")}";

    public string GetDesc(GameMod? mod = null) => this.KeyDesc.GetLang(mod);
}

public static class StageTypes {
    public static readonly StageType Atk =
        new(null, "StageAtk",
            $"{ThemeColor.Atk.Str()}/i[energy-sword]", Stats.Str, Stats.Mag);

    public static readonly StageType Def =
        new(null, "StageDef",
            $"{ThemeColor.Def.Str()}/i[rosa-shield]", Stats.Amr, Stats.Res);

    public static readonly StageType Fth =
        new(null, "StatFth",
            $"{ThemeColor.Fth.Str()}/i[star-altar]", Stats.Fth);

    public static readonly StageType Agi =
        new(null, "StatAgi",
            $"{ThemeColor.Agi.Str()}/i[walking-boot]", Stats.Agi);
}