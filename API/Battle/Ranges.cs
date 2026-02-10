namespace API.Battle;

public static class Ranges
{
    public static readonly Range Self = new(Core.Id, "RangeSelf", 3,
        Side.Ally, [Target.Self])
    {
        CanTargetSelf = true
    };

    public static readonly Range Other1R = new(Core.Id, "RangeOther1R", 1,
        Side.Both, [Target.Target]);

    public static readonly Range Other2R = new(Core.Id, "RangeOther2R", 2,
        Side.Both, [Target.Target]);

    public static readonly Range Other3R = new(Core.Id, "RangeOther3R", 3,
        Side.Both, [Target.Target]);

    public static readonly Range Other1ROrSelf = new(Core.Id, "RangeOther1ROrSelf", 1,
        Side.Both, [Target.Target])
    {
        CanTargetSelf = true
    };

    public static readonly Range Other2ROrSelf = new(Core.Id, "RangeOther2ROrSelf", 2,
        Side.Both, [Target.Target])
    {
        CanTargetSelf = true
    };

    public static readonly Range Other3ROrSelf = new(Core.Id, "RangeOther3ROrSelf", 3,
        Side.Both, [Target.Target])
    {
        CanTargetSelf = true
    };

    public static readonly Range Others21R = new(Core.Id, "RangeOthers21R", 1,
        Side.Both, [Target.Target])

    {
        TargetCount = 2
    };

    public static readonly Range Others22R = new(Core.Id, "RangeOthers22R", 2,
        Side.Both, [Target.Target])

    {
        TargetCount = 2
    };

    public static readonly Range Others23R = new(Core.Id, "RangeOthers23R", 3,
        Side.Both, [Target.Target])

    {
        TargetCount = 2
    };

    public static readonly Range Ally1R = new(Core.Id, "RangeAlly1R", 1,
        Side.Ally, [Target.Target]);

    public static readonly Range Ally2R = new(Core.Id, "RangeAlly2R", 2,
        Side.Ally, [Target.Target]);

    public static readonly Range Ally3R = new(Core.Id, "RangeAlly3R", 3,
        Side.Ally, [Target.Target]);


    public static readonly Range Allies21R = new(Core.Id, "RangeAllies21R", 1,
        Side.Ally, [Target.Target])

    {
        TargetCount = 2
    };

    public static readonly Range Allies22R = new(Core.Id, "RangeAllies22R", 2,
        Side.Ally, [Target.Target])

    {
        TargetCount = 2
    };

    public static readonly Range Allies23R = new(Core.Id, "RangeAllies23R", 3,
        Side.Ally, [Target.Target])

    {
        TargetCount = 2
    };

    public static readonly Range Opponent1R = new(Core.Id, "RangeOpponent1R", 1,
        Side.Opponent, [Target.Target]);

    public static readonly Range Opponent2R = new(Core.Id, "RangeOpponent2R", 2,
        Side.Opponent, [Target.Target]);

    public static readonly Range Opponent3R = new(Core.Id, "RangeOpponent3R", 3,
        Side.Opponent, [Target.Target]);


    public static readonly Range Team = new(Core.Id, "RangeTeam", 3,
        Side.Both, [Target.Target, Target.TargetTeam])

    {
        CanTargetSelf = true
    };

    public static readonly Range All =
        new(Core.Id, "RangeAll", 3, Side.Both,
            [Target.Self, Target.SelfTeam, Target.Target, Target.TargetTeam])
        {
            CanTargetSelf = true
        };

    public static readonly Range AllOthers = new(Core.Id, "RangeAllOthers", 3,
        Side.Both, [Target.SelfTeam, Target.Target, Target.TargetTeam]);

    public static readonly Range Adjacent = new(Core.Id, "RangeAdjacent", 1,
        Side.Both, [Target.SelfUp, Target.SelfDown,
        Target.SelfAcross, Target.SelfAcrossUp, Target.SelfAcrossDown]);

    public static readonly Range SelfUpDown = new(Core.Id, "RangeSelfUpDown", 3,
        Side.Ally, [Target.Self, Target.SelfUp, Target.SelfDown])
    {
        CanTargetSelf = true
    };

    public static readonly Range Across = new(Core.Id, "RangeAcross", 1,
        Side.Opponent, [Target.SelfAcross]);

    public static readonly Range AcrossUpDown = new(Core.Id, "RangeAcrossUpDown", 0,
        Side.Opponent, [Target.SelfAcross, Target.SelfAcrossUp, Target.SelfAcrossDown]);

    public static readonly Range ColumnOf31R = new(Core.Id, "RangeColumnOf31R", 1,
        Side.Both, [Target.Target, Target.TargetUp, Target.TargetDown])
    {
        CanTargetSelf = true
    };

    public static readonly Range ColumnOf32R = new(Core.Id, "RangeColumnOf32R", 2,
        Side.Both, [Target.Target, Target.TargetUp, Target.TargetDown])
    {
        CanTargetSelf = true
    };
}