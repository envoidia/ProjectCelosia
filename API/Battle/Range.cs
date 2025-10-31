using System.Collections.Generic;
using System.ComponentModel;
using static API.Battle.PosLib;
using static API.Battle.Target;

namespace API.Battle;

public class Range {
    public string Name { get; }
    public int RangeVertical { get; }
    public Side Side { get; }
    private Target[] Targets { get; }

    public bool CanTargetSelf { get; init; } = false;
    public int TargetCount { get; init; } = 1;

    public Range(string name, int rangeVertical, Side side, params Target[] targets) {
        this.Name = name;
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