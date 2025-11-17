namespace API.Battle;

public sealed class SkillInstance(Skill skill) {
    public Skill Skill => skill;
    public int Cooldown { get; set; } = 0;
}