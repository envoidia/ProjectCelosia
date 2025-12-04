using API.Extensions;
using API.Graphics;
using API.Modding;

namespace API.Entity;

public abstract class NamedEntity(string keyName) {
    public string KeyName => keyName;

    public virtual string GetName(string color, GameMod? mod = null) => color + this.KeyName.GetLang(mod);

    public virtual string GetName(GameMod? mod = null) => this.GetName(Colors.White, mod);
}