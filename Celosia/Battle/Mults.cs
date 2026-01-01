using API;
using API.Battle;

namespace Celosia.Battle;

public static class Mults
{
    public static readonly Mult IgnisDmgDealt = new(Core.BaseModId, "MultIgnisDmgDealt", true);
    public static readonly Mult IgnisDmgTaken = new(Core.BaseModId, "MultIgnisDmgTaken", false);
    public static readonly Mult GlaciesDmgDealt = new(Core.BaseModId, "MultGlaciesDmgDealt", true);
    public static readonly Mult GlaciesDmgTaken = new(Core.BaseModId, "MultGlaciesDmgTaken", false);
    public static readonly Mult FulgurDmgDealt = new(Core.BaseModId, "MultFulgurDmgDealt", true);
    public static readonly Mult FulgurDmgTaken = new(Core.BaseModId, "MultFulgurDmgTaken", false);
    public static readonly Mult VentusDmgDealt = new(Core.BaseModId, "MultVentusDmgDealt", true);
    public static readonly Mult VentusDmgTaken = new(Core.BaseModId, "MultVentusDmgTaken", false);
    public static readonly Mult TerraDmgDealt = new(Core.BaseModId, "MultTerraDmgDealt", true);
    public static readonly Mult TerraDmgTaken = new(Core.BaseModId, "MultTerraDmgTaken", false);
    public static readonly Mult LuxDmgDealt = new(Core.BaseModId, "MultLuxDmgDealt", true);
    public static readonly Mult LuxDmgTaken = new(Core.BaseModId, "MultLuxDmgTaken", false);
    public static readonly Mult MalumDmgDealt = new(Core.BaseModId, "MultMalumDmgDealt", true);
    public static readonly Mult MalumDmgTaken = new(Core.BaseModId, "MultMalumDmgTaken", false);
}