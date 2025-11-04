using System;
using API.Extensions;

namespace API.Battle.BuffEffects;

public class ChangeExtraActions(int change) : IBuffEffect {
    public void OnGive(Unit self, uint stacks) => Calc(self, (int) (change * stacks));
    public void OnRemove(Unit self, uint stacks) => Calc(self, (int) (change * stacks * -1));

    private static void Calc(Unit self, int changeFull) {
        uint exAOld = self.ExtraActions;
        uint exANew = (uint) (exAOld + changeFull);
        self.ExtraActions = exANew;
        
        BattleHandlerLib.AppendToLog(string.Format(Lang.LogChangeExtraActions, self.FormatName(),
            Math.Max(exAOld, 0).Format(), Math.Max(exANew, 0).Format()));
    }
}