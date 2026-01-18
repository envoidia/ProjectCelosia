namespace API.Battle;

// todo make nullable for blank spots / small teams
public sealed class Team(Unit u1, Unit u2, Unit u3, Unit u4)
{
    public Unit[] Units
    {
        get
        {
            return [u1, u2, u3, u4];
        }
    }

    public int Bloom = 0;
}