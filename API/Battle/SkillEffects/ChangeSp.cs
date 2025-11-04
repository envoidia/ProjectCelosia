namespace API.Battle.SkillEffects;

public class ChangeSp(int change) : SkillEffect {
    public override ResultType Apply(Unit self, Unit target, bool isMainTarget, ResultType prevResultType) {
        if (!this.MainTargetOnly || isMainTarget) {
            BattleHandlerLib.AppendToLog(Calcs.ChangeSp(this.GiveToSelf ? self : target, change));
        }

        return ResultType.PseudoSuccess;
    }
}