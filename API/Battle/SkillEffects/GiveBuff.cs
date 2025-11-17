using System;
using System.Text;
using API.Battle.BuffEffects;
using API.Extensions;
using API.Graphics;

namespace API.Battle.SkillEffects;

// todo special case for shield
public sealed class GiveBuff(Buff buff, int turns, int stacks = 1) : SkillEffect(descInclusion: buff) {
    public ResultType MinResultType { get; init; } = ResultType.Success;

    public override ResultType Apply(Unit self, Unit target, bool isMainTarget, ResultType prevResultType) {
        if ((this.MainTargetOnly && !isMainTarget) || ((int) prevResultType > (int) this.MinResultType)) {
            return ResultType.PseudoSuccess;
        }

        Unit unit = this.GiveToSelf ? self : target;

        // todo fix overflow
        int turnsMod = turns + self.GetDurationModBuffTypeDealt(buff.BuffType)
                             + unit.GetDurationModBuffTypeTaken(buff.BuffType);

        int stacksMod = Math.Min(stacks + self.GetStacksModBuffTypeDealt(buff.BuffType) +
                                 unit.GetStacksModBuffTypeTaken(buff.BuffType), buff.MaxStacks);

        self.OnGiveBuff(target, buff, turnsMod, stacksMod);

        BuffInstance? buffInstance = null;

        foreach (BuffInstance instance in unit.BuffInstances) {
            if (instance.Buff == buff) buffInstance = instance;
        }

        // Already has buff
        string buffName = buff.GetName();

        if (buffInstance is not null) {
            StringBuilder str = new();

            int stacksOld = buffInstance.Stacks;
            int stacksNew = Math.Min(buff.MaxStacks, stacksOld + stacksMod);

            if (stacksNew != stacksOld) {
                buffInstance.Stacks = stacksNew;

                str.Append(string.Format(Lang.LogGiveBuffStacks, unit.FormatName(), buffName,
                    Colors.Num + stacksOld, Colors.Num + stacksNew));
            }

            int turnsOld = buffInstance.Turns;
            if (turnsMod > turnsOld) {
                buffInstance.Turns = turnsMod;

                if (stacksNew != stacksOld) {
                    str.Append(string.Format(Lang.LogTurnsNameless, Colors.Num + turnsOld, Colors.Num + turnsMod));
                } else {
                    str = new StringBuilder(string.Format(Lang.LogGiveBuffTurns, unit.FormatName(),
                        buffName, Colors.Num + turnsOld, Colors.Num + turnsMod));
                }
            }

            BattleHandlerLib.AppendToLog(str.ToString());

            int stacksAdded = stacksNew - stacksOld;

            if (stacksAdded <= 0) return ResultType.PseudoSuccess;

            foreach (IBuffEffect buffEffect in buffInstance.Buff.BuffEffects) {
                buffEffect.OnGive(unit, stacksAdded);
            }
        } else {
            // Add buff
            BattleHandlerLib.AppendToLog(Lang.LogGiveBuffGain.FormatIcu(unit.FormatName(false),
                buffName, buff.MaxStacks, Colors.Num + stacksMod, stacksMod,
                Colors.Num + turnsMod, turnsMod));

            unit.BuffInstances.Add(new BuffInstance(buff, turnsMod, stacksMod));
            buffInstance = unit.BuffInstances[^1];

            IBuffEffect[] buffEffects = buffInstance.Buff.BuffEffects;
            foreach (IBuffEffect buffEffect in buffEffects) {
                buffEffect.OnGive(unit, buffInstance.Stacks);
            }
        }

        return ResultType.PseudoSuccess;
    }
}