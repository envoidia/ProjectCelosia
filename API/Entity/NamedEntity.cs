namespace API.Entity;

public abstract class NamedEntity(string name, string description) {
    public string Name { get; set; } = name;
    public virtual string Description { get; set; } = description;
}