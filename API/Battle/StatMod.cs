namespace API.Battle;

public class StatMod {
    public string KeyName { get; }
    public bool IsPositive { get; }

    public StatMod(string keyName, bool isPositive) {
        this.KeyName = keyName;
        this.IsPositive = isPositive;
        Core.StatMods.Add(this);
    }

    public override int GetHashCode() => this.KeyName.GetHashCode();

    public string FormatVal(int val) => "todo"; // todo Color + sign
}

public static class StatMods {
    public static readonly StatMod DurationBuffDealt = new("ModDurationBuffDealt", true);
    public static readonly StatMod DurationBuffTaken = new("ModDurationBuffTaken", true);
    public static readonly StatMod DurationDebuffDealt = new("ModDurationDebuffDealt", true);
    public static readonly StatMod DurationDebuffTaken = new("ModDurationDebuffTaken", false);
    public static readonly StatMod StacksBuffDealt = new("ModStacksBuffDealt", true);
    public static readonly StatMod StacksBuffTaken = new("ModStacksBuffTaken", true);
    public static readonly StatMod StacksDebuffDealt = new("ModStacksDebuffDealt", true);
    public static readonly StatMod StacksDebuffTaken = new("ModStacksDebuffTaken", false);
    public static readonly StatMod Range = new("ModRange", true);
}