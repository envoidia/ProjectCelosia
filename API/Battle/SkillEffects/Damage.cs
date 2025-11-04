namespace API.Battle.SkillEffects;

public class Damage(Element element, uint pow, SkillType skillType) : SkillEffect(pow, skillType) {
    public ResultType MinResultType { get; init; } = ResultType.HitEffectBlock;
    public bool IsPierce { get; init; } = false;
    public bool IsFollowUp { get; init; } = false;

    public override ResultType Apply(Unit self, Unit target, bool isMainTarget, ResultType prevResultType) {
        // If the previous hit failed entirely, this one wouldn't have been reached. If this return statement is ever
        // reached, it's under special circumstances (such as a main target only effect), so let the attack continue
        // just to be safe
        if (((int) prevResultType < (int) this.MinResultType) || (this.MainTargetOnly && !isMainTarget)) {
            return ResultType.PseudoSuccess;
        }

        uint atk;
        uint def;

        if (this.SkillType == SkillTypes.Str) {
            atk = self.GetStat(Stats.Str);
            def = target.GetStat(Stats.Amr);
        } else {
            atk = self.GetStat(Stats.Mag);
            def = target.GetStat(Stats.Res);
        }

        float affMultDmgDealt = AffLib.DmgDealt[self.GetAffinity(element)] / 1000f;
        float affMultDmgTaken = AffLib.DmgTaken[target.GetAffinity(element)] / 1000f;
        
        float multWeakDmgDealt = 1;
        float multWeakDmgTaken = 1;
        
        if (target.IsWeakTo(element)) {
            multWeakDmgDealt = self.GetMult(Mults.WeakDmgDealt);
            multWeakDmgTaken = target.GetMult(Mults.WeakDmgTaken);
        }

        float multFollowUpDmgDealt = 1;
        float multFollowUpDmgTaken = 1;

        if (this.IsFollowUp) {
            multFollowUpDmgDealt = self.GetMult(Mults.FollowUpDmgDealt);
            multFollowUpDmgTaken = target.GetMult(Mults.FollowUpDmgTaken);
        }

        uint dmg;

        // No damage on affinity immunity
        if (affMultDmgTaken == 0) {
            dmg = 0;
        } else {
            // todo null safety
            dmg = BattleLib.StatMult * (uint) (((float) atk / def) * this.Pow * affMultDmgDealt * affMultDmgTaken *
                                               self.GetMult(Mults.DmgDealt) * target.GetMult(Mults.DmgTaken) *
                                               self.GetMult(element.MultDmgDealt) *
                                               target.GetMult(element.MultDmgTaken) * multWeakDmgDealt *
                                               multWeakDmgTaken * multFollowUpDmgDealt * multFollowUpDmgTaken);

            self.OnDealDamage(target, dmg, element);
            target.OnTakeDamage(self, dmg, element);
        }

        // Deal damage
        Result result = target.Damage(dmg, this.IsPierce);
        BattleHandlerLib.AppendToLog(result.Messages);

        return result.ResultType;
    }
}