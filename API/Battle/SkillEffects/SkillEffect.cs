using API.Name;

namespace API.Battle.SkillEffects;

// todo arbitrary predicates (here and IBuffEffect)
public abstract class SkillEffect(int pow = 0, SkillType? skillType = null, IDescribable? descInclusion = null)
{
    public int Pow
    {
        get
        {
            return pow;
        }
    }

    public SkillType? SkillType
    {
        get
        {
            return skillType;
        }
    }

    public IDescribable? DescInclusion { get; init; } = descInclusion;

    public bool GiveToSelf { get; init; } = false;
    public bool MainTargetOnly { get; init; } = false;
    public bool IsInstant { get; init; } = false;
    public Element Element { get; init; } = Element.Vis;

    public abstract SkillResultType Apply(Unit self, Unit target, bool isMainTarget, SkillResultType prevResultType);
}