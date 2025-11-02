using System.Collections.Generic;
using API.Entity;
using static API.Battle.PosLib;
using static API.Battle.Target;

namespace API.Battle;

public class Range : NamedEntity {
    public int RangeVertical { get; }
    public Side Side { get; }
    private Target[] Targets { get; }

    public bool CanTargetSelf { get; init; } = false;
    public int TargetCount { get; init; } = 1;

    public Range(string keyName, int rangeVertical, Side side, params Target[] targets) : base(keyName) {
        this.RangeVertical = rangeVertical;
        this.Side = side;
        this.Targets = targets;
        Core.Ranges.Add(this);
    }

    public int[] GetTargetPositions(int posSelf, int posTarget) {
        List<int> pos = [];

        foreach (Target target in this.Targets) {
            switch (target) {
                case Self:
                    pos.Add(posSelf);
                    break;
                case SelfUp:
                    pos.Add(GetUpDown(posSelf, -1));
                    break;
                case SelfDown:
                    pos.Add(GetUpDown(posSelf, 1));
                    break;
                case SelfAcross:
                    pos.Add(GetAcross(posSelf));
                    break;
                case SelfAcrossUp:
                    pos.Add(GetUpDown(GetAcross(posSelf), -1));
                    break;
                case SelfAcrossDown:
                    pos.Add(GetUpDown(GetAcross(posSelf), 1));
                    break;
                case SelfTeam:
                    pos.AddRange(GetTeamWithout(posSelf));
                    break;
                case Target.Target:
                    pos.Add(posTarget);
                    break;
                case TargetUp:
                    pos.Add(GetUpDown(posTarget, -1));
                    break;
                case TargetDown:
                    pos.Add(GetUpDown(posTarget, 1));
                    break;
                case TargetTeam:
                    pos.AddRange(GetTeamWithout(posTarget));
                    break;
            }
        }

        return pos.ToArray();
    }
}

public static class Ranges {
    public static readonly Range Self = new("RangeSelf", 3, Side.Ally, Target.Self) {
        CanTargetSelf = true
    };

    public static readonly Range Other1R = new("RangeOther1R", 1, Side.Both, Target.Target);
    public static readonly Range Other2R = new("RangeOther2R", 2, Side.Both, Target.Target);
    public static readonly Range Other3R = new("RangeOther3R", 3, Side.Both, Target.Target);

    public static readonly Range Other1ROrSelf = new("RangeOther1ROrSelf", 1, Side.Both, Target.Target) {
        CanTargetSelf = true
    };

    public static readonly Range Other2ROrSelf = new("RangeOther2ROrSelf", 2, Side.Both, Target.Target) {
        CanTargetSelf = true
    };

    public static readonly Range Other3ROrSelf = new("RangeOther3ROrSelf", 3, Side.Both, Target.Target) {
        CanTargetSelf = true
    };

    public static readonly Range Others21R = new("RangeOthers21R", 1, Side.Both, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Others22R = new("RangeOthers22R", 2, Side.Both, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Others23R = new("RangeOthers23R", 3, Side.Both, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Ally1R = new("RangeAlly1R", 1, Side.Ally, Target.Target);
    public static readonly Range Ally2R = new("RangeAlly2R", 2, Side.Ally, Target.Target);
    public static readonly Range Ally3R = new("RangeAlly3R", 3, Side.Ally, Target.Target);

    public static readonly Range Allies21R = new("RangeAllies21R", 1, Side.Ally, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Allies22R = new("RangeAllies22R", 2, Side.Ally, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Allies23R = new("RangeAllies23R", 3, Side.Ally, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Opponent1R = new("RangeOpponent1R", 1, Side.Opponent, Target.Target);
    public static readonly Range Opponent2R = new("RangeOpponent2R", 2, Side.Opponent, Target.Target);
    public static readonly Range Opponent3R = new("RangeOpponent3R", 3, Side.Opponent, Target.Target);

    public static readonly Range Team = new("RangeTeam", 3, Side.Both, Target.Target, TargetTeam) {
        CanTargetSelf = true
    };

    public static readonly Range All =
        new("RangeAll", 3, Side.Both, Target.Self, SelfTeam, Target.Target, TargetTeam) {
            CanTargetSelf = true
        };

    public static readonly Range AllOthers = new("RangeAllOthers", 3, Side.Both, SelfTeam, Target.Target,
        TargetTeam);

    public static readonly Range Adjacent = new("RangeAdjacent", 1, Side.Both, SelfUp, SelfDown,
        SelfAcross, SelfAcrossUp, SelfAcrossDown);

    public static readonly Range SelfUpDown =
        new("RangeSelfUpDown", 3, Side.Ally, Target.Self, SelfUp, SelfDown) {
            CanTargetSelf = true
        };

    public static readonly Range Across = new("RangeAcross", 1, Side.Opponent, SelfAcross);

    public static readonly Range AcrossUpDown = new("RangeAcrossUpDown", 0, Side.Opponent, SelfAcross,
        SelfAcrossUp, SelfAcrossDown);

    public static readonly Range ColumnOf31R =
        new("RangeColumnOf31R", 1, Side.Both, Target.Target, TargetUp, TargetDown) {
            CanTargetSelf = true
        };

    public static readonly Range ColumnOf32R =
        new("RangeColumnOf32R", 2, Side.Both, Target.Target, TargetUp, TargetDown) {
            CanTargetSelf = true
        };
}