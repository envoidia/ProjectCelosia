using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class Stat : INameable, IRegistrable
{
    public StageType? StageType { get; }

    public string KeyName { get; }

    public string ModId { get; }
    public string ItemId { get; init; }

    public Stat(string modId, string keyName, StageType? stageType, string? itemId = null)
    {
        this.StageType = stageType;

        this.KeyName = keyName;

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
    }

    public override string ToString()
    {
        return $"{base.ToString()}: {this.GetName()}";
    }

    public string GetName(ThemeColor color)
    {
        return color.Str + this.GetLang();
    }

    public string GetName()
    {
        return this.GetName(ThemeColor.Stat);
    }
}