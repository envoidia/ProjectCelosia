namespace API.Battle;

public class Mult {
    public string KeyName { get; }
    public bool IsPositive { get; }

    public Mult(string keyName, bool isPositive) {
        this.KeyName = keyName;
        this.IsPositive = isPositive;
        Core.Mults.Add(this);
    }

    public override int GetHashCode() => this.KeyName.GetHashCode();

    public string FormatVal(uint val) => "todo"; // todo Color + formatnum + %
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
    public static readonly Mult PercentageDmgTaken = new("MultPercentageDmgTaken", false);
    public static readonly Mult HealingDealt = new("MultHealingDealt", true);
    public static readonly Mult HealingTaken = new("MultHealingTaken", true);
    public static readonly Mult SpGain = new("MultSpGain", true);
    public static readonly Mult SpUse = new("MultSpUse", false);
}