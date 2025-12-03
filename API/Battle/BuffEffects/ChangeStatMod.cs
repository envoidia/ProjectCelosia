using API.Battle.State;
using API.Graphics;

namespace API.Battle.BuffEffects;

public sealed class ChangeStatMod(StatMod mod, int change) : IBuffEffect {
    public void OnGive(Unit self, int stacks) => this.Calc(self, change * stacks);
    public void OnRemove(Unit self, int stacks) => this.Calc(self, change * -stacks);

    private void Calc(Unit self, int changeFull) {
        int modOld = self.GetStatMod(mod);
        int modNew = modOld + changeFull;

        self.SetStatMod(mod, modNew);

        LogLib.Add(string.Format(Lang.LogChangeMod, self.FormatName()),
            Colors.Stat + mod.GetName(), mod.Format(modOld), mod.Format(modNew));
    }
}