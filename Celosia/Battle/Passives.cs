using API;
using API.Battle;
using API.Battle.BuffEffects;

namespace Celosia.Battle;

public static class Passives
{
    public static readonly Passive IgnisAffUp = new(Core.BaseModId, "PassiveIgnisAffUp",
        "PassiveIgnisAffUpDesc", "todo", [new ChangeAffinity(Elements.Ignis, 1)]);
}