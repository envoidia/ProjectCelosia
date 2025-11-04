using API.Graphics;

namespace API.Battle.BuffEffects;

public class ChangeStatMod(StatMod mod, int change) : IBuffEffect {
    public void OnGive(Unit self, uint stacks) => this.Calc(self, (int) (change * stacks));
    public void OnRemove(Unit self, uint stacks) => this.Calc(self, (int) (change * stacks * -1));

    private void Calc(Unit self, int changeFull) {
        int modOld = self.GetStatMod(mod);
        int modNew = modOld + changeFull;

        self.SetStatMod(mod, modNew);
        
        BattleHandlerLib.AppendToLog(string.Format(Lang.LogChangeMod, self.FormatName()),
            Colors.Stat + mod.GetName(), mod.Format(modOld), mod.Format(modNew));
    }
}