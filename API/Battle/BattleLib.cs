namespace API.Battle;

public static class BattleLib {
    public const int StatMult = 10;

    public static BuffType GetStageBuffType(int stacks) => stacks >= 0 ? BuffType.Buff : BuffType.Debuff;
}