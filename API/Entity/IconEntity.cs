namespace API.Entity;

public abstract class IconEntity(string name, string description, string icon) : NamedEntity(name, description) {
    public string Icon { get; set; } = icon;

    public string GetNameWithIcon() => icon + " [WHITE]" + this.Name;

    public string GetNameWithIcon(string color) => icon + " " + color + this.Name;
}