using API.Entity;
using API.Graphics;
using API.Modding;

namespace API.Battle;

public sealed class SkillType : NamedEntity, IModItem {
    public GameMod? Source { get; }

    public SkillType(GameMod? source, string keyName) : base(keyName) {
        this.Source = source;
        Core.SkillTypes.Add(this);
    }

    public override string GetName(GameMod? mod = null) => this.GetName(Colors.Stat);
}

public static class SkillTypes {
    public static readonly SkillType Str = new(null, "StatStr");
    public static readonly SkillType Mag = new(null, "StatMag");
    public static readonly SkillType Fth = new(null, "StatFth");
    public static readonly SkillType Stat = new(null, "SkillTypeStat");
}