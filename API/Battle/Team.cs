namespace API.Battle;

public class Team(params Unit[] units) {
    public Unit[] Units => units;
    public uint Bloom { get; set; } = 0;
}