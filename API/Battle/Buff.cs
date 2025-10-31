using API.Entity;
using API.Graphics;

namespace API.Battle;

public class Buff : ComplexDescriptionEntity {
    public BuffType BuffType { get; }
    public uint MaxStacks { get; }
    public IBuffEffect[] BuffEffects { get; }

    public override string Description => string.Format(Lang.BuffDesc, this.BuffType.GetName(),
        this.MaxStacks == 1 ? "" : string.Format(Lang.BuffDescStacksTo, Colors.Num + this.MaxStacks),
        this.GetPartialDesc());

    public Buff(string name, string description, string icon, BuffType buffType, uint maxStacks,
        params IBuffEffect[] buffEffects) : base(name, description, icon) {
        this.BuffType = buffType;
        this.MaxStacks = maxStacks;
        this.BuffEffects = buffEffects;
        Core.Buffs.Add(this);
    }
}