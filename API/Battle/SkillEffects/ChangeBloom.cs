namespace API.Battle.SkillEffects;

public sealed class ChangeBloom(int change) : SkillEffect {
    public override ResultType Apply(Unit self, Unit target, bool isMainTarget, ResultType prevResultType) {
        if (!this.MainTargetOnly || isMainTarget) {
            Unit unit = this.GiveToSelf ? self : target;
            Team team = BattleHandler.Battle.GetTeamAtPos(unit.Pos);

            BattleHandler.AppendToLog(CalcLib.ChangeBloom(team, unit.GetSide(), change));
        }

        return ResultType.PseudoSuccess;
    }
}