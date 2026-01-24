using System;
using API.Battle.State;
using API.Extensions;
using API.Graphics;

namespace API.Battle.SkillEffects;

public sealed class ChangeStage(StageType stageType, int turns, int stacks) : SkillEffect(descInclusion: stageType)
{
    public override SkillResultType Apply(Unit self, Unit target, bool isMainTarget, SkillResultType prevResultType)
    {
        if (this.MainTargetOnly && !isMainTarget)
        {
            return SkillResultType.PseudoSuccess;
        }

        string? msg = null;
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

        if (stageNew != stageOld)
        {
            str = "LogChangeStageStacks".FormatLang([unit.FormatName(),
                stageName, stageOld.Format(), stageNew.Format()]);
            str2 = unit.GetStageStatString(stageType, stageNew);

            unit.SetStage(stageType, stageNew);
        }

        // Refresh turns
        int turnsOld = unit.GetStageTurns(stageType);
        if (((stageOld >= 0) && (stacksMod >= 0)) || ((stageOld <= 0) && (stacksMod <= 0) && (turnsMod > turnsOld)))
        {
            unit.SetStageTurns(stageType, turnsMod);
            if (stageNew != stageOld)
            {
                msg = str + "LogTurnsNameless".FormatLang([ThemeColor.Imp.Str + turnsOld,
                    ThemeColor.Imp.Str + turnsMod]) + str2;
            }
            else
            {
                msg = "LogChangeStageTurns".FormatLang([unit.FormatName(), stageName,
                    ThemeColor.Imp.Str + turnsOld, ThemeColor.Imp.Str + turnsMod]);
            }
        }
        else if (stageNew != stageOld)
        {
            msg = str + str2;
        }

        if (msg is not null)
        {
            LogLib.Add(msg);
        }

        return SkillResultType.PseudoSuccess;
    }
}