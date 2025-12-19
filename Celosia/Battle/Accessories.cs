using API.Battle;
using API.Graphics;

namespace Celosia.Battle;

public static class Accessories {
    public static readonly Accessory FirebornRing = new(Main.Id, "AccessoryFirebornRing",
        $"{ThemeColor.Ignis.Str()}/i[fire-ring]") {
        Passives = [Passives.IgnisAffUp]
    };
}