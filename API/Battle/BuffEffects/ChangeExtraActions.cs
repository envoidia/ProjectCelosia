using System;
using API.Battle.State;
using API.Extensions;

namespace API.Battle.BuffEffects;

public sealed class ChangeExtraActions(int change) : IBuffEffect {
    public void OnGive(Unit self, int stacks) => _Calc(self, change * stacks);
    public void OnRemove(Unit self, int stacks) => _Calc(self, change * -stacks);

    private static void _Calc(Unit self, int changeFull) {
        int exAOld = self.ExtraActions;
        int exANew = exAOld + changeFull;
        self.ExtraActions = exANew;

        LogLib.Add("LogChangeExtraActions".FormatLang([self.FormatName(),
            Math.Max(exAOld, 0).Format(), Math.Max(exANew, 0).Format()]));
    }
}