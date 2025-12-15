using API.Battle.State;
using API.Extensions;

namespace API.Battle.BuffEffects;

public sealed class ChangeBooleanStat(BoolStat stat, int change) : IBuffEffect {
    public void OnGive(Unit self, int stacks) => this._Calc(self, change * stacks);
    public void OnRemove(Unit self, int stacks) => this._Calc(self, change * -stacks);

    private void _Calc(Unit self, int changeFull) {
        int statOld = self.GetBoolStat(stat);
        int statNew = statOld + changeFull;
        self.SetBoolStat(stat, statNew);

        // todo how does the effect block message appear
        if (!(statOld >= 1 && statNew >= 1) && stat != BoolStats.EffectBlock) {
            LogLib.Add(stat.LogMsgKey.FormatLang(self.FormatName(stat.PossessiveNameInLogMsg),
                statNew, self.Sp.Format()));
        }
    }
}