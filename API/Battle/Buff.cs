using API.Battle.BuffEffects;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

/// <summary>
/// todo docs
/// </summary>
public sealed class Buff : ComplexDescribable, IRegistrable {
    public BuffType BuffType { get; }
    public int MaxStacks { get; }
    public IBuffEffect[] BuffEffects { get; }

    public string ItemId { get; init; }

    public Buff(string modId, string keyName, string icon, string keyDesc, BuffType buffType,
        int maxStacks, params IBuffEffect[] buffEffects) : base(keyName, icon, keyDesc) {
        this.BuffType = buffType;
        this.MaxStacks = maxStacks;
        this.BuffEffects = buffEffects;

        this.ModId = modId;
        this.ItemId = keyName;

        Registry.Register(this);
    }

    public override string GetName() => this.GetName(ThemeColor.Buff);

    // Todo use stack amount to show multiplied values
    public override string GetFullDesc() =>
        string.Format(Lang.BuffDesc, this.BuffType.GetName(),
            this.MaxStacks == 1 ? "" : string.Format(Lang.BuffDescStacksTo, ThemeColor.Imp.Str() + this.MaxStacks),
            this._GetFormattedDescInclusions());
}

public static class Buffs {
    // todo
    public static readonly Buff Defend;
    public static readonly Buff Shield;
}