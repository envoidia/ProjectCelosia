using API.Extensions;
using API.Modding;

namespace API.Entity;

public abstract class DescriptionEntity(string keyName, string keyDescription) : NamedEntity(keyName) {
    public virtual string GetDescription(IGameMod? mod = null) =>
        mod is null ? keyDescription.GetLang() : keyDescription.GetLang(mod);
}