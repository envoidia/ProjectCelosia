using API.Extensions;

namespace API.Entity;

public abstract class NamedEntity(string keyName) {
    public string KeyName { get; } = keyName;

    public virtual string GetName() => this.KeyName.GetLang();
}