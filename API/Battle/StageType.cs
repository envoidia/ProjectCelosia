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

    public StageType(GameMod? source, string keyName, string keyDesc, string icon, params Stat[] stats) {
        this.Source = source;
        this.KeyName = keyName;
        this.Icon = icon;
        this.KeyDesc = keyDesc;

        this.Stats = stats;
        Core.StageTypes.Add(this);
    }

    public string GetName(string color, GameMod? mod = null) =>
        $"{this.Icon} {color}{this.KeyName.GetLang(mod)}";

    public string GetName(GameMod? mod = null) => this.GetName(Colors.Buff, mod);

    public string GetNameWithSign(int stage) => $"{this.GetName()} {(stage > 0 ? "Up" : "Down")}";

    public string GetDesc(GameMod? mod = null) => this.KeyDesc.GetLang(mod);
}

public static class StageTypes {
    public static readonly StageType None = new(null, "", "", "");

    public static readonly StageType Atk =
        new(null, "StageAtk", "todo", "/c[lightRed]/i[energy-sword]", Stats.Str, Stats.Mag);

    public static readonly StageType Def =
        new(null, "StageDef", "todo", "/c[#006eff]/i[rosa-shield]", Stats.Amr, Stats.Res);

    public static readonly StageType Fth =
        new(null, "StatFth", "todo", "/c[lightPurple]/i[star-altar]", Stats.Fth);

    public static readonly StageType Agi =
        new(null, "StatAgi", "todo", "/c[lightGreen]/i[walking-boot]", Stats.Agi);
}