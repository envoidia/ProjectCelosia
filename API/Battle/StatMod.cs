using API.Entity;
using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Util;

namespace API.Battle;

public class StatMod : NamedEntity, IModItem {
    public bool IsPositive { get; }

    public IGameMod? Source { get; }

    public StatMod(IGameMod? source, string keyName, bool isPositive) : base(keyName) {
        this.Source = source;
        this.IsPositive = isPositive;
        Core.StatMods.Add(this);
    }

    public string Format(int val) {
        (string pos, string neg) = TextLib.GetColors(this.IsPositive);

        return val.Format(val > 1000 ? pos : val < 1000 ? neg : Colors.Num);
    }
}

public static class StatMods {
    public static readonly StatMod DurationBuffDealt = new(null, "ModDurationBuffDealt", true);
    public static readonly StatMod DurationBuffTaken = new(null, "ModDurationBuffTaken", true);
    public static readonly StatMod DurationDebuffDealt = new(null, "ModDurationDebuffDealt", true);
    public static readonly StatMod DurationDebuffTaken = new(null, "ModDurationDebuffTaken", false);
    public static readonly StatMod StacksBuffDealt = new(null, "ModStacksBuffDealt", true);
    public static readonly StatMod StacksBuffTaken = new(null, "ModStacksBuffTaken", true);
    public static readonly StatMod StacksDebuffDealt = new(null, "ModStacksDebuffDealt", true);
    public static readonly StatMod StacksDebuffTaken = new(null, "ModStacksDebuffTaken", false);
    public static readonly StatMod Range = new(null, "ModRange", true);
}