using API.Entity;

namespace API.Battle;

public class StageType : IconEntity {
    public Stat[] Stats { get; }

    public StageType(string keyName, string descKey, string icon, params Stat[] stats)
        : base(keyName, descKey, icon) {
        this.Stats = stats;
        Core.StageTypes.Add(this);
    }

    public string GetNameWithSign(int stage) => this.GetName() + " " + (stage > 0 ? "Up" : "Down");

    public override int GetHashCode() => this.KeyName.GetHashCode();
}

public class StageTypes {
    public static readonly StageType None = new("", "", "");

    public static readonly StageType Atk =
        new("StageAtk", "todo", "/c[lightRed]/i[energy-sword]", Stats.Str, Stats.Mag);

    public static readonly StageType Def =
        new("StageDef", "todo", "/c[#006eff]/i[rosa-shield]", Stats.Amr, Stats.Res);

    public static readonly StageType Fth =
        new("StatFth", "todo", "/c[lightPurple]/i[star-altar]", Stats.Fth);

    public static readonly StageType Agi =
        new("StatAgi", "todo", "/c[lightGreen]/i[walking-boot]", Stats.Agi);
}