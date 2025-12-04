using API.Battle;

namespace Celosia.Battle;

public static class Accessories {
    public static readonly Accessory FirebornRing = new(Main.Mod, "AccessoryFirebornRing",
        "Todo", "/c[orange]/i[fire-ring]") {
        Passives = [Passives.IgnisAffUp]
    };
}