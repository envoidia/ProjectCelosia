namespace API.Battle.BuffEffects;

public class ChangeBloom(int change, bool isImmediate = false) : IBuffEffect {
    public void OnGive(Unit self, uint stacks) {
        if (!isImmediate) return;

        BattleHandlerLib.AppendToLog(Calcs.ChangeBloom(BattleHandlerLib.Battle.GetTeamAtPos(self.Pos), self.GetSide(),
            change));
    }

    public string[] OnTurnEnd(Unit self, uint stacks) {
        if (isImmediate) return [];

        return [Calcs.ChangeBloom(BattleHandlerLib.Battle.GetTeamAtPos(self.Pos), self.GetSide(), change)];
    }
}