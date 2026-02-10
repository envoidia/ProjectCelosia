using System.Collections.Generic;
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
        return this.GetName(ThemeColor.White);
    }

    public string GetDesc()
    {
        return this.KeyDesc.GetLang();
    }
}