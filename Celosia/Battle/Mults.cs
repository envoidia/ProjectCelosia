using API.Battle;

namespace Celosia.Battle;

public static class Mults {
    public static readonly Mult IgnisDmgDealt = new(Main.Id, "MultIgnisDmgDealt", true);
    public static readonly Mult IgnisDmgTaken = new(Main.Id, "MultIgnisDmgTaken", false);
    public static readonly Mult GlaciesDmgDealt = new(Main.Id, "MultGlaciesDmgDealt", true);
    public static readonly Mult GlaciesDmgTaken = new(Main.Id, "MultGlaciesDmgTaken", false);
    public static readonly Mult FulgurDmgDealt = new(Main.Id, "MultFulgurDmgDealt", true);
    public static readonly Mult FulgurDmgTaken = new(Main.Id, "MultFulgurDmgTaken", false);
    public static readonly Mult VentusDmgDealt = new(Main.Id, "MultVentusDmgDealt", true);
    public static readonly Mult VentusDmgTaken = new(Main.Id, "MultVentusDmgTaken", false);
    public static readonly Mult TerraDmgDealt = new(Main.Id, "MultTerraDmgDealt", true);
    public static readonly Mult TerraDmgTaken = new(Main.Id, "MultTerraDmgTaken", false);
    public static readonly Mult LuxDmgDealt = new(Main.Id, "MultLuxDmgDealt", true);
    public static readonly Mult LuxDmgTaken = new(Main.Id, "MultLuxDmgTaken", false);
    public static readonly Mult MalumDmgDealt = new(Main.Id, "MultMalumDmgDealt", true);
    public static readonly Mult MalumDmgTaken = new(Main.Id, "MultMalumDmgTaken", false);
}