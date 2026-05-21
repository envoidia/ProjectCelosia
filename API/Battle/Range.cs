using System;
using System.Collections.Generic;
using API.Battle.State;
using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

/// <summary>
/// The reach that a skill can have
/// </summary>
// todo add descs or only inameable?
public sealed class Range : IDescribable, IRegistrable
{
    public int RangeVertical { get; }
    public Side Side { get; }
    private Target[] _Targets { get; }

    public bool CanTargetSelf { get; init; } = false;
    public int TargetCount { get; init; } = 1;

    public string KeyName { get; }
    public string KeyDesc { get; }

    public string ModId { get; }
    public string ItemId { get; init; }

    public Range(string modId, string keyName, int rangeVertical, Side side, Target[] targets, string? itemId = null)
    {
        this.RangeVertical = rangeVertical;
        this.Side = side;
        this._Targets = targets;

        this.KeyName = keyName;
        this.KeyDesc = $"{keyName}Desc";

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
    }

    /// <returns>
    /// All positions that this will impact for the given self and target positions
    /// </returns>
    public List<int> GetTargetPositions(int posSelf, int posTarget)
    {
        List<int> pos = [];

        foreach (Target target in this._Targets)
        {
            switch (target)
            {
                case Target.Self:
                    pos.Add(posSelf);
                    break;
                case Target.SelfUp:
                    pos.Add(PosLib.GetUpDown(posSelf, -1));
                    break;
                case Target.SelfDown:
                    pos.Add(PosLib.GetUpDown(posSelf, 1));
                    break;
                case Target.SelfAcross:
                    pos.Add(PosLib.GetAcross(posSelf));
                    break;
                case Target.SelfAcrossUp:
                    pos.Add(PosLib.GetUpDown(PosLib.GetAcross(posSelf), -1));
                    break;
                case Target.SelfAcrossDown:
                    pos.Add(PosLib.GetUpDown(PosLib.GetAcross(posSelf), 1));
                    break;
                case Target.SelfTeam:
                    pos.AddRange(PosLib.GetTeamWithout(posSelf));
                    break;
                case Target.Target:
                    pos.Add(posTarget);
                    break;
                case Target.TargetUp:
                    pos.Add(PosLib.GetUpDown(posTarget, -1));
                    break;
                case Target.TargetDown:
                    pos.Add(PosLib.GetUpDown(posTarget, 1));
                    break;
                case Target.TargetTeam:
                    pos.AddRange(PosLib.GetTeamWithout(posTarget));
                    break;
            }
        }

        return pos;
    }

    /// <returns>
    /// Whether this can reach the given target position from the given self position with the given range mod
    /// </returns>
    public bool CanReach(int posSelf, int posTarget, int modRange)
    {
        // Check for disallowed self-targeting
        if (!this.CanTargetSelf && posTarget == posSelf)
        {
            return false;
        }

        // Check if target is within vertical range
        if (Math.Abs(PosLib.GetHeight(posSelf)
            - PosLib.GetHeight(posTarget)) > this.RangeVertical + modRange)
        {
            return false;
        }

        // Check if the targeted side is allowed
        return this.Side == Side.Both || this.Side == PosLib.GetRelativeSide(posSelf, posTarget);
    }

    /// <returns>
    /// For each position, whether this can target it for the given self position and range mod
    /// </returns>
    public bool[] GetMainTargetPositions(int posSelf, int modRange)
    {
        bool[] pos = new bool[BattleLib.UnitCount];

        for (int i = 0; i < BattleLib.UnitCount; i++)
        {
            pos[i] = this.CanReach(posSelf, i, modRange);
        }

        return pos;
    }

    public override string ToString()
    {
        return $"{base.ToString()}: {this.GetName()} -- {this.GetDesc()}";
    }

    public string GetName(ThemeColor color)
    {
        return color.Str + this.GetLang();
    }

    public string GetName()
    {
        return this.GetName(ThemeColor.Fg);
    }

    public string GetDesc()
    {
        return this.KeyDesc.GetLang();
    }
}
