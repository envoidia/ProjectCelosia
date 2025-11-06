using API.Battle.BuffEffects;
using API.Entity;
using API.Graphics;
using API.Modding;

namespace API.Battle;

public class Buff : ComplexDescriptionEntity, IModItem {
    public BuffType BuffType { get; }
    public uint MaxStacks { get; }
    public IBuffEffect[] BuffEffects { get; }

    public IGameMod? Source { get; }

    public Buff(IGameMod? source, string keyName, string keyDescription, string icon, BuffType buffType, uint maxStacks,
        params IBuffEffect[] buffEffects) : base(keyName, keyDescription, icon) {
        this.Source = source;
        this.BuffType = buffType;
        this.MaxStacks = maxStacks;
        this.BuffEffects = buffEffects;
        Core.Buffs.Add(this);
    }

    public override string GetDescription() => string.Format(Lang.BuffDesc, this.BuffType.GetName(),
        this.MaxStacks == 1 ? "" : string.Format(Lang.BuffDescStacksTo, Colors.Num + this.MaxStacks),
        this.GetPartialDescription());
}