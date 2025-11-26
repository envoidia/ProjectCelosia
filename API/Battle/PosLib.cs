using System.Collections.Generic;
using API.Battle.State;

namespace API.Battle;

public static class PosLib {

    public const int HighestAlly = BattleLib.TeamSize - 1;
    public const int LowestOpp = HighestAlly + 1;
    public const int HighestOpp = BattleLib.UnitCount - 1;
    public const int Invalid = -1;

    /// <summary>
    /// Returns the pos off spaces below this one, or InvalidPos if it's invalid
    /// </summary>
    public static int GetUpDown(int pos, int off) {
        int posNew = pos + off;

        if ((pos < LowestOpp && posNew >= LowestOpp) ||
            (pos >= LowestOpp && posNew is < LowestOpp or > HighestOpp)) {
            return Invalid;
        }

        return posNew;
    }

    /// <summary>
    /// Returns the pos directly across from this one
    /// </summary>
    public static int GetAcross(int pos) => pos + (LowestOpp * (pos < LowestOpp ? 1 : -1));

    /// <summary>
    /// Returns the poses of the Units on the team except the provided one
    /// </summary>
    public static int[] GetTeamWithout(int pos) {
        int lower = 0;
        int upper = HighestAlly;

        if (pos >= LowestOpp) {
            lower = LowestOpp;
            upper = HighestOpp;
        }

        List<int> result = [];
        for (int i = lower; i <= upper; i++) {
            if (i != pos) result.Add(i);
        }

        return [.. result];
    }

    /// <summary>
    /// Returns the height 0-3 of pos
    /// </summary>
    public static int GetHeight(int pos) => pos < LowestOpp ? pos : pos - LowestOpp;

    /// <summary>
    /// Returns the Side of pos
    /// </summary>
    public static Side GetSide(int pos) => pos < LowestOpp ? Side.Ally : Side.Opponent;

    /// <summary>
    /// Returns the Side of pos2 relative to the Side of pos1
    /// </summary>
    public static Side GetRelativeSide(int pos1, int pos2) {
        if (pos2 < LowestOpp) return pos1 < LowestOpp ? Side.Ally : Side.Opponent;
        return pos1 >= LowestOpp ? Side.Ally : Side.Opponent;
    }
}