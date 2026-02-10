using System;
using API.Extensions;
using API.Graphics;
using API.Modding;
using API.Name;

namespace API.Battle;

/// <summary>
/// A multiplier stat. Defaults to 1000 (100%)
/// </summary>
// todo IDescribable?
public sealed class Mult : INameable, IRegistrable
{
    /// <summary>
    /// Whether higher is better
    /// </summary>
    public bool IsPositive { get; }

    /// <summary>
    /// Minimum value this is allowed to reach. Default 100 (10%)
    /// </summary>
    public int MinValue { get; init; } = 100;

    public string KeyName { get; }

    public string ModId { get; }
    public string ItemId { get; init; }

    public Mult(string modId, string keyName, bool isPositive, string? itemId = null)
    {
        this.IsPositive = isPositive;

        this.KeyName = keyName;

        this.ModId = modId;
        this.ItemId = itemId ?? keyName;

        Registry.Register(this);
    }

    public string Format(int val)
    {
        return Math.Max(val, this.MinValue).FormatPerc(isPositive: this.IsPositive);
    }

    public string FormatChange(float val)
    {
        return Math.Max(val, this.MinValue).FormatPerc(true, isPositive: this.IsPositive);
    }

    public override string ToString()
    {
        return $"{base.ToString()}: {this.GetName()}";
    }

    public string GetName(ThemeColor color)
    {
        return color.Str + this.GetLang();
    }

    public string GetName()
    {
        return this.GetName(ThemeColor.Stat);
    }
}