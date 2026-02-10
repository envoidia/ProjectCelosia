using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

/// <summary>
/// todo docs
/// </summary>
public sealed class SkillType : INameable, IRegistrable
{
    public string KeyName { get; }

    public string ModId { get; }
    public string ItemId { get; init; }

    /// <summary>
    /// todo docs
    /// </summary>
    /// <param name="modId"></param>
    /// <param name="keyName"></param>
    /// <param name="itemId">Item ID. If not provided, will use <c>keyName</c></param>
    public SkillType(string modId, string keyName, string? itemId = null)
    {
        this.KeyName = keyName;

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
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