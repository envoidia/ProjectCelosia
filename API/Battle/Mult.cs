using System;
using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

public sealed class Mult : _IModItem, INameable {
    public bool IsPositive { get; }
    public int MinValue { get; init; } = 100;

    public GameMod? Source { get; }
    public string KeyName { get; }

    public Mult(GameMod? source, string keyName, bool isPositive) {
        this.Source = source;
        this.KeyName = keyName;

        this.IsPositive = isPositive;

        Core.Mults.Add(this);
    }

    public string Format(int val) => Math.Max(val, this.MinValue).FormatPerc(isPositive: this.IsPositive);

    public string FormatChange(float val) =>
        Math.Max(val, this.MinValue).FormatPerc(true, isPositive: this.IsPositive);

    public string GetName(ColorCode color, GameMod? mod = null) => color + this.KeyName.GetLang(mod);
    public string GetName(GameMod? mod = null) => this.GetName(ColorCode.Stat, mod);

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