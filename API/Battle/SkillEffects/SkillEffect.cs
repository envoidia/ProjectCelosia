using API.Entity;

namespace API.Battle.SkillEffects;

public abstract class SkillEffect(
    uint pow = 0,
    SkillType skillType = SkillType.Stat,
    IconEntity? descInclusion = null) {
    public uint Pow { get; } = pow;
    public SkillType SkillType { get; } = skillType;

    public bool GiveToSelf { get; init; } = false;
    public bool MainTargetOnly { get; init; } = false;
    public bool IsInstant { get; init; } = false;

    public IconEntity? DescInclusion { get; init; } = descInclusion;

    public abstract ResultType Apply(Unit self, Unit target, bool isMainTarget, ResultType prevResultType);
}