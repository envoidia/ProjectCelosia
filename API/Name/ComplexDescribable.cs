using System.Collections.Generic;
using System.Text;
using API.Extensions;
using API.Graphics;
using API.Modding;
using OneOf;

namespace API.Name;

/// <summary>
/// An item that can be named and described, with a description that can include formatting args
/// and the descriptions of any <c>IDescribable</c>
/// </summary>
public abstract class ComplexDescribable(GameMod? source, string keyName, string icon, string keyDesc) : IDescribable {
    public DescArg[] DescArgs { private get; init; } = [];
    public HashSet<IDescribable> DescInclusions { protected get; init; } = [];

    public GameMod? Source => source;
    public string KeyName { get; set; } = keyName;
    public string Icon { get; set; } = icon;
    public string KeyDesc { get; set; } = keyDesc;

    private string[] _GetDescArgs(GameMod? mod = null) {
        string[] args = new string[this.DescArgs.Length];
        for (int i = 0; i < this.DescArgs.Length; i++) args[i] = this.DescArgs[i].GetString(mod ?? this.Source);
        return args;
    }

    public virtual string GetName(ThemeColor color, GameMod? mod = null) =>
        $"{this.Icon} {color.Str()}{this.KeyName.GetLang(mod ?? this.Source)}";
    public virtual string GetName(GameMod? mod = null) => this.GetName(ThemeColor.White, mod ?? this.Source);

    public virtual string GetDesc(GameMod? mod = null) =>
        this.KeyDesc.FormatLang(mod ?? this.Source, this._GetDescArgs(mod ?? this.Source));

    /// <returns>
    /// The description of this with all inclusions
    /// </returns>
    public abstract string GetFullDesc(GameMod? mod = null);

    protected virtual HashSet<IDescribable> _GetDescInclusions() => this.DescInclusions;

    protected string _GetFormattedDescInclusions(GameMod? mod = null) {
        StringBuilder formattedInclusions = new(this.GetDesc(mod ?? this.Source));
        if (this.DescInclusions.Count > 0) formattedInclusions.Append('\n');

        foreach (IDescribable inclusion in this._GetDescInclusions()) {
            formattedInclusions.Append('\n').Append(ThemeColor.White.Str()).Append('(')
                .Append(inclusion.GetName(mod ?? this.Source)).Append(ThemeColor.White.Str()).Append(": ")
                .Append(inclusion.GetDesc().Replace("\n", ". ")).Append(ThemeColor.White.Str()).Append(')');
        }

        return formattedInclusions.ToString();
    }
}

public enum DescArgType {
    PlainText,
    LangKey
}

// todo unions when
// todo must take IDescribable

/// <summary>
/// A formatting argument for the description. Can be an <c>IDescribable</c> or a <c>string</c>
/// </summary>
public sealed class DescArg(OneOf<string, ComplexDescribable> value, DescArgType descriptionArgType = DescArgType.PlainText) {
    public string GetString(GameMod? mod) => value.Match(
        str => descriptionArgType == DescArgType.PlainText ? str : str.GetLang(),
        ne => ne.GetName(mod));

    public static implicit operator DescArg(string val) => new(val);
    public static implicit operator DescArg(ComplexDescribable val) => new(val);
}