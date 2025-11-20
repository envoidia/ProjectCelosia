namespace API.Battle.BuffEffects;

public sealed class ChangeSp(int change, bool isImmediate = false) : IBuffEffect {
    public void OnGive(Unit self, int stacks) {
        if (!isImmediate) return;

        string str = CalcLib.ChangeSp(self, change);
        if (!self.IsBoolStat(BoolStats.InfiniteSp)) BattleHandler.AppendToLog(str);
    }

    public string[] OnTurnEnd(Unit self, int stacks) {
        if (isImmediate) return [];

        string str = CalcLib.ChangeSp(self, change);
        if (!self.IsBoolStat(BoolStats.InfiniteSp)) return [str];

        return [];
    }
}