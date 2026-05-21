using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;
using API.Util;

namespace API.Battle;

/// <summary>
/// An int modifier that defaults to 0 and can be positive or negative
/// </summary>
public sealed class StatMod : INameable, IRegistrable
{
    public bool IsPositive { get; }

    public string KeyName { get; }

    public string ModId { get; }
    public string ItemId { get; init; }

    public StatMod(string modId, string keyName, bool isPositive, string? itemId = null)
    {
        this.IsPositive = isPositive;

        this.KeyName = keyName;

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
    }

    // todo use NE.Format
    public string Format(int val)
    {
        return val switch
        {
            > 0 => val.Format(TextLib.GetIncColor(this.IsPositive)),
            < 0 => val.Format(TextLib.GetDecColor(this.IsPositive)),
            _ => val.Format(ThemeColor.Emphasis)
        };
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
