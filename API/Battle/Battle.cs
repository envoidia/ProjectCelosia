namespace API.Battle;

public class Battle(Team playerTeam, Team opponentTeam) {
    public Team PlayerTeam { get; } = playerTeam;
    public Team OpponentTeam { get; } = opponentTeam;
    public uint Turn { get; set; } = 0;

    // Returns the team that the Unit at pos belongs to
    public Team GetTeamAtPos(uint pos) => pos < 4 ? this.PlayerTeam : this.OpponentTeam;
}