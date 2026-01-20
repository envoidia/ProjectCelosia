using API.Battle.State;

namespace API.Battle.SkillEffects;

public sealed class ChangeBloom(int change) : SkillEffect
{
    public override SkillResultType Apply(Unit self, Unit target, bool isMainTarget, SkillResultType prevResultType)
    {
        if (!this.MainTargetOnly || isMainTarget)
        {
            Unit unit = this.GiveToSelf ? self : target;
            Team team = BattleLib.Battle.GetTeamAtPos(unit.Pos);

            LogLib.Add(CalcLib.ChangeBloom(team, unit.GetSide(), change));
        }

        return SkillResultType.PseudoSuccess;
    }
}