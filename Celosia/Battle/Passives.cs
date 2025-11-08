using API.Battle;
using API.Battle.BuffEffects;

namespace Celosia.Battle;

public static class Passives {
    public static readonly Passive IgnisAffUp = new(Main.ModInstance, "PassiveIgnisAffUp",
        "PassiveIgnisAffUpDesc", "todo") {
        BuffEffects = [new ChangeAffinity(Elements.Ignis, 1)]
    };
}