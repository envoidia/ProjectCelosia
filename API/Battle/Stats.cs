namespace API.Battle;

public static class Stats
{
    public static readonly Stat Hp = new(Core.Id, "StatHp", null);
    public static readonly Stat Str = new(Core.Id, "StatStr", StageTypes.Atk);
    public static readonly Stat Mag = new(Core.Id, "StatMag", StageTypes.Atk);
    public static readonly Stat Fth = new(Core.Id, "StatFth", StageTypes.Fth);
    public static readonly Stat Amr = new(Core.Id, "StatAmr", StageTypes.Def);
    public static readonly Stat Res = new(Core.Id, "StatRes", StageTypes.Def);
    public static readonly Stat Agi = new(Core.Id, "StatAgi", StageTypes.Agi);
}
