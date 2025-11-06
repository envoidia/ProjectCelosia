namespace API.Battle;

public class BuffInstance(Buff buff, uint turns, uint stacks) {
    public Buff Buff => buff;
    public uint Turns { get; set; } = turns;
    public uint Stacks { get; set; } = stacks;

    public string GetTurnsStacksFormatted() => "x" + this.Stacks + "(" + this.Turns + ")";
}