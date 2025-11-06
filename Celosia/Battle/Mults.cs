using API.Battle;
using API.Modding;

namespace Celosia.Battle;

public static class Mults {
    public static Mult IgnisDmgDealt;
    public static Mult IgnisDmgTaken;
    public static Mult GlaciesDmgDealt;
    public static Mult GlaciesDmgTaken;
    public static Mult FulgurDmgDealt;
    public static Mult FulgurDmgTaken;
    public static Mult VentusDmgDealt;
    public static Mult VentusDmgTaken;
    public static Mult TerraDmgDealt;
    public static Mult TerraDmgTaken;
    public static Mult LuxDmgDealt;
    public static Mult LuxDmgTaken;
    public static Mult MalumDmgDealt;
    public static Mult MalumDmgTaken;

    public static void Initialize(IGameMod mod) {
        IgnisDmgDealt = new Mult(mod, "MultIgnisDmgDealt", true);
        IgnisDmgTaken = new Mult(mod, "MultIgnisDmgTaken", false);
        GlaciesDmgDealt = new Mult(mod, "MultGlaciesDmgDealt", true);
        GlaciesDmgTaken = new Mult(mod, "MultGlaciesDmgTaken", false);
        FulgurDmgDealt = new Mult(mod, "MultFulgurDmgDealt", true);
        FulgurDmgTaken = new Mult(mod, "MultFulgurDmgTaken", false);
        VentusDmgDealt = new Mult(mod, "MultVentusDmgDealt", true);
        VentusDmgTaken = new Mult(mod, "MultVentusDmgTaken", false);
        TerraDmgDealt = new Mult(mod, "MultTerraDmgDealt", true);
        TerraDmgTaken = new Mult(mod, "MultTerraDmgTaken", false);
        LuxDmgDealt = new Mult(mod, "MultLuxDmgDealt", true);
        LuxDmgTaken = new Mult(mod, "MultLuxDmgTaken", false);
        MalumDmgDealt = new Mult(mod, "MultMalumDmgDealt", true);
        MalumDmgTaken = new Mult(mod, "MultMalumDmgTaken", false);
    }
}