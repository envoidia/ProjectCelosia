using System.Collections.Generic;

namespace API.Battle;

public static class PosLib {
    public const int InvalidPos = int.MaxValue;

    // Returns the pos off spaces below this one, or int.MaxValue if it's invalid
    public static int GetUpDown(int pos, int off) {
        int posNew = pos + off;
        switch (pos) {
            case < 4 when posNew > 3:
            case >= 4 when posNew is < 4 or > 7:
                return InvalidPos;
            default:
                return posNew;
        }
    }

    // Returns the pos directly across from this one
    public static int GetAcross(int pos) => pos + (4 * (pos < 4 ? 1 : -1));

    // Returns the poses of the Units on the team except the provided one
    public static int[] GetTeamWithout(int pos) {
        int lower = pos < 4 ? 0 : 4;
        int upper = pos < 4 ? 3 : 7;

        List<int> result = [];
        for (int i = lower; i <= upper; i++) {
            if (i != pos) result.Add(i);
        }

        return result.ToArray();
    }

    // Returns the height 0-3 of pos
    public static int GetHeight(int pos) => pos < 4 ? pos : pos - 4;

    // Returns the Side of pos
    public static Side GetSide(int pos) => pos < 4 ? Side.Ally : Side.Opponent;

    // Returns the Side of pos2 relative to the Side of pos1
    public static Side GetRelativeSide(int pos1, int pos2) => pos2 < 4 ? pos1 < 4 ? Side.Ally : Side.Opponent
        : pos1 >= 4 ? Side.Ally : Side.Opponent;
}