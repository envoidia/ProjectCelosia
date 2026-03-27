namespace API.Battle;

// todo make nullable for blank spots / small teams
public sealed class Team(Unit u1, Unit u2, Unit u3, Unit u4)
{
    // todo allow updating
    public readonly Unit[] Units = [u1, u2, u3, u4];

    public const int MaxBloom = 1000;
    public int Bloom = 0;
}