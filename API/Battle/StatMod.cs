using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;
using API.Util;

namespace API.Battle;

public sealed class StatMod : _IModItem, INameable {
    public bool IsPositive { get; }

    public GameMod? Source { get; }
    public string KeyName { get; }

    public StatMod(GameMod? source, string keyName, bool isPositive) {
        this.Source = source;
        this.KeyName = keyName;

        this.IsPositive = isPositive;

        Core.StatMods.Add(this);
    }

    public string Format(int val) => val switch {
        > 1000 => val.Format(TextLib.GetIncColor(this.IsPositive)),
        < 1000 => val.Format(TextLib.GetDecColor(this.IsPositive)),
        _ => val.Format(ColorCode.Num)
    };

    public string GetName(ColorCode color, GameMod? mod = null) => color + this.KeyName.GetLang(mod);
    public string GetName(GameMod? mod = null) => this.GetName(ColorCode.Stat, mod);
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