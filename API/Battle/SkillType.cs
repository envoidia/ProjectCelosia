using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class SkillType : INameable, IRegistrable {
    public string KeyName { get; }

    public string ModId { get; }
    public string ItemId { get; init; }

    public SkillType(string modId, string keyName) {
        this.KeyName = keyName;

        this.ModId = modId;
        this.ItemId = keyName;

        Registry.Register(this);
    }

    public string GetName(ThemeColor color) => color.Str() + this.GetLang();
    public string GetName() => this.GetName(ThemeColor.Stat);
}

public static class SkillTypes {
    public static readonly SkillType Str = new(Core.Id, "StatStr");
    public static readonly SkillType Mag = new(Core.Id, "StatMag");
    public static readonly SkillType Fth = new(Core.Id, "StatFth");
    public static readonly SkillType Stat = new(Core.Id, "SkillTypeStat");
}