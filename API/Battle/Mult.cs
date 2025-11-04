using System;
using API.Entity;
using API.Extensions;
using API.Graphics;

namespace API.Battle;

public class Mult : NamedEntity {
    public bool IsPositive { get; }
    public uint MinValue { get; init; } = 100;

    public Mult(string keyName, bool isPositive) : base(keyName) {
        this.IsPositive = isPositive;
        Core.Mults.Add(this);
    }

    public override int GetHashCode() => this.KeyName.GetHashCode();

    public string Format(uint val) {
        (string c1, string c2) = this.GetColors();

        return Math.Max(val, this.MinValue)
            .Format(val > 1000 ? c1 : val < 1000 ? c2 : Colors.Num, "%", 10d);
    }

    public string FormatChange(double val) {
        (string c1, string c2) = this.GetColors();

        return Math.Max(val, this.MinValue).Format(val > 0 ? c1 : val < 0 ? c2 : Colors.Num, "%");
    }

    public (string, string) GetColors() => this.IsPositive ? (Colors.Pos, Colors.Neg) : (Colors.Neg, Colors.Pos);
}

public static class Mults {
    public static readonly Mult DmgDealt = new("MultDmgDealt", true);
    public static readonly Mult DmgTaken = new("MultDmgTaken", false);
    public static readonly Mult IgnisDmgDealt = new("MultIgnisDmgDealt", true);
    public static readonly Mult IgnisDmgTaken = new("MultIgnisDmgTaken", false);
    public static readonly Mult GlaciesDmgDealt = new("MultGlaciesDmgDealt", true);
    public static readonly Mult GlaciesDmgTaken = new("MultGlaciesDmgTaken", false);
    public static readonly Mult FulgurDmgDealt = new("MultFulgurDmgDealt", true);
    public static readonly Mult FulgurDmgTaken = new("MultFulgurDmgTaken", false);
    public static readonly Mult VentusDmgDealt = new("MultVentusDmgDealt", true);
    public static readonly Mult VentusDmgTaken = new("MultVentusDmgTaken", false);
    public static readonly Mult TerraDmgDealt = new("MultTerraDmgDealt", true);
    public static readonly Mult TerraDmgTaken = new("MultTerraDmgTaken", false);
    public static readonly Mult LuxDmgDealt = new("MultLuxDmgDealt", true);
    public static readonly Mult LuxDmgTaken = new("MultLuxDmgTaken", false);
    public static readonly Mult MalumDmgDealt = new("MultMalumDmgDealt", true);
    public static readonly Mult MalumDmgTaken = new("MultMalumDmgTaken", false);
    public static readonly Mult WeakDmgDealt = new("MultWeakDmgDealt", true);
    public static readonly Mult WeakDmgTaken = new("MultWeakDmgTaken", false);
    public static readonly Mult FollowUpDmgDealt = new("MultFollowUpDmgDealt", true);
    public static readonly Mult FollowUpDmgTaken = new("MultFollowUpDmgTaken", false);
    public static readonly Mult DoTDmgTaken = new("MultDoTDmgTaken", false);
    public static readonly Mult HealingDealt = new("MultHealingDealt", true);
    public static readonly Mult HealingTaken = new("MultHealingTaken", true);
    public static readonly Mult SpGain = new("MultSpGain", true);
    public static readonly Mult SpUse = new("MultSpUse", false);

    public static readonly Mult PercentageDmgTaken = new("MultPercentageDmgTaken", false) {
        MinValue = 1
    };
}