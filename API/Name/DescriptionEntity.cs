using API.Extensions;
using API.Modding;
using API.Name;

namespace API.Entity;

public abstract class DescriptionEntity(string keyName, string keyDescription) : NamedEntity(keyName) {
    public virtual string GetDescription(GameMod? mod = null) => keyDescription.GetLang(mod);
}