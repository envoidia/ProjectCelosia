using API.Battle.BuffEffects;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class Buff : ComplexDescribable, _IModItem {
    public BuffType BuffType { get; }
    public int MaxStacks { get; }
    public IBuffEffect[] BuffEffects { get; }

    public GameMod? Source { get; }

    public Buff(GameMod? source, string keyName, string icon, string keyDesc, BuffType buffType,
        int maxStacks, params IBuffEffect[] buffEffects) : base(keyName, icon, keyDesc) {
        this.BuffType = buffType;
        this.MaxStacks = maxStacks;
        this.BuffEffects = buffEffects;

        this.Source = source;

        Core.Buffs.Add(this);
    }

    public override string GetName(GameMod? mod = null) => this.GetName(ColorCode.Buff, mod);

    public override string GetFullDesc(GameMod? mod = null) =>
        string.Format(Lang.BuffDesc, this.BuffType.GetName(),
            this.MaxStacks == 1 ? "" : string.Format(Lang.BuffDescStacksTo, ColorCode.Num + this.MaxStacks),
            this._GetFormattedDescInclusions(mod));
}

public static class Buffs {
    // todo
    public static readonly Buff Defend;
    public static readonly Buff Shield;
}