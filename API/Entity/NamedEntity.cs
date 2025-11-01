namespace API.Entity;

public abstract class NamedEntity(string keyName, string keyDescription) {
    public string KeyName { get; } = keyName;
    public virtual string KeyDescription { get; } = keyDescription;

    public virtual string GetName() => this.KeyName; // todo
}