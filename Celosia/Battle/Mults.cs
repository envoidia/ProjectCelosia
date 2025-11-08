using API.Battle;

namespace Celosia.Battle;

public static class Mults {
    public static readonly Mult IgnisDmgDealt = new(Main.ModInstance, "MultIgnisDmgDealt", true);
    public static readonly Mult IgnisDmgTaken = new(Main.ModInstance, "MultIgnisDmgTaken", false);
    public static readonly Mult GlaciesDmgDealt = new(Main.ModInstance, "MultGlaciesDmgDealt", true);
    public static readonly Mult GlaciesDmgTaken = new(Main.ModInstance, "MultGlaciesDmgTaken", false);
    public static readonly Mult FulgurDmgDealt = new(Main.ModInstance, "MultFulgurDmgDealt", true);
    public static readonly Mult FulgurDmgTaken = new(Main.ModInstance, "MultFulgurDmgTaken", false);
    public static readonly Mult VentusDmgDealt = new(Main.ModInstance, "MultVentusDmgDealt", true);
    public static readonly Mult VentusDmgTaken = new(Main.ModInstance, "MultVentusDmgTaken", false);
    public static readonly Mult TerraDmgDealt = new(Main.ModInstance, "MultTerraDmgDealt", true);
    public static readonly Mult TerraDmgTaken = new(Main.ModInstance, "MultTerraDmgTaken", false);
    public static readonly Mult LuxDmgDealt = new(Main.ModInstance, "MultLuxDmgDealt", true);
    public static readonly Mult LuxDmgTaken = new(Main.ModInstance, "MultLuxDmgTaken", false);
    public static readonly Mult MalumDmgDealt = new(Main.ModInstance, "MultMalumDmgDealt", true);
    public static readonly Mult MalumDmgTaken = new(Main.ModInstance, "MultMalumDmgTaken", false);
}