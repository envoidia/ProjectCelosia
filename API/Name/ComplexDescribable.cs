using System;
using System.Collections.Generic;
using System.Text;
using API.Extensions;
using API.Graphics;
using API.Modding;

namespace API.Name;

/// <summary>
/// An item that can be named and described, with a description that can include formatting args
/// and the descriptions of any <c>IDescribable</c>
/// </summary>
public abstract class ComplexDescribable(string keyName, string icon, string keyDesc) : IDescribable
{
    public DescArg[] DescArgs { private get; init; } = [];
    public HashSet<IDescribable> DescInclusions { protected get; init; } = [];

    public string KeyName { get; set; } = keyName;

    public string Icon { get; set; } = icon;
    public string KeyDesc { get; set; } = keyDesc;

    /// <summary>
    /// In case this is inherited by an <c>IRegistrable</c>
    /// </summary>
    public string ModId { get; protected init; } = Core.Id;

    private ReadOnlySpan<string> _GetDescArgs()
    {
        Span<string> args = new string[this.DescArgs.Length];

        for (int i = 0; i < this.DescArgs.Length; i++)
        {
            args[i] = this.DescArgs[i].GetString();
        }

        return args;
    }

    public override string ToString()
    {
        return $"{base.ToString()}: {this.GetName()} -- {this.GetDesc()}";
    }

    public virtual string GetName(ThemeColor color)
    {
        return $"{this.Icon} {color.Str}{this.KeyName.GetLang(this.ModId)}";
    }

    public virtual string GetName()
    {
        return this.GetName(ThemeColor.White);
    }

    public virtual string GetDesc()
    {
        return this.KeyDesc.FormatLang(this.ModId, this._GetDescArgs());
    }

    /// <returns>
    /// The description of this with all inclusions
    /// </returns>
    public abstract string GetFullDesc();

    protected virtual HashSet<IDescribable> _GetDescInclusions()
    {
        return this.DescInclusions;
    }

    protected string _GetFormattedDescInclusions()
    {
        StringBuilder formattedInclusions = new(this.GetDesc());
        HashSet<IDescribable> di = this._GetDescInclusions();

        if (di.Count > 0)
        {
            formattedInclusions.Append('\n');
        }

        foreach (IDescribable inclusion in di)
        {
            formattedInclusions.Append('\n').Append(ThemeColor.White.Str).Append('(')
                .Append(inclusion.GetName()).Append(ThemeColor.White.Str).Append(": ")
                .Append(inclusion.GetDesc().Replace("\n", ". ")).Append(ThemeColor.White.Str).Append(')');
        }

        return formattedInclusions.ToString();
    }
}