namespace API.Battle;

public class Battle(Team playerTeam, Team opponentTeam) {
    public Team PlayerTeam => playerTeam;
    public Team OpponentTeam => opponentTeam;
    public uint Turn { get; set; } = 0;

    public Unit GetUnitAtPos(uint pos) => pos < 4 ? this.PlayerTeam.Units[pos] : this.OpponentTeam.Units[pos - 4];

    // Returns the team that the Unit at pos belongs to
    public Team GetTeamAtPos(uint pos) => pos < 4 ? this.PlayerTeam : this.OpponentTeam;

    public Unit[] GetAllUnits() => [..this.PlayerTeam.Units, ..this.OpponentTeam.Units];
}