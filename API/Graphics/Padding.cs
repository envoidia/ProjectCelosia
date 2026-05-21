namespace API.Graphics;

public readonly record struct Padding(int L, int R, int T, int B)
{
    public static readonly Padding Zero = new(0);

    public int LR
    {
        get
        {
            return this.L + this.R;
        }
    }

    public int LT
    {
        get
        {
            return this.L + this.T;
        }
    }

    public int TB
    {
        get
        {
            return this.T + this.B;
        }
    }

    public int RB
    {
        get
        {
            return this.R + this.B;
        }
    }

    public Padding(int lrtb) : this(lrtb, lrtb, lrtb, lrtb) { }
    public Padding(int lr, int tb) : this(lr, lr, tb, tb) { }
}
