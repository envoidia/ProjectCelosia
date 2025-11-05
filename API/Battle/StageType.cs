using API.Entity;
using API.Modding;

namespace API.Battle;

public class StageType : IconEntity, IModItem {
    public Stat[] Stats { get; }

    public GameMod? Source { get; }

    public StageType(GameMod? source, string keyName, string descKey, string icon, params Stat[] stats)
        : base(keyName, descKey, icon) {
        this.Source = source;
        this.Stats = stats;
        Core.StageTypes.Add(this);
    }

    public string GetNameWithSign(int stage) => this.GetName() + " " + (stage > 0 ? "Up" : "Down");

    public override int GetHashCode() => this.KeyName.GetHashCode();
}

public class StageTypes {
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