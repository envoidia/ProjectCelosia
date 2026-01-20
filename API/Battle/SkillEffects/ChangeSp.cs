using API.Battle.State;

namespace API.Battle.SkillEffects;

public sealed class ChangeSp(int change) : SkillEffect
{
    public override SkillResultType Apply(Unit self, Unit target, bool isMainTarget, SkillResultType prevResultType)
    {
        if (!this.MainTargetOnly || isMainTarget)
        {
            LogLib.Add(CalcLib.ChangeSp(this.GiveToSelf ? self : target, change));
        }

        return SkillResultType.PseudoSuccess;
    }
}