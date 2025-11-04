namespace API.Battle.BuffEffects;

public class ChangeSp(int change, bool isImmediate = false) : IBuffEffect {
    public void OnGive(Unit self, uint stacks) {
        if (!isImmediate) return;

        string str = Calcs.ChangeSp(self, change);
        if (!self.IsBoolStat(BoolStats.InfiniteSp)) BattleHandlerLib.AppendToLog(str);
    }

    public string[] OnTurnEnd(Unit self, uint stacks) {
        if (isImmediate) return [];

        string str = Calcs.ChangeSp(self, change);
        if (!self.IsBoolStat(BoolStats.InfiniteSp)) return [str];

        return [];
    }
}