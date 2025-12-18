using System.Collections.Generic;
using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;
using static API.Battle.PosLib;
using static API.Battle.Target;

namespace API.Battle;

/// <summary>
/// The reach that a skill can have
/// </summary>
public sealed class Range : _IModItem, IDescribable {
    public int RangeVertical { get; }
    public Side Side { get; }
    private Target[] _Targets { get; }

    public bool CanTargetSelf { get; init; } = false;
    public int TargetCount { get; init; } = 1;

    public GameMod? Source { get; }
    public string KeyName { get; }
    public string KeyDesc { get; }

    public Range(GameMod? source, string keyName, int rangeVertical, Side side, params Target[] targets) {
        this.Source = source;
        this.KeyName = keyName;
        this.KeyDesc = $"{keyName}Desc";

        this.RangeVertical = rangeVertical;
        this.Side = side;
        this._Targets = targets;

        Core.Ranges.Add(this);
    }

    public int[] GetTargetPositions(int posSelf, int posTarget) {
        List<int> pos = [];

        foreach (Target target in this._Targets) {
            switch (target) {
                case Self: pos.Add(posSelf); break;
                case SelfUp: pos.Add(GetUpDown(posSelf, -1)); break;
                case SelfDown: pos.Add(GetUpDown(posSelf, 1)); break;
                case SelfAcross: pos.Add(GetAcross(posSelf)); break;
                case SelfAcrossUp: pos.Add(GetUpDown(GetAcross(posSelf), -1)); break;
                case SelfAcrossDown: pos.Add(GetUpDown(GetAcross(posSelf), 1)); break;
                case SelfTeam: pos.AddRange(GetTeamWithout(posSelf)); break;
                case Target.Target: pos.Add(posTarget); break;
                case TargetUp: pos.Add(GetUpDown(posTarget, -1)); break;
                case TargetDown: pos.Add(GetUpDown(posTarget, 1)); break;
                case TargetTeam: pos.AddRange(GetTeamWithout(posTarget)); break;
            }
        }

        return [.. pos];
    }

    public string GetName(ThemeColor color, GameMod? mod = null) => color.Str() + this.KeyName.GetLang(mod);
    public string GetName(GameMod? mod = null) => this.GetName(ThemeColor.White, mod);
    public string GetDesc(GameMod? mod = null) => this.KeyDesc.GetLang(mod);

}

public static class Ranges {
    public static readonly Range Self = new(null, "RangeSelf", 3, Side.Ally, Target.Self) {
        CanTargetSelf = true
    };

    public static readonly Range Other1R = new(null, "RangeOther1R", 1, Side.Both, Target.Target);
    public static readonly Range Other2R = new(null, "RangeOther2R", 2, Side.Both, Target.Target);
    public static readonly Range Other3R = new(null, "RangeOther3R", 3, Side.Both, Target.Target);

    public static readonly Range Other1ROrSelf = new(null, "RangeOther1ROrSelf", 1, Side.Both, Target.Target) {
        CanTargetSelf = true
    };

    public static readonly Range Other2ROrSelf = new(null, "RangeOther2ROrSelf", 2, Side.Both, Target.Target) {
        CanTargetSelf = true
    };

    public static readonly Range Other3ROrSelf = new(null, "RangeOther3ROrSelf", 3, Side.Both, Target.Target) {
        CanTargetSelf = true
    };

    public static readonly Range Others21R = new(null, "RangeOthers21R", 1, Side.Both, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Others22R = new(null, "RangeOthers22R", 2, Side.Both, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Others23R = new(null, "RangeOthers23R", 3, Side.Both, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Ally1R = new(null, "RangeAlly1R", 1, Side.Ally, Target.Target);
    public static readonly Range Ally2R = new(null, "RangeAlly2R", 2, Side.Ally, Target.Target);
    public static readonly Range Ally3R = new(null, "RangeAlly3R", 3, Side.Ally, Target.Target);

    public static readonly Range Allies21R = new(null, "RangeAllies21R", 1, Side.Ally, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Allies22R = new(null, "RangeAllies22R", 2, Side.Ally, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Allies23R = new(null, "RangeAllies23R", 3, Side.Ally, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Opponent1R = new(null, "RangeOpponent1R", 1, Side.Opponent, Target.Target);
    public static readonly Range Opponent2R = new(null, "RangeOpponent2R", 2, Side.Opponent, Target.Target);
    public static readonly Range Opponent3R = new(null, "RangeOpponent3R", 3, Side.Opponent, Target.Target);

    public static readonly Range Team = new(null, "RangeTeam", 3, Side.Both, Target.Target, TargetTeam) {
        CanTargetSelf = true
    };

    public static readonly Range All =
        new(null, "RangeAll", 3, Side.Both, Target.Self, SelfTeam, Target.Target, TargetTeam) {
            CanTargetSelf = true
        };

    public static readonly Range AllOthers = new(null, "RangeAllOthers", 3, Side.Both, SelfTeam, Target.Target,
        TargetTeam);

    public static readonly Range Adjacent = new(null, "RangeAdjacent", 1, Side.Both, SelfUp, SelfDown,
        SelfAcross, SelfAcrossUp, SelfAcrossDown);

    public static readonly Range SelfUpDown =
        new(null, "RangeSelfUpDown", 3, Side.Ally, Target.Self, SelfUp, SelfDown) {
            CanTargetSelf = true
        };

    public static readonly Range Across = new(null, "RangeAcross", 1, Side.Opponent, SelfAcross);

    public static readonly Range AcrossUpDown = new(null, "RangeAcrossUpDown", 0, Side.Opponent, SelfAcross,
        SelfAcrossUp, SelfAcrossDown);

    public static readonly Range ColumnOf31R =
        new(null, "RangeColumnOf31R", 1, Side.Both, Target.Target, TargetUp, TargetDown) {
            CanTargetSelf = true
        };

    public static readonly Range ColumnOf32R =
        new(null, "RangeColumnOf32R", 2, Side.Both, Target.Target, TargetUp, TargetDown) {
            CanTargetSelf = true
        };
}