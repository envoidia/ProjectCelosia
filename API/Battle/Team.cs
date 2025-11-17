namespace API.Battle;

public sealed class Team(params Unit[] units) {
    public Unit[] Units => units;
    public int Bloom { get; set; } = 0;
}