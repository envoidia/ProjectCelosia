using System;
using System.Text;
using API.Battle.BuffEffects;
using API.Battle.State;
using API.Debug;
using API.Extensions;
using API.Graphics;

namespace API.Battle.SkillEffects;

// todo special case for shield
public sealed class GiveBuff(Buff buff, int turns, int stacks = 1) : SkillEffect(descInclusion: buff)
{
    public SkillResultType MinResultType { get; init; } = SkillResultType.Success;

    public override SkillResultType Apply(Unit self, Unit target, bool isMainTarget, SkillResultType prevResultType)
    {
        if ((this.MainTargetOnly && !isMainTarget) || ((int) prevResultType > (int) this.MinResultType))
        {
            return SkillResultType.PseudoSuccess;
        }

        Unit unit = this.GiveToSelf ? self : target;

        int turnsMod = turns + self.GetDurationModBuffTypeDealt(buff.BuffType)
                             + unit.GetDurationModBuffTypeTaken(buff.BuffType);

        int stacksMod = Math.Min(stacks + self.GetStacksModBuffTypeDealt(buff.BuffType) +
                                 unit.GetStacksModBuffTypeTaken(buff.BuffType), buff.MaxStacks);

        self.OnGiveBuff(target, buff, turnsMod, stacksMod);

        BuffInstance? buffInstance = null;

        foreach (BuffInstance instance in unit.BuffInstances)
        {
            if (instance.Buff == buff)
            {
                buffInstance = instance;
            }
        }

        string buffName = buff.GetName();

        // Already has buff
        // todo fix dupe buff bug
        if (buffInstance is not null)
        {
            const int Cap = 192;
            StringBuilder str = new(Cap);

            int stacksOld = buffInstance.Stacks;
            int stacksNew = Math.Min(buff.MaxStacks, stacksOld + stacksMod);

            if (stacksNew != stacksOld)
            {
                buffInstance.Stacks = stacksNew;

                str.Append("LogGiveBuffStacks".FormatLang([unit.FormatName(), buffName,
                    ThemeColor.Imp.Str + stacksOld, ThemeColor.Imp.Str + stacksNew]));
            }

            int turnsOld = buffInstance.Turns;
            if (turnsMod > turnsOld)
            {
                buffInstance.Turns = turnsMod;

                if (stacksNew != stacksOld)
                {
                    str.Append("LogTurnsNameless".FormatLang([ThemeColor.Imp.Str + turnsOld,
                        ThemeColor.Imp.Str + turnsMod]));
                }
                else
                {
                    str.Clear();
                    str.Append("LogGiveBuffTurns".FormatLang([unit.FormatName(),
                        buffName, ThemeColor.Imp.Str + turnsOld, ThemeColor.Imp.Str + turnsMod]));
                }
            }

            Assert.CapIs(str, Cap); // todo remove before final release
            LogLib.Add(str.ToString());

            int stacksAdded = stacksNew - stacksOld;

            if (stacksAdded <= 0)
            {
                return SkillResultType.PseudoSuccess;
            }

            foreach (IBuffEffect buffEffect in buffInstance.Buff.BuffEffects)
            {
                buffEffect.OnGive(unit, stacksAdded);
            }
        }
        else
        {
            // Add buff
            LogLib.Add("LogGiveBuffGain".IcuFormatLang([unit.FormatName(false),
                buffName, buff.MaxStacks, ThemeColor.Imp.Str + stacksMod, stacksMod,
                ThemeColor.Imp.Str + turnsMod, turnsMod]));

            unit.GiveBuffInstances(new BuffInstance(buff, turnsMod, stacksMod));
        }

        return SkillResultType.PseudoSuccess;
    }
}