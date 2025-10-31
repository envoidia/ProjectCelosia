using API.Util;

namespace API.Battle;

public static class AffLib {
    public static readonly ExtrapolatingArray DmgDealt =
        new([300, 500, 650, 800, 900, 1000, 1100, 1200, 1350, 1500, 1700], 5, 200, -200);

    public static readonly ExtrapolatingArray DmgTaken =
        new([2500, 2000, 1700, 1400, 1200, 1000, 900, 800, 650, 500, 0], 5, 0, 500);

    public static readonly ExtrapolatingArray SpCost =
        new([1700, 1500, 1300, 1200, 1100, 1000, 950, 900, 850, 800, 750], 5, -50, 200);
}