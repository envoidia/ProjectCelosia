using System;
using API.Entity;
using API.Extensions;
using API.Graphics;
using API.Modding;

namespace API.Battle;

public class Mult : NamedEntity, IModItem {
    public bool IsPositive { get; }
    public uint MinValue { get; init; } = 100;

    public IGameMod? Source { get; }

    public Mult(IGameMod? source, string keyName, bool isPositive) : base(keyName) {
        this.Source = source;
        this.IsPositive = isPositive;
        Core.Mults.Add(this);
    }

    public override int GetHashCode() => this.KeyName.GetHashCode();

    public string Format(uint val) {
        (string c1, string c2) = this.GetColors();

        return Math.Max(val, this.MinValue)
            .Format(val > 1000 ? c1 : val < 1000 ? c2 : Colors.Num, "%", 10f);
    }

    public string FormatChange(float val) {
        (string c1, string c2) = this.GetColors();

        return Math.Max(val, this.MinValue).Format(val > 0 ? c1 : val < 0 ? c2 : Colors.Num, "%");
    }

    public (string, string) GetColors() => this.IsPositive ? (Colors.Pos, Colors.Neg) : (Colors.Neg, Colors.Pos);
}

public static class Mults {
    public static readonly Mult DmgDealt = new(null, "MultDmgDealt", true);
    public static readonly Mult DmgTaken = new(null, "MultDmgTaken", false);
    public static readonly Mult WeakDmgDealt = new(null, "MultWeakDmgDealt", true);
    public static readonly Mult WeakDmgTaken = new(null, "MultWeakDmgTaken", false);
    public static readonly Mult FollowUpDmgDealt = new(null, "MultFollowUpDmgDealt", true);
    public static readonly Mult FollowUpDmgTaken = new(null, "MultFollowUpDmgTaken", false);
    public static readonly Mult DoTDmgTaken = new(null, "MultDoTDmgTaken", false);
    public static readonly Mult HealingDealt = new(null, "MultHealingDealt", true);
    public static readonly Mult HealingTaken = new(null, "MultHealingTaken", true);
    public static readonly Mult SpGain = new(null, "MultSpGain", true);
    public static readonly Mult SpUse = new(null, "MultSpUse", false);

    public static readonly Mult PercentageDmgTaken = new(null, "MultPercentageDmgTaken", false) {
        MinValue = 1
    };
}