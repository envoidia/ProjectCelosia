using API.Battle.State;
using API.Graphics;

namespace API.Battle.BuffEffects;

public sealed class ChangeStatMod(StatMod mod, int change) : IBuffEffect {
    public void OnGive(Unit self, int stacks) => this._Calc(self, change * stacks);
    public void OnRemove(Unit self, int stacks) => this._Calc(self, change * -stacks);

    private void _Calc(Unit self, int changeFull) {
        int modOld = self.GetStatMod(mod);
        int modNew = modOld + changeFull;

        self.SetStatMod(mod, modNew);

        LogLib.Add(string.Format(Lang.LogChangeMod, self.FormatName()),
            ColorCode.Stat + mod.GetName(), mod.Format(modOld), mod.Format(modNew));
    }
}