using System;
using API.Graphics;

namespace API.Battle.BuffEffects;

public class ChangeMult(Mult mult, int change) : IBuffEffect {
    public void OnGive(Unit self, uint stacks) => this.Calc(self, (int) (change * stacks));
    public void OnRemove(Unit self, uint stacks) => this.Calc(self, (int) (change * -stacks));

    private void Calc(Unit self, int changeFull) {
        // Minimum mult to display
        uint multMin = mult.MinValue;

        uint multOld = self.GetRawMult(mult);
        uint multNew = (uint) (multOld + changeFull);
        float changeDisplay = (Math.Max(multNew, multMin) - Math.Max(multOld, multMin)) / 10f;

        self.SetMult(mult, multNew);

        BattleHandlerLib.AppendToLog(string.Format(Lang.LogChangeMult, self.FormatName(), Colors.Stat + mult.GetName(),
            mult.Format(multOld), mult.Format(multNew), mult.FormatChange(changeDisplay)));
    }
}