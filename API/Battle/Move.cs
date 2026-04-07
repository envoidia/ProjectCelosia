using System;
using API.Battle.State;
using API.Extensions;

namespace API.Battle;

public sealed record Move(SkillInstance SkillInstance, Unit Self, int TargetPos)
{
    public bool IsInRange()
    {
        return this.SkillInstance.Skill.Range.CanReach(this.Self.Pos, this.TargetPos,
            this.Self.GetStatMod(StatMods.Range));
    }

    public string GetTriesToUseString()
    {
        return "LogTriesToUse1".FormatLang([this.Self.FormatName(false),
            this.SkillInstance.Skill.GetName()]) + (this.SkillInstance.Skill.IsRangeSelf()
            ? "" : "LogTriesToUse2".FormatLang([
            BattleLib.Battle.GetUnitAtPos(this.TargetPos).FormatName(false), this.TargetPos, false]));
    }
}