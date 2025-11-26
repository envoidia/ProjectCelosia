namespace API.Battle;

public sealed class BuffInstance(Buff buff, int turns, int stacks) {
    /// <summary>
    /// >= 1,000 turns == infinite
    /// </summary>
    public const int InfiniteTurns = 1000;

    public Buff Buff => buff;
    public int Turns { get; set; } = turns;
    public int Stacks { get; set; } = stacks;

    public string GetTurnsStacksFormatted() => $"x{this.Stacks}({this.Turns})";
}