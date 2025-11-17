namespace API.Battle.SkillEffects;

public sealed class ChangeBloom(int change) : SkillEffect {
    public override ResultType Apply(Unit self, Unit target, bool isMainTarget, ResultType prevResultType) {
        if (!this.MainTargetOnly || isMainTarget) {
            Unit unit = this.GiveToSelf ? self : target;
            Team team = BattleHandlerLib.Battle.GetTeamAtPos(unit.Pos);

            BattleHandlerLib.AppendToLog(CalcLib.ChangeBloom(team, unit.GetSide(), change));
        }

        return ResultType.PseudoSuccess;
    }
}