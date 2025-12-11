using System;

namespace API.Graphics;

public readonly record struct Padding(int L = 0, int R = 0, int T = 0, int B = 0) {
    public static readonly Padding Zero = new(0);

    public int LR => this.L + this.R;
    public int TB => this.T + this.B;

    public Padding(int n) : this(n, n, n, n) { }
    public Padding(int x, int y) : this(x, x, y, y) { }
}
