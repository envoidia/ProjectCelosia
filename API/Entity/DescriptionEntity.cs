using API.Extensions;

namespace API.Entity;

public abstract class DescriptionEntity(string keyName, string keyDescription) : NamedEntity(keyName) {
    internal string KeyDescription { get; } = keyDescription;

    public virtual string GetDescription() => this.KeyDescription.GetLang();
}