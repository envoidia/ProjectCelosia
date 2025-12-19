using API.Battle;
using API.Battle.BuffEffects;

namespace Celosia.Battle;

public static class Passives {
    public static readonly Passive IgnisAffUp = new(Main.Id, "PassiveIgnisAffUp",
        "PassiveIgnisAffUpDesc", "todo", [new ChangeAffinity(Elements.Ignis, 1)]);
}