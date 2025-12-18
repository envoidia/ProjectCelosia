using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class Stat : INameable {
    public StageType? StageType { get; }

    public GameMod? Source { get; }
    public string KeyName { get; }

    public Stat(GameMod? source, string keyName, StageType? stageType) {
        this.Source = source;
        this.KeyName = keyName;

        this.StageType = stageType;

        Core.Stats.Add(this);
    }

    public string GetName(ThemeColor color, GameMod? mod = null) => color.Str() + this.KeyName.GetLang(mod ?? this.Source);
    public string GetName(GameMod? mod = null) => this.GetName(ThemeColor.Stat, mod ?? this.Source);
}

public static class Stats {
    public static readonly Stat Hp = new(null, "Hp", null);
    public static readonly Stat Str = new(null, "StatStr", StageTypes.Atk);
    public static readonly Stat Mag = new(null, "StatMag", StageTypes.Atk);
    public static readonly Stat Fth = new(null, "StatFth", StageTypes.Fth);
    public static readonly Stat Amr = new(null, "StatAmr", StageTypes.Def);
    public static readonly Stat Res = new(null, "StatRes", StageTypes.Def);
    public static readonly Stat Agi = new(null, "StatAgi", StageTypes.Agi);
}