namespace API.Battle;

public sealed class SkillInstance(Skill skill) {
    public Skill Skill => skill;
    public int Cooldown { get; set; } = 0;

    public string GetCostCdFormatted() {
        if(this.Skill.Cooldown == 0) return this.Skill.GetCostFormatted();
        if(this.Cooldown == 0) return $"{this.Skill.Cooldown} {Lang.CD}, {this.Skill.GetCostFormatted()}";
        return $"{this.Cooldown}/{this.Skill.Cooldown} {Lang.CD}, {this.Skill.GetCostFormatted()}";
    }
}