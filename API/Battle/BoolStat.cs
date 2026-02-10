using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class BoolStat : INameable, IRegistrable
{
    public string LogMsgKey { get; }
    public bool IsPositive { get; }
    public bool PossessiveNameInLogMsg { get; }
    public bool IsVisible { get; }

    public string KeyName { get; }

    public string ModId { get; }
    public string ItemId { get; init; }

    public BoolStat(string modId, string keyName, string logMsgKey, bool isPositive, bool possessiveNameInLogMsg,
        bool isVisible, string? itemId = null)
    {
        this.LogMsgKey = logMsgKey;
        this.IsPositive = isPositive;
        this.PossessiveNameInLogMsg = possessiveNameInLogMsg;
        this.IsVisible = isVisible;

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

    // todo format?
}