namespace API.Battle;

public static class Ranges {
    public static readonly Range Self = new(Lang.RangeSelf, 3, Side.Ally, Target.Self) {
        CanTargetSelf = true
    };

    public static readonly Range Other1R = new(Lang.RangeOther1R, 1, Side.Both, Target.Target);
    public static readonly Range Other2R = new(Lang.RangeOther2R, 2, Side.Both, Target.Target);
    public static readonly Range Other3R = new(Lang.RangeOther3R, 3, Side.Both, Target.Target);

    public static readonly Range Other1ROrSelf = new(Lang.RangeOther1ROrSelf, 1, Side.Both, Target.Target) {
        CanTargetSelf = true
    };

    public static readonly Range Other2ROrSelf = new(Lang.RangeOther2ROrSelf, 2, Side.Both, Target.Target) {
        CanTargetSelf = true
    };

    public static readonly Range Other3ROrSelf = new(Lang.RangeOther3ROrSelf, 3, Side.Both, Target.Target) {
        CanTargetSelf = true
    };

    public static readonly Range Others21R = new(Lang.RangeOthers21R, 1, Side.Both, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Others22R = new(Lang.RangeOthers22R, 2, Side.Both, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Others23R = new(Lang.RangeOthers23R, 3, Side.Both, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Ally1R = new(Lang.RangeAlly1R, 1, Side.Ally, Target.Target);
    public static readonly Range Ally2R = new(Lang.RangeAlly2R, 2, Side.Ally, Target.Target);
    public static readonly Range Ally3R = new(Lang.RangeAlly3R, 3, Side.Ally, Target.Target);

    public static readonly Range Allies21R = new(Lang.RangeAllies21R, 1, Side.Ally, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Allies22R = new(Lang.RangeAllies22R, 2, Side.Ally, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Allies23R = new(Lang.RangeAllies23R, 3, Side.Ally, Target.Target) {
        TargetCount = 2
    };

    public static readonly Range Opponent1R = new(Lang.RangeOpponent1R, 1, Side.Opponent, Target.Target);
    public static readonly Range Opponent2R = new(Lang.RangeOpponent2R, 2, Side.Opponent, Target.Target);
    public static readonly Range Opponent3R = new(Lang.RangeOpponent3R, 3, Side.Opponent, Target.Target);

    public static readonly Range Team = new(Lang.RangeTeam, 3, Side.Both, Target.Target, Target.TargetTeam) {
        CanTargetSelf = true
    };

    public static readonly Range All =
        new(Lang.RangeAll, 3, Side.Both, Target.Self, Target.SelfTeam, Target.Target, Target.TargetTeam) {
            CanTargetSelf = true
        };

    public static readonly Range AllOthers = new(Lang.RangeAllOthers, 3, Side.Both, Target.SelfTeam, Target.Target,
        Target.TargetTeam);

    public static readonly Range Adjacent = new(Lang.RangeAdjacent, 1, Side.Both, Target.SelfUp, Target.SelfDown,
        Target.SelfAcross, Target.SelfAcrossUp, Target.SelfAcrossDown);

    public static readonly Range SelfUpDown =
        new(Lang.RangeSelfUpDown, 3, Side.Ally, Target.Self, Target.SelfUp, Target.SelfDown) {
            CanTargetSelf = true
        };

    public static readonly Range Across = new(Lang.RangeAcross, 1, Side.Opponent, Target.SelfAcross);

    public static readonly Range AcrossUpDown = new(Lang.RangeAcrossUpDown, 0, Side.Opponent, Target.SelfAcross,
        Target.SelfAcrossUp, Target.SelfAcrossDown);

    public static readonly Range ColumnOf31R =
        new(Lang.RangeColumnOf31R, 1, Side.Both, Target.Target, Target.TargetUp, Target.TargetDown) {
            CanTargetSelf = true
        };

    public static readonly Range ColumnOf32R =
        new(Lang.RangeColumnOf32R, 2, Side.Both, Target.Target, Target.TargetUp, Target.TargetDown) {
            CanTargetSelf = true
        };
}