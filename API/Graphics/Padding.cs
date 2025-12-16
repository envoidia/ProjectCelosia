namespace API.Graphics;

public readonly record struct Padding(int L = 0, int R = 0, int T = 0, int B = 0) {
    public static readonly Padding Zero = new(0);

    public int LR => this.L + this.R;
    public int TB => this.T + this.B;

    public Padding(int lrtb) : this(lrtb, lrtb, lrtb, lrtb) { }
    public Padding(int lr, int tb) : this(lr, lr, tb, tb) { }
}
