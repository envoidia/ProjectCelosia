using System;
using API.Battle.State;

namespace API.Battle;

public sealed record Move(SkillInstance SkillInstance, Unit Self, int TargetPos) {
    public bool IsInRange() {
        Range range = this.SkillInstance.Skill.Range;

        // Check for disallowed self-targeting
        if (!range.CanTargetSelf && (this.TargetPos == this.Self.Pos)) {
            return false;
        }

        // Check if target is within vertical range
        if (Math.Abs(PosLib.GetHeight(this.Self.Pos) - PosLib.GetHeight(this.TargetPos)) >
            (range.RangeVertical + this.Self.GetStatMod(StatMods.Range))) {
            return false;
        }

        // Check if the targeted side is allowed
        return (range.Side == Side.Both) || (range.Side == PosLib.GetRelativeSide(this.Self.Pos, this.TargetPos));
    }

    public string GetTriesToUseString() =>
        string.Format(Lang.LogTriesToUse1, this.Self.FormatName(false), this.SkillInstance.Skill.GetName())
            + (this.SkillInstance.Skill.IsRangeSelf() ? "" : string.Format(Lang.LogTriesToUse2,
            BattleHandler.Battle.GetUnitAtPos(this.TargetPos).FormatName(false), this.TargetPos, false));
}