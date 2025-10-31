namespace API.Battle;

using static BattleLib;

public class Stats(uint hp, uint str, uint mag, uint fth, uint amr, uint res, uint agi) {
    public uint Hp { get; set; } = hp;
    public uint Str { get; set; } = str;
    public uint Mag { get; set; } = mag;
    public uint Fth { get; set; } = fth;
    public uint Amr { get; set; } = amr;
    public uint Res { get; set; } = res;
    public uint Agi { get; set; } = agi;

    public Stats(Stats stats) : this(stats.Hp, stats.Str, stats.Mag, stats.Fth, stats.Amr, stats.Res, stats.Agi) { }

    public Stats(uint stats) : this(stats, stats, stats, stats, stats, stats, stats) { }

    public uint GetStat(Stat stat) => stat switch {
        Stat.Str => this.Str,
        Stat.Mag => this.Mag,
        Stat.Fth => this.Fth,
        Stat.Amr => this.Amr,
        Stat.Res => this.Res,
        Stat.Agi => this.Agi
    };

    public void SetStat(Stat stat, uint set) {
        switch (stat) {
            case Stat.Str: this.Str = set; break;
            case Stat.Mag: this.Mag = set; break;
            case Stat.Fth: this.Fth = set; break;
            case Stat.Amr: this.Amr = set; break;
            case Stat.Res: this.Res = set; break;
            case Stat.Agi: this.Agi = set; break;
        }
    }

    public void AddToStat(Stat stat, uint change) {
        switch (stat) {
            case Stat.Str: this.Str += change; break;
            case Stat.Mag: this.Mag += change; break;
            case Stat.Fth: this.Fth += change; break;
            case Stat.Amr: this.Amr += change; break;
            case Stat.Res: this.Res += change; break;
            case Stat.Agi: this.Agi += change; break;
        }
    }

    public Stats GetScaledStats(uint lvl) => new((this.Hp + ((this.Hp / 2) * lvl)) * StatMult,
        (this.Str + ((this.Str / 2) * lvl)) * StatMult,
        (this.Mag + ((this.Mag / 2) * lvl)) * StatMult,
        (this.Fth + ((this.Fth / 2) * lvl)) * StatMult,
        (this.Amr + ((this.Amr / 2) * lvl)) * StatMult,
        (this.Res + ((this.Res / 2) * lvl)) * StatMult,
        (this.Agi + ((this.Agi / 2) * lvl)) * StatMult);
}