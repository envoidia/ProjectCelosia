using API.Entity;
using API.Extensions;
using API.Graphics;

namespace API.Battle;

public class StatMod : NamedEntity {
    private readonly bool _isPositive;

    public StatMod(string keyName, bool isPositive) : base(keyName) {
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
    public static readonly StatMod DurationBuffDealt = new("ModDurationBuffDealt", true);
    public static readonly StatMod DurationBuffTaken = new("ModDurationBuffTaken", true);
    public static readonly StatMod DurationDebuffDealt = new("ModDurationDebuffDealt", true);
    public static readonly StatMod DurationDebuffTaken = new("ModDurationDebuffTaken", false);
    public static readonly StatMod StacksBuffDealt = new("ModStacksBuffDealt", true);
    public static readonly StatMod StacksBuffTaken = new("ModStacksBuffTaken", true);
    public static readonly StatMod StacksDebuffDealt = new("ModStacksDebuffDealt", true);
    public static readonly StatMod StacksDebuffTaken = new("ModStacksDebuffTaken", false);
    public static readonly StatMod Range = new("ModRange", true);
}

public class Foo : NamedEntity {
    public Foo(int lorem) : base("") { }
}