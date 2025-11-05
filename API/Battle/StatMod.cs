using API.Entity;
using API.Extensions;
using API.Graphics;
using API.Modding;

namespace API.Battle;

public class StatMod : NamedEntity, IModItem {
    private readonly bool _isPositive;

    public GameMod? Source { get; }

    public StatMod(GameMod? source, string keyName, bool isPositive) : base(keyName) {
        this.Source = source;
        this._isPositive = isPositive;
        Core.StatMods.Add(this);
    }

    public override int GetHashCode() => this.KeyName.GetHashCode();

    public string Format(int val) {
        string c1; // Increased
        string c2; // Decreased

        if (this._isPositive) {
            c1 = Colors.Pos;
            c2 = Colors.Neg;
        } else {
            c1 = Colors.Neg;
            c2 = Colors.Pos;
        }

        return val.Format(val > 1000 ? c1 : val < 1000 ? c2 : Colors.Num);
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