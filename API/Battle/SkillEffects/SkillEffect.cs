using API.Name;

namespace API.Battle.SkillEffects;

// todo arbitrary predicates (here and IBuffEffect)
public abstract class SkillEffect(int pow = 0, SkillType? skillType = null, IDescribable? descInclusion = null)
{
    public readonly int Pow = pow;
    public readonly SkillType? SkillType = skillType;
    public readonly IDescribable? DescInclusion = descInclusion;

    public readonly bool GiveToSelf = false;
    public readonly bool MainTargetOnly = false;
    public readonly bool IsInstant = false;
    public Element Element { get; init; } = Element.Vis;

    public abstract SkillResultType Apply(Unit self, Unit target, bool isMainTarget, SkillResultType prevResultType);
}