using API.Battle.State;

namespace API.Battle.BuffEffects;

public sealed class ChangeBloom(int change, bool isImmediate = false) : IBuffEffect {
    public void OnGive(Unit self, int stacks) {
        if (!isImmediate) return;

        MenuLog.Add(CalcLib.ChangeBloom(BattleLib.Battle.GetTeamAtPos(self.Pos), self.GetSide(),
            change));
    }

    public string[] OnTurnEnd(Unit self, int stacks) {
        if (isImmediate) return [];

        return [CalcLib.ChangeBloom(BattleLib.Battle.GetTeamAtPos(self.Pos), self.GetSide(), change)];
    }
}