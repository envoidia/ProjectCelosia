using API.Battle;
using API.Modding;

namespace Celosia.Battle;

public static class Passives {
    public static Passive IgnisAffUp;

    public static void Initialize(IGameMod mod) {
        IgnisAffUp = new Passive(mod, "PassiveIgnisAffUp", "PassiveIgnisAffUpDesc", "todo") {
            BuffEffects = [] // todo
        };
    }
}