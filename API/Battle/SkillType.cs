using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class SkillType : _IModItem, INameable {
    public GameMod? Source { get; }
    public string KeyName { get; }

    public SkillType(GameMod? source, string keyName) {
        this.Source = source;
        this.KeyName = keyName;

        Core.SkillTypes.Add(this);
    }

    public string GetName(string color = Colors.Stat, GameMod? mod = null) => color + this.KeyName.GetLang(mod);
}

public static class SkillTypes {
    public static readonly SkillType Str = new(null, "StatStr");
    public static readonly SkillType Mag = new(null, "StatMag");
    public static readonly SkillType Fth = new(null, "StatFth");
    public static readonly SkillType Stat = new(null, "SkillTypeStat");
}