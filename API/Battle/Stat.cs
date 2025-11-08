using API.Entity;
using API.Graphics;
using API.Modding;

namespace API.Battle;

public class Stat : NamedEntity, IModItem {
    public StageType StageType { get; }

    public IGameMod? Source { get; }

    public Stat(IGameMod? source, string keyName, StageType stageType) : base(keyName) {
        this.Source = source;
        this.StageType = stageType;
        Core.Stats.Add(this);
    }

    public override string GetName(IGameMod? mod = null) => this.GetName(Colors.Stat);
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