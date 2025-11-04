using API.Battle;

namespace Game.Battle;

public static class Accessories {
    public static readonly Accessory FirebornRing = new("AccessoryFirebornRing", "todo", "/c[orange]/i[fire-ring]") {
        Passives = [Passives.IgnisAffUp]
    };
}