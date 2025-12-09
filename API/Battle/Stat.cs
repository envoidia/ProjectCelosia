using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class Stat : _IModItem, INameable {
    public StageType StageType { get; }

    public GameMod? Source { get; }
    public string KeyName { get; }


    public Stat(GameMod? source, string keyName, StageType stageType) {
        this.Source = source;
        this.KeyName = keyName;

        this.StageType = stageType;

        Core.Stats.Add(this);
    }

    public string GetName(ColorCode color, GameMod? mod = null) => color + this.KeyName.GetLang(mod);
    public string GetName(GameMod? mod = null) => this.GetName(ColorCode.Stat, mod);
}

public static class Stats {
    public static readonly Stat Hp = new(null, "Hp", StageTypes.None);
    public static readonly Stat Str = new(null, "StatStr", StageTypes.Atk);
    public static readonly Stat Mag = new(null, "StatMag", StageTypes.Atk);
    public static readonly Stat Fth = new(null, "StatFth", StageTypes.Fth);
    public static readonly Stat Amr = new(null, "StatAmr", StageTypes.Def);
    public static readonly Stat Res = new(null, "StatRes", StageTypes.Def);
    public static readonly Stat Agi = new(null, "StatAgi", StageTypes.Agi);
}