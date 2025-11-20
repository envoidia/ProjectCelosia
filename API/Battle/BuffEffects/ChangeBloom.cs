namespace API.Battle.BuffEffects;

public sealed class ChangeBloom(int change, bool isImmediate = false) : IBuffEffect {
    public void OnGive(Unit self, int stacks) {
        if (!isImmediate) return;

        BattleHandler.AppendToLog(CalcLib.ChangeBloom(BattleHandler.Battle.GetTeamAtPos(self.Pos), self.GetSide(),
            change));
    }

    public string[] OnTurnEnd(Unit self, int stacks) {
        if (isImmediate) return [];

        return [CalcLib.ChangeBloom(BattleHandler.Battle.GetTeamAtPos(self.Pos), self.GetSide(), change)];
    }
}