using System;

namespace API.Battle;

public record Move(SkillInstance SkillInstance, Unit Self, uint TargetPos) {
    public bool IsInRange() {
        Range range = this.SkillInstance.Skill.Range;

        // Check for disallowed self-targeting
        if (!range.CanTargetSelf && (this.TargetPos == this.Self.Pos)) {
            return false;
        }

        // Check if target is within vertical range
        if (Math.Abs((int) PosLib.GetHeight(this.Self.Pos) - (int) PosLib.GetHeight(this.TargetPos)) >
            (range.RangeVertical + this.Self.GetStatMod(StatMods.Range))) {
            return false;
        }

        // Check if the targeted side is allowed
        return (range.Side == Side.Both) || (range.Side == PosLib.GetRelativeSide(this.Self.Pos, this.TargetPos));
    }

    public string GetTriesToUseString() =>
        // todo
        "";
}