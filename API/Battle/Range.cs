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
public sealed class Range : IDescribable, IRegistrable {
    public int RangeVertical { get; }
    public Side Side { get; }
    private Target[] _Targets { get; }

    public bool CanTargetSelf { get; init; } = false;
    public int TargetCount { get; init; } = 1;

    public string KeyName { get; }
    public string KeyDesc { get; }

    public string ModId { get; }
    public string ItemId { get; init; }

    public Range(string modId, string keyName, int rangeVertical, Side side, Target[] targets, string? itemId = null) {
        this.RangeVertical = rangeVertical;
        this.Side = side;
        this._Targets = targets;

        this.KeyName = keyName;
        this.KeyDesc = $"{keyName}Desc";

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
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

    public override string ToString() => $"{base.ToString()}: {this.GetName()} -- {this.GetDesc()}";

    public string GetName(ThemeColor color) => color.Str() + this.GetLang();
    public string GetName() => this.GetName(ThemeColor.White);
    public string GetDesc() => this.KeyDesc.GetLang(this.ModId);

}

public static class Ranges {
    public static readonly Range Self = new(Core.Id, "RangeSelf", 3, Side.Ally, [Target.Self]) {
        CanTargetSelf = true
    };

    public static readonly Range Other1R = new(Core.Id, "RangeOther1R", 1, Side.Both, [Target.Target]);
    public static readonly Range Other2R = new(Core.Id, "RangeOther2R", 2, Side.Both, [Target.Target]);
    public static readonly Range Other3R = new(Core.Id, "RangeOther3R", 3, Side.Both, [Target.Target]);

    public static readonly Range Other1ROrSelf = new(Core.Id, "RangeOther1ROrSelf", 1, Side.Both, [Target.Target]) {
        CanTargetSelf = true
    };

    public static readonly Range Other2ROrSelf = new(Core.Id, "RangeOther2ROrSelf", 2, Side.Both, [Target.Target]) {
        CanTargetSelf = true
    };

    public static readonly Range Other3ROrSelf = new(Core.Id, "RangeOther3ROrSelf", 3, Side.Both, [Target.Target]) {
        CanTargetSelf = true
    };

    public static readonly Range Others21R = new(Core.Id, "RangeOthers21R", 1, Side.Both, [Target.Target]) {
        TargetCount = 2
    };

    public static readonly Range Others22R = new(Core.Id, "RangeOthers22R", 2, Side.Both, [Target.Target]) {
        TargetCount = 2
    };

    public static readonly Range Others23R = new(Core.Id, "RangeOthers23R", 3, Side.Both, [Target.Target]) {
        TargetCount = 2
    };

    public static readonly Range Ally1R = new(Core.Id, "RangeAlly1R", 1, Side.Ally, [Target.Target]);
    public static readonly Range Ally2R = new(Core.Id, "RangeAlly2R", 2, Side.Ally, [Target.Target]);
    public static readonly Range Ally3R = new(Core.Id, "RangeAlly3R", 3, Side.Ally, [Target.Target]);

    public static readonly Range Allies21R = new(Core.Id, "RangeAllies21R", 1, Side.Ally, [Target.Target]) {
        TargetCount = 2
    };

    public static readonly Range Allies22R = new(Core.Id, "RangeAllies22R", 2, Side.Ally, [Target.Target]) {
        TargetCount = 2
    };

    public static readonly Range Allies23R = new(Core.Id, "RangeAllies23R", 3, Side.Ally, [Target.Target]) {
        TargetCount = 2
    };

    public static readonly Range Opponent1R = new(Core.Id, "RangeOpponent1R", 1, Side.Opponent, [Target.Target]);
    public static readonly Range Opponent2R = new(Core.Id, "RangeOpponent2R", 2, Side.Opponent, [Target.Target]);
    public static readonly Range Opponent3R = new(Core.Id, "RangeOpponent3R", 3, Side.Opponent, [Target.Target]);

    public static readonly Range Team = new(Core.Id, "RangeTeam", 3, Side.Both, [Target.Target, TargetTeam]) {
        CanTargetSelf = true
    };

    public static readonly Range All =
        new(Core.Id, "RangeAll", 3, Side.Both, [Target.Self, SelfTeam, Target.Target, TargetTeam]) {
            CanTargetSelf = true
        };

    public static readonly Range AllOthers = new(Core.Id, "RangeAllOthers", 3, Side.Both, [SelfTeam, Target.Target,
        TargetTeam]);

    public static readonly Range Adjacent = new(Core.Id, "RangeAdjacent", 1, Side.Both, [SelfUp, SelfDown,
        SelfAcross, SelfAcrossUp, SelfAcrossDown]);

    public static readonly Range SelfUpDown =
        new(Core.Id, "RangeSelfUpDown", 3, Side.Ally, [Target.Self, SelfUp, SelfDown]) {
            CanTargetSelf = true
        };

    public static readonly Range Across = new(Core.Id, "RangeAcross", 1, Side.Opponent, [SelfAcross]);

    public static readonly Range AcrossUpDown = new(Core.Id, "RangeAcrossUpDown", 0, Side.Opponent, [SelfAcross,
        SelfAcrossUp, SelfAcrossDown]);

    public static readonly Range ColumnOf31R =
        new(Core.Id, "RangeColumnOf31R", 1, Side.Both, [Target.Target, TargetUp, TargetDown]) {
            CanTargetSelf = true
        };

    public static readonly Range ColumnOf32R =
        new(Core.Id, "RangeColumnOf32R", 2, Side.Both, [Target.Target, TargetUp, TargetDown]) {
            CanTargetSelf = true
        };
}