using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class StageType : IDescribable, IRegistrable
{
    public Stat[] Stats { get; }

    public string KeyName { get; }

    public string Icon { get; }
    public string KeyDesc { get; }

    public string ModId { get; }
    public string ItemId { get; init; }

    /// <summary>
    /// todo docs
    /// </summary>
    /// <param name="modId"></param>
    /// <param name="keyName"></param>
    /// <param name="icon"></param>
    /// <param name="itemId">Item ID. If not provided, will use <c>keyName</c></param>
    /// <param name="stats"></param>
    public StageType(string modId, string keyName, string icon, Stat[] stats, string? itemId = null)
    {
        this.Stats = stats;

        this.KeyName = keyName;
        this.KeyDesc = $"{keyName}Desc";
        this.Icon = icon;

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
    }

    public override string ToString()
    {
        return $"{base.ToString()}: {this.GetName()} -- {this.GetDesc()}";
    }

    public string GetName(ThemeColor color)
    {
        return $"{this.Icon} {color.Str}{this.GetLang()}";
    }

    public string GetName()
    {
        return this.GetName(ThemeColor.Buff);
    }

    public string GetNameWithSign(int stage)
    {
        return $"{this.GetName()} {(stage > 0 ? "Up" : "Down")}";
    }

    public string GetDesc()
    {
        return this.KeyDesc.GetLang(this.ModId);
    }
}