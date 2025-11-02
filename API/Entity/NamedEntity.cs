using API.Extensions;

namespace API.Entity;

public abstract class NamedEntity(string keyName) {
    internal string KeyName { get; } = keyName;

    public virtual string GetName() => this.KeyName.GetLang();
}