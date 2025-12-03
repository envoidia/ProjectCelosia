using System;
using System.Collections.Generic;
using API.Battle.State;
using API.Extensions;
using API.Graphics;

namespace API.Battle.SkillEffects;

public sealed class ChangeStage(StageType stageType, int turns, int stacks) : SkillEffect(descInclusion: stageType) {
    public override ResultType Apply(Unit self, Unit target, bool isMainTarget, ResultType prevResultType) {
        if (this.MainTargetOnly && !isMainTarget) return ResultType.PseudoSuccess;

        List<string> msg = [];
        string str = "";
        string str2 = "";

        Unit unit = this.GiveToSelf ? self : target;

        // Apply self's mods
        int turnsMod = turns + self.GetDurationModBuffTypeDealt(BattleLib.GetStageBuffType(stacks)) +
                       unit.GetDurationModBuffTypeTaken(BattleLib.GetStageBuffType(stacks));

        int stacksMod = stacks + self.GetStacksModBuffTypeDealt(BattleLib.GetStageBuffType(stacks)) +
                        unit.GetStacksModBuffTypeTaken(BattleLib.GetStageBuffType(stacks));

        self.OnChangeStage(target, stageType, turnsMod, stacksMod);

        int stageOld = target.GetStage(stageType);
        int stageNew = Math.Clamp(stageOld + stacksMod, -5, 5);

        string stageName = stageType.GetName();

        if (stageNew != stageOld) {
            str = string.Format(Lang.LogChangeStageStacks, unit.FormatName(),
                stageName, stageOld.Format(), stageNew.Format());
            str2 = unit.GetStageStatString(stageType, stageNew);

            unit.SetStage(stageType, stageNew);
        }

        // Refresh turns
        int turnsOld = unit.GetStageTurns(stageType);
        if (((stageOld >= 0) && (stacksMod >= 0)) || ((stageOld <= 0) && (stacksMod <= 0) && (turnsMod > turnsOld))) {
            unit.SetStageTurns(stageType, turnsMod);
            if (stageNew != stageOld) {
                msg.Add(str + string.Format(Lang.LogTurnsNameless, Colors.Num + turnsOld, Colors.Num + turnsMod) +
                        str2);
            } else {
                msg.Add(string.Format(Lang.LogChangeStageTurns, unit.FormatName(), stageName, Colors.Num + turnsOld,
                    Colors.Num + turnsMod));
            }
        } else if (stageNew != stageOld) {
            msg.Add(str + str2);
        }

        if (msg.Count > 0) LogLib.Add(msg);

        return ResultType.PseudoSuccess;
    }
}