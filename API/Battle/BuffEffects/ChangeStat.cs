using API.Battle.State;
using API.Extensions;
using API.Graphics;

namespace API.Battle.BuffEffects;

public sealed class ChangeStat(Stat stat, int change) : IBuffEffect {
    public void OnGive(Unit self, int stacks) => this.Calc(self, change * stacks);
    public void OnRemove(Unit self, int stacks) => this.Calc(self, change * -stacks);

    private void Calc(Unit self, int changeFull) {
        int statDefaultDisp = self.GetBaseStat(stat);
        int statOldDispWithStage = self.GetStat(stat);

        self.SetStatMult(stat, self.GetStatMult(stat) + changeFull);

        int statNewDispWithStage = self.GetStat(stat);

        MenuLog.Add(string.Format(Lang.LogChangeStat, self.FormatName()), Colors.Stat + stat.GetName(),
            statOldDispWithStage.Format(statDefaultDisp.ToString()), statNewDispWithStage.Format(statDefaultDisp.ToString()),
            self.GetBaseStat(stat).Format(), (statNewDispWithStage - statOldDispWithStage).Format());
    }
}