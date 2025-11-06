using API.Extensions;

namespace API.Entity;

public abstract class IconEntity(string keyName, string keyDescription, string icon)
    : DescriptionEntity(keyName, keyDescription) {
    public string Icon => icon;

    public override string GetName() => this.Icon + " /c[white]" + this.KeyName.GetLang();
    public string GetName(string color) => this.Icon + " " + color + this.KeyName.GetLang();
}