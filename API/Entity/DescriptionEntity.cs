using API.Extensions;

namespace API.Entity;

public abstract class DescriptionEntity(string keyName, string keyDescription) : NamedEntity(keyName) {
    public virtual string GetDescription() => keyDescription.GetLang();
}