namespace API.Battle;

public static class StatMods
{
    public static readonly StatMod DurationBuffDealt = new(Core.Id, "ModDurationBuffDealt", true);
    public static readonly StatMod DurationBuffTaken = new(Core.Id, "ModDurationBuffTaken", true);
    public static readonly StatMod DurationDebuffDealt = new(Core.Id, "ModDurationDebuffDealt", true);
    public static readonly StatMod DurationDebuffTaken = new(Core.Id, "ModDurationDebuffTaken", false);
    public static readonly StatMod StacksBuffDealt = new(Core.Id, "ModStacksBuffDealt", true);
    public static readonly StatMod StacksBuffTaken = new(Core.Id, "ModStacksBuffTaken", true);
    public static readonly StatMod StacksDebuffDealt = new(Core.Id, "ModStacksDebuffDealt", true);
    public static readonly StatMod StacksDebuffTaken = new(Core.Id, "ModStacksDebuffTaken", false);
    public static readonly StatMod Range = new(Core.Id, "ModRange", true);
}