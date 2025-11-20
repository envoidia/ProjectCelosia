using System.Collections.Generic;
using System.Linq;

namespace API.Battle;

public sealed class Battle {
    public Team PlayerTeam;
    public Team OpponentTeam;
    public int Turn { get; set; } = 0;

    public Battle(Team playerTeam, Team opponentTeam) {
        this.PlayerTeam = playerTeam;
        this.OpponentTeam = opponentTeam;

        // Assign disambiguation identifiers to duplicate UnitTypes
        Unit[] units = this.GetAllUnits();

        Dictionary<UnitType, int> countDict = units.GroupBy(u => u.UnitType).ToDictionary(g => g.Key, g => g.Count());
        Dictionary<UnitType, int> counterDict = [];

        foreach (Unit unit in units) {
            if (!countDict.ContainsKey(unit.UnitType)) {
                unit.DupeIndex = 0;
                continue;
            }

            counterDict.TryAdd(unit.UnitType, 0);
            unit.DupeIndex = ++counterDict[unit.UnitType];
        }
    }

    public Unit GetUnitAtPos(int pos) => pos < 4 ? this.PlayerTeam.Units[pos] : this.OpponentTeam.Units[pos - 4];

    /// <summary>
    /// Returns the Team that the Unit at pos belongs to
    /// </summary>
    public Team GetTeamAtPos(int pos) => pos < 4 ? this.PlayerTeam : this.OpponentTeam;

    public Team GetTeamBySide(Side side) => side == Side.Ally ? this.PlayerTeam : this.OpponentTeam;

    public Unit[] GetAllUnits() => [.. this.PlayerTeam.Units, .. this.OpponentTeam.Units];
}