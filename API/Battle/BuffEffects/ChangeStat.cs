using API.Extensions;
using API.Graphics;

namespace API.Battle.BuffEffects;

public class ChangeStat(Stat stat, int change) : IBuffEffect {
    public void OnGive(Unit self, uint stacks) => this.Calc(self, (int) (change * stacks));
    public void OnRemove(Unit self, uint stacks) => this.Calc(self, (int) (change * -stacks));

    private void Calc(Unit self, int changeFull) {
        uint statDefaultDisp = self.GetBaseStat(stat);
        uint statOldDispWithStage = self.GetStat(stat);

        self.SetStatMult(stat, (uint) (self.GetStatMult(stat) + changeFull));

        uint statNewDispWithStage = self.GetStat(stat);

        BattleHandlerLib.AppendToLog(string.Format(Lang.LogChangeStat, self.FormatName()), Colors.Stat + stat.GetName(),
            statOldDispWithStage.Format(statDefaultDisp), statNewDispWithStage.Format(statDefaultDisp),
            self.GetBaseStat(stat).Format(), (statNewDispWithStage - statOldDispWithStage).Format());
    }
}