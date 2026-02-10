namespace API.Battle;

public static class Skills
{
    public static readonly Skill Nothing = new(Core.Id, "SkillNothing", "Blank", Ranges.Other3ROrSelf, 0);
    public static readonly Skill Defend = new(Core.Id, "SkillDefend", "Todo", Ranges.Self, 0);
}
