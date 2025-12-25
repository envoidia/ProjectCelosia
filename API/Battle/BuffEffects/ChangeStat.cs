using API.Battle.State;
using API.Extensions;
using API.Graphics;
using API.Util;

namespace API.Battle.BuffEffects;

public sealed class ChangeStat(Stat stat, int change) : IBuffEffect {
    public void OnGive(Unit self, int stacks) => this._Calc(self, change * stacks);
    public void OnRemove(Unit self, int stacks) => this._Calc(self, change * -stacks);

    private void _Calc(Unit self, int changeFull) {
        int statDefault = self.GetBaseStat(stat);
        int statOldWithStage = self.GetStat(stat);

        self.SetStatMult(stat, self.GetStatMult(stat) + changeFull);

        int statNewWithStage = self.GetStat(stat);

        LogLib.Add("LogChangeStat".FormatLang(self.FormatName(), stat.GetName(),
            TextLib.FormatStat(statOldWithStage, statDefault), TextLib.FormatStat(statNewWithStage, statDefault),
            self.GetBaseStat(stat).Format(ThemeColor.Imp, false), (statNewWithStage - statOldWithStage).Format()));
    }
}