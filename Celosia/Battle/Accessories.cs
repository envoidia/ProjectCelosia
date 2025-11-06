using API.Battle;
using API.Modding;

namespace Celosia.Battle;

public static class Accessories {
    public static Accessory FirebornRing;

    public static void Initialize(IGameMod mod) {
        FirebornRing = new Accessory(mod, "AccessoryFirebornRing", "todo",
            "/c[orange]/i[fire-ring]") {
            Passives = [Passives.IgnisAffUp]
        };
    }
}