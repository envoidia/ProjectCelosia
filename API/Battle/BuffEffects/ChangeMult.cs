using System;
using API.Battle.State;
using API.Graphics;

namespace API.Battle.BuffEffects;

public sealed class ChangeMult(Mult mult, int change) : IBuffEffect {
    public void OnGive(Unit self, int stacks) => this._Calc(self, change * stacks);
    public void OnRemove(Unit self, int stacks) => this._Calc(self, change * -stacks);

    private void _Calc(Unit self, int changeFull) {
        // Minimum mult to display
        int multMin = mult.MinValue;

        int multOld = self.GetRawMult(mult);
        int multNew = multOld + changeFull;
        float changeDisplay = (Math.Max(multNew, multMin) - Math.Max(multOld, multMin)) / 10f;

        self.SetMult(mult, multNew);

        LogLib.Add(string.Format(Lang.LogChangeMult, self.FormatName(), Colors.Stat + mult.GetName(),
            mult.Format(multOld), mult.Format(multNew), mult.FormatChange(changeDisplay)));
    }
}