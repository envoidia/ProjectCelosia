namespace API.Battle;

public class Team(params Unit[] units) {
    public Unit[] Units { get; } = units;
    public uint Bloom { get; set; } = 0;
}