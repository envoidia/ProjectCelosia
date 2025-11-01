namespace API.Battle;

public class Stat {
    public string KeyName { get; }
    public StageType StageType { get; }

    public Stat(string keyName, StageType stageType) {
        this.KeyName = keyName;
        this.StageType = stageType;
    }

    public override int GetHashCode() => this.KeyName.GetHashCode();
}

public static class Stats {
    public static readonly Stat Hp = new("Hp", StageTypes.None);
    public static readonly Stat Str = new("StatStr", StageTypes.Atk);
    public static readonly Stat Mag = new("StatMag", StageTypes.Atk);
    public static readonly Stat Fth = new("StatFth", StageTypes.Fth);
    public static readonly Stat Amr = new("StatAmr", StageTypes.Def);
    public static readonly Stat Res = new("StatRes", StageTypes.Def);
    public static readonly Stat Agi = new("StatAgi", StageTypes.Agi);
}