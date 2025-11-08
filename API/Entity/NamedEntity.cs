using API.Extensions;
using API.Graphics;
using API.Modding;

namespace API.Entity;

public abstract class NamedEntity(string keyName) {
    public string KeyName => keyName;

    public virtual string GetName(string color, IGameMod? mod = null) =>
        color + (mod is null ? this.KeyName.GetLang() : this.KeyName.GetLang(mod));

    public virtual string GetName(IGameMod? mod = null) => this.GetName(Colors.White, mod);
}