namespace API.Battle;

public sealed class Battle(Team playerTeam, Team opponentTeam) {
    public Team PlayerTeam => playerTeam;
    public Team OpponentTeam => opponentTeam;
    public int Turn { get; set; } = 0;

    public Unit GetUnitAtPos(int pos) => pos < 4 ? this.PlayerTeam.Units[pos] : this.OpponentTeam.Units[pos - 4];

    // Returns the team that the Unit at pos belongs to
    public Team GetTeamAtPos(int pos) => pos < 4 ? this.PlayerTeam : this.OpponentTeam;

    public Unit[] GetAllUnits() => [..this.PlayerTeam.Units, ..this.OpponentTeam.Units];
}