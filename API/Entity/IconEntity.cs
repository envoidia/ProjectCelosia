namespace API.Entity;

public abstract class IconEntity(string keyName, string keyDescription, string icon) : NamedEntity(keyName, keyDescription) {
    public string Icon { get; set; } = icon;

    public override string GetName() => this.Icon + " /c[white]" + this.KeyName; // todo

    public string GetName(string color) => this.Icon + " " + color + this.KeyName; // todo
}