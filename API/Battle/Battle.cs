using System.Collections.Generic;
using System.Linq;

namespace API.Battle;

public sealed class Battle
{
    public Team PlayerTeam { get; }
    public Team OpponentTeam { get; }

    /// <summary>
    /// Current turn, starting at 1
    /// </summary>
    public int Turn = 1;

    public Battle(Team playerTeam, Team opponentTeam)
    {
        this.PlayerTeam = playerTeam;
        this.OpponentTeam = opponentTeam;

        // Assign disambiguation identifiers to duplicate UnitTypes
        Unit[] units = this.GetAllUnits();

        Dictionary<UnitType, int> countDict = units.GroupBy(static u => u.UnitType)
            .ToDictionary(static g => g.Key,
                static g => g.Count());
        Dictionary<UnitType, int> counterDict = [];

        for (int i = 0; i < units.Length; i++)
        {
            units[i].Pos = i;

            if (!countDict.ContainsKey(units[i].UnitType))
            {
                units[i].DupeIndex = 0;
                continue;
            }

            counterDict.TryAdd(units[i].UnitType, 0);
            units[i].DupeIndex = ++counterDict[units[i].UnitType];
        }
    }

    public Unit GetUnitAtPos(int pos)
    {
        return pos < PosLib.LowestOpp ? this.PlayerTeam.Units[pos] : this.OpponentTeam.Units[pos - PosLib.LowestOpp];
    }

    /// <returns>
    /// The <c>Team</c> that the <c>Unit</c> at <c>pos</c> belongs to
    /// </returns>
    public Team GetTeamAtPos(int pos)
    {
        return pos < PosLib.LowestOpp ? this.PlayerTeam : this.OpponentTeam;
    }

    public Team GetTeamBySide(Side side)
    {
        return side == Side.Ally ? this.PlayerTeam : this.OpponentTeam;
    }

    /// <returns>
    /// A new array of all <c>Units</c>
    /// </returns>
    public Unit[] GetAllUnits()
    {
        return [.. this.PlayerTeam.Units, .. this.OpponentTeam.Units];
    }
}