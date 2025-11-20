using System;
using API.Extensions;

namespace API.Battle.BuffEffects;

public sealed class ChangeExtraActions(int change) : IBuffEffect {
    public void OnGive(Unit self, int stacks) => Calc(self, change * stacks);
    public void OnRemove(Unit self, int stacks) => Calc(self, change * -stacks);

    private static void Calc(Unit self, int changeFull) {
        int exAOld = self.ExtraActions;
        int exANew = exAOld + changeFull;
        self.ExtraActions = exANew;

        BattleHandler.AppendToLog(string.Format(Lang.LogChangeExtraActions, self.FormatName(),
            Math.Max(exAOld, 0).Format(), Math.Max(exANew, 0).Format()));
    }
}