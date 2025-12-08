using API.Battle.BuffEffects;
using API.Entity;
using API.Graphics;
using API.Modding;

namespace API.Battle;

public sealed class Buff : ComplexDescriptionEntity, _IModItem {
    public BuffType BuffType { get; }
    public int MaxStacks { get; }
    public IBuffEffect[] BuffEffects { get; }

    public GameMod? Source { get; }

    public Buff(GameMod? source, string keyName, string keyDescription, string icon, BuffType buffType, int maxStacks,
        params IBuffEffect[] buffEffects) : base(keyName, keyDescription, icon) {
        this.Source = source;
        this.BuffType = buffType;
        this.MaxStacks = maxStacks;
        this.BuffEffects = buffEffects;
        Core.Buffs.Add(this);
    }

    public override string GetName(GameMod? mod = null) => this.GetName(Colors.Buff);

    public override string GetDescriptionWithInclusions(GameMod? mod = null) =>
        string.Format(Lang.BuffDesc, this.BuffType.GetName(),
            this.MaxStacks == 1 ? "" : string.Format(Lang.BuffDescStacksTo, Colors.Num + this.MaxStacks),
            this._GetFormattedDescriptionInclusions(mod));
}

public static class Buffs {
    // todo
    public static readonly Buff Defend;
    public static readonly Buff Shield;
}