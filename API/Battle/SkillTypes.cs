namespace API.Battle;

public static class SkillTypes
{
    public static readonly SkillType Str = new(Core.Id, "StatStr", "SkillTypeStr");
    public static readonly SkillType Mag = new(Core.Id, "StatMag", "SkillTypeMag");
    public static readonly SkillType Fth = new(Core.Id, "StatFth", "SkillTypeFth");
    public static readonly SkillType Stat = new(Core.Id, "SkillTypeStat");
}