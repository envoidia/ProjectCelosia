using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class Stat : INameable, IRegistrable {
    public StageType? StageType { get; }

    public string KeyName { get; }

    public string ModId { get; }
    public string ItemId { get; init; }

    public Stat(string modId, string keyName, StageType? stageType, string? itemId = null) {
        this.StageType = stageType;

        this.KeyName = keyName;

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
    }

    public override string ToString() => $"{base.ToString()}: {this.GetName()}";

    public string GetName(ThemeColor color) => color.Str + this.GetLang();
    public string GetName() => this.GetName(ThemeColor.Stat);
}

public static class Stats {
    public static readonly Stat Hp = new(Core.Id, "StatHp", null);
    public static readonly Stat Str = new(Core.Id, "StatStr", StageTypes.Atk);
    public static readonly Stat Mag = new(Core.Id, "StatMag", StageTypes.Atk);
    public static readonly Stat Fth = new(Core.Id, "StatFth", StageTypes.Fth);
    public static readonly Stat Amr = new(Core.Id, "StatAmr", StageTypes.Def);
    public static readonly Stat Res = new(Core.Id, "StatRes", StageTypes.Def);
    public static readonly Stat Agi = new(Core.Id, "StatAgi", StageTypes.Agi);
}