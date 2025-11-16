using API.Graphics;
using API.Modding;

namespace API.Entity;

public abstract class IconEntity(string keyName, string keyDescription, string icon)
    : DescriptionEntity(keyName, keyDescription) {
    public string Icon => icon;

    public override string GetName(string color, IGameMod? mod = null) =>
        $"{this.Icon} {base.GetName(color, mod)}";

    public override string GetName(IGameMod? mod = null) => this.GetName(Colors.White);
}