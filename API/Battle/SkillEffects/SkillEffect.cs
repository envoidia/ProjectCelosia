using System.Globalization;
using API.Entity;

namespace API.Battle.SkillEffects;

// todo arbitrary predicates (here and IBuffEffect)
public abstract class SkillEffect(int pow = 0, SkillType? skillType = null, IconEntity? descInclusion = null) {
    public int Pow => pow;
    public SkillType? SkillType => skillType;
    public DescriptionEntity? DescInclusion { get; init; } = descInclusion;

    public bool GiveToSelf { get; init; } = false;
    public bool MainTargetOnly { get; init; } = false;
    public bool IsInstant { get; init; } = false;
    public Element Element { get; init; } = Elements.Vis;

    public abstract ResultType Apply(Unit self, Unit target, bool isMainTarget, ResultType prevResultType);
}