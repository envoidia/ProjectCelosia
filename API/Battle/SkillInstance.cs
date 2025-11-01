namespace API.Battle;

public class SkillInstance(Skill skill) {
    public Skill Skill { get; } = skill;
    public uint Cooldown { get; set; } = 0;
}