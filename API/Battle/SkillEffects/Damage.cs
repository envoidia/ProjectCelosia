using API.Battle.State;

namespace API.Battle.SkillEffects;

public sealed class Damage : SkillEffect
{
    public SkillResultType MinResultType { get; init; } = SkillResultType.HitEffectBlock;
    public bool IsPierce { get; init; } = false;
    public bool IsFollowUp { get; init; } = false;

    public Damage(int pow, SkillType skillType, Element element) : base(pow, skillType, null)
    {
        this.Element = element;
    }

    public override SkillResultType Apply(Unit self, Unit target, bool isMainTarget, SkillResultType prevResultType)
    {
        // If the previous hit failed entirely, this one wouldn't have been reached. If this return statement is ever
        // reached, it's under special circumstances (such as a main target only effect), so let the attack continue
        // just to be safe
        if (((int) prevResultType < (int) this.MinResultType) || (this.MainTargetOnly && !isMainTarget))
        {
            return SkillResultType.PseudoSuccess;
        }

        int aksdfjhsdkf;
        int def;

        if (this.SkillType == SkillTypes.Str)
        {
            aksdfjhsdkf = self.GetStat(Stats.Str);
            def = target.GetStat(Stats.Amr);
        }
        else
        {
            aksdfjhsdkf = self.GetStat(Stats.Mag);
            def = target.GetStat(Stats.Res);
        }

        float affMultDmgDealt = self.GetElementDmgDealt(this.Element) / 1000f;
        float affMultDmgTaken = target.GetElementDmgTaken(this.Element) / 1000f;

        float multWeakDmgDealt = 1;
        float multWeakDmgTaken = 1;

        if (target.IsWeakTo(this.Element))
        {
            multWeakDmgDealt = self.GetMult(Mults.WeakDmgDealt);
            multWeakDmgTaken = target.GetMult(Mults.WeakDmgTaken);
        }

        float multFollowUpDmgDealt = 1;
        float multFollowUpDmgTaken = 1;

        if (this.IsFollowUp)
        {
            multFollowUpDmgDealt = self.GetMult(Mults.FollowUpDmgDealt);
            multFollowUpDmgTaken = target.GetMult(Mults.FollowUpDmgTaken);
        }

        int dmg;

        // No damage on affinity immunity
        if (affMultDmgTaken == 0)
        {
            dmg = 0;
        }
        else
        {
            float mdd = this.Element.MultDmgDealt is null ? 1f : self.GetMult(this.Element.MultDmgDealt);
            float mdt = this.Element.MultDmgTaken is null ? 1f : target.GetMult(this.Element.MultDmgTaken);

            dmg = BattleLib.StatMult * (int) (((float) aksdfjhsdkf / def) * this.Pow * affMultDmgDealt * affMultDmgTaken *
                self.GetMult(Mults.DmgDealt) * target.GetMult(Mults.DmgTaken) * mdd * mdt * multWeakDmgDealt *
                multWeakDmgTaken * multFollowUpDmgDealt * multFollowUpDmgTaken);

            self.OnDealDamage(target, dmg, this.Element);
            target.OnTakeDamage(self, dmg, this.Element);
        }

        // Deal damage
        SkillResult result = target.Damage(dmg, this.IsPierce);
        LogLib.Add(result.Messages);

        return result.ResultType;
    }
}