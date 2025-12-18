using API.Battle.BuffEffects;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class Buff : ComplexDescribable {
    public BuffType BuffType { get; }
    public int MaxStacks { get; }
    public IBuffEffect[] BuffEffects { get; }

    public Buff(GameMod? source, string keyName, string icon, string keyDesc, BuffType buffType,
        int maxStacks, params IBuffEffect[] buffEffects) : base(source, keyName, icon, keyDesc) {
        this.BuffType = buffType;
        this.MaxStacks = maxStacks;
        this.BuffEffects = buffEffects;

        Core.Buffs.Add(this);
    }

    public override string GetName(GameMod? mod = null) => this.GetName(ThemeColor.Buff, mod ?? this.Source);

    // Todo use stack amount to show multiplied values
    public override string GetFullDesc(GameMod? mod = null) =>
        string.Format(Lang.BuffDesc, this.BuffType.GetName(),
            this.MaxStacks == 1 ? "" : string.Format(Lang.BuffDescStacksTo, ThemeColor.Imp.Str() + this.MaxStacks),
            this._GetFormattedDescInclusions(mod ?? this.Source));
}

public static class Buffs {
    // todo
    public static readonly Buff Defend;
    public static readonly Buff Shield;
}