using System.Collections.Generic;

namespace API.Battle;

public static class PosLib {
    public const int InvalidPos = -1;

    /// <summary>
    /// Returns the pos off spaces below this one, or InvalidPos if it's invalid
    /// </summary>
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

    /// <summary>
    /// Returns the pos directly across from this one
    /// </summary>
    public static int GetAcross(int pos) => pos + (4 * (pos < 4 ? 1 : -1));

    /// <summary>
    /// Returns the poses of the Units on the team except the provided one
    /// </summary>
    public static int[] GetTeamWithout(int pos) {
        int lower = 0;
        int upper = 3;

        if(pos >= 4) {
            lower = 4;
            upper = 7;
        }

        List<int> result = [];
        for (int i = lower; i <= upper; i++) {
            if (i != pos) result.Add(i);
        }

        return result.ToArray();
    }

    /// <summary>
    /// Returns the height 0-3 of pos
    /// </summary>
    public static int GetHeight(int pos) => pos < 4 ? pos : pos - 4;

    /// <summary>
    /// Returns the Side of pos
    /// </summary>
    public static Side GetSide(int pos) => pos < 4 ? Side.Ally : Side.Opponent;

    /// <summary>
    /// Returns the Side of pos2 relative to the Side of pos1
    /// </summary>
    public static Side GetRelativeSide(int pos1, int pos2) => pos2 < 4 ? pos1 < 4 ? Side.Ally : Side.Opponent
        : pos1 >= 4 ? Side.Ally : Side.Opponent;
}