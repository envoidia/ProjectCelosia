using System;
using API.Entity;
using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Util;

namespace API.Battle;

public sealed class Mult : NamedEntity, IModItem {
    public bool IsPositive { get; }
    public int MinValue { get; init; } = 100;

    public GameMod? Source { get; }

    public Mult(GameMod? source, string keyName, bool isPositive) : base(keyName) {
        this.Source = source;
        this.IsPositive = isPositive;
        Core.Mults.Add(this);
    }

    public string Format(int val) => Math.Max(val, this.MinValue).Format(val switch {
        > 1000 => TextLib.GetIncColor(this.IsPositive),
        < 1000 => TextLib.GetDecColor(this.IsPositive),
        _ => Colors.Num
    }, true, '%', 10f);

    public string FormatChange(float val) => Math.Max(val, this.MinValue).Format(val switch {
        > 0 => TextLib.GetIncColor(this.IsPositive),
        < 0 => TextLib.GetDecColor(this.IsPositive),
        _ => Colors.Num
    }, true, '%');
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
    public static readonly Mult PercentageDmgTaken = new(null, "MultPercentageDmgTaken", false) { MinValue = 1 };
}