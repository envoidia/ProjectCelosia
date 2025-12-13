using System.Collections.Generic;
using API.Battle.State;

namespace API.Battle;

public static class PosLib {
    public const int Lowest = 0;
    public const int Highest = BattleLib.UnitCount - 1;
    public const int HighestAlly = BattleLib.TeamSize - 1;
    public const int LowestOpp = HighestAlly + 1;
    public const int HighestOpp = Highest;
    public const int Invalid = -1;

    /// <returns>
    /// The <c>pos</c> <c>off</c> spaces below this one, or <c>InvalidPos</c> if it's invalid
    /// </returns>
    public static int GetUpDown(int pos, int off) {
        int posNew = pos + off;

        if ((pos < LowestOpp && posNew >= LowestOpp) ||
            (pos >= LowestOpp && posNew is < LowestOpp or > HighestOpp)) {
            return Invalid;
        }

        return posNew;
    }

    /// <returns>
    /// The <c>pos</c> directly across from this one
    /// </returns>
    public static int GetAcross(int pos) => pos + (LowestOpp * (pos < LowestOpp ? 1 : -1));

    /// <returns>
    /// The <c>pos</c>es of the <c>Units</c> on the <c>Team</c> except the provided one
    /// </returns>
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

    /// <returns>
    /// The height 0-3 of <c>pos</c>
    /// </returns>
    public static int GetHeight(int pos) => pos < LowestOpp ? pos : pos - LowestOpp;

    /// <returns>
    /// The <c>Side</c> of <c>pos</c>
    /// </returns>
    public static Side GetSide(int pos) => pos < LowestOpp ? Side.Ally : Side.Opponent;

    /// <returns>
    /// The <c>Side</c> of <c>pos2</c> relative to the <c>Side</c> of <c>pos1</c>
    /// </returns>
    public static Side GetRelativeSide(int pos1, int pos2) {
        if (pos2 < LowestOpp) return pos1 < LowestOpp ? Side.Ally : Side.Opponent;
        return pos1 >= LowestOpp ? Side.Ally : Side.Opponent;
    }
}