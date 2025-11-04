using API.Extensions;

namespace API.Battle.BuffEffects;

public class ChangeBooleanStat(BoolStat stat, int change) : IBuffEffect {
    public void OnGive(Unit self, uint stacks) => this.Calc(self, (int) (change * stacks));
    public void OnRemove(Unit self, uint stacks) => this.Calc(self, (int) (change * -stacks));

    private void Calc(Unit self, int changeFull) {
        uint statOld = self.GetBoolStat(stat);
        uint statNew = (uint) (statOld + changeFull);
        self.SetBoolStat(stat, statNew);

        // todo how does the effect block message appear
        if (!((statOld >= 1) && (statNew >= 1)) && (stat != BoolStats.EffectBlock)) {
            BattleHandlerLib.AppendToLog(stat.LogMsgKey.FormatLang(self.FormatName(stat.PossessiveNameInLogMsg),
                statNew, self.Sp.Format()));
        }
    }
}