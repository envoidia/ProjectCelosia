using System.Collections.Generic;

namespace API.Battle;

public static class PosLib {
    public const uint InvalidPos = uint.MaxValue;

    // Returns the pos off spaces below this one, or uint.MaxValue if it's invalid
    public static uint GetUpDown(uint pos, int off) {
        uint posNew = (uint) (pos + off);
        switch (pos) {
            case < 4 when posNew > 3:
            case >= 4 when posNew is < 4 or > 7:
                return InvalidPos;
            default:
                return posNew;
        }
    }

    // Returns the pos directly across from this one
    public static uint GetAcross(uint pos) => (uint) (pos + (4 * (pos < 4 ? 1 : -1)));

    // Returns the poses of the Units on the team except the provided one
    public static uint[] GetTeamWithout(uint pos) {
        uint lower = pos < 4 ? 0u : 4u;
        uint upper = pos < 4 ? 3u : 7u;

        List<uint> result = [];
        for (uint i = lower; i <= upper; i++) {
            if (i != pos) result.Add(i);
        }

        return result.ToArray();
    }

    // Returns the height 0-3 of pos
    public static uint GetHeight(uint pos) => pos < 4 ? pos : pos - 4;

    // Returns the Side of pos
    public static Side GetSide(uint pos) => pos < 4u ? Side.Ally : Side.Opponent;

    // Returns the Side of pos2 relative to the Side of pos1
    public static Side GetRelativeSide(uint pos1, uint pos2) => pos2 < 4 ? pos1 < 4 ? Side.Ally : Side.Opponent
        : pos1 >= 4 ? Side.Ally : Side.Opponent;
}