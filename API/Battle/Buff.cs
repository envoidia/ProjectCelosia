using API.Battle.BuffEffects;
using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

/// <summary>
/// todo docs
/// </summary>
public sealed class Buff : ComplexDescribable, IRegistrable
{
    public BuffType BuffType { get; }
    public int MaxStacks { get; }
    public IBuffEffect[] BuffEffects { get; }

    public string ItemId { get; init; }

    public Buff(string modId, string keyName, string icon, string keyDesc, BuffType buffType,
    int maxStacks, IBuffEffect[] buffEffects, string? itemId = null)
        : base(keyName, icon, keyDesc)
    {
        this.BuffType = buffType;
        this.MaxStacks = maxStacks;
        this.BuffEffects = buffEffects;

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
    }

    public override string GetName()
    {
        return this.GetName(ThemeColor.Buff);
    }

    public string GetNameWithoutIcon()
    {
        return $"{ThemeColor.Buff.Str}{this.KeyName.GetLang(this.ModId)}";
    }

    // Todo use stack amount to show multiplied values
    // todo show stacks + (de)buff in the truncated desc too
    public override string GetFullDesc()
    {
        return "BuffDesc".FormatLang([this.BuffType.GetName(),
            this.MaxStacks == 1 ? "" : "BuffDescStacksTo".FormatLang(ThemeColor.Imp.Str + this.MaxStacks),
            this._GetFormattedDescInclusions()]);
    }
}