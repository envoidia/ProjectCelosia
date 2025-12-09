using System.Collections.Generic;
using System.Text;
using API.Extensions;
using API.Graphics;
using API.Modding;
using OneOf;

namespace API.Name;

public abstract class ComplexDescribable(string keyName, string icon, string keyDesc) : IDescribable {
    public DescArg[] DescArgs { private get; init; } = [];
    public HashSet<IDescribable> DescInclusions { protected get; init; } = [];

    public string KeyName => keyName;
    public string Icon => icon;
    public string KeyDesc => keyDesc;

    private string[] _GetDescArgs(GameMod? mod = null) {
        string[] args = new string[this.DescArgs.Length];
        for (int i = 0; i < this.DescArgs.Length; i++) args[i] = this.DescArgs[i].GetString(mod);
        return args;
    }

    public virtual string GetName(string color, GameMod? mod = null) =>
        $"{this.Icon} {color}{this.KeyName.GetLang(mod)}";
    public virtual string GetName(GameMod? mod = null) => this.GetName(Colors.White, mod);

    public virtual string GetDesc(GameMod? mod = null) =>
        this.KeyDesc.FormatLang(mod, this._GetDescArgs(mod));

    public abstract string GetFullDesc(GameMod? mod = null);

    protected virtual HashSet<IDescribable> _GetDescInclusions() => this.DescInclusions;

    protected string _GetFormattedDescInclusions(GameMod? mod = null) {
        StringBuilder formattedInclusions = new(this.GetDesc(mod));
        if (this.DescInclusions.Count > 0) formattedInclusions.Append('\n');

        foreach (IDescribable inclusion in this._GetDescInclusions()) {
            formattedInclusions.Append('\n').Append(Colors.White).Append('(')
                .Append(inclusion.GetName(mod)).Append(Colors.White).Append(": ")
                .Append(inclusion.GetDesc().Replace("\n", ". ")).Append(Colors.White).Append(')');
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
public sealed class DescArg(OneOf<string, ComplexDescribable> value, DescArgType descriptionArgType = DescArgType.PlainText) {
    public string GetString(GameMod? mod) => value.Match(
        str => descriptionArgType == DescArgType.PlainText ? str : str.GetLang(),
        ne => ne.GetName(mod));

    public static implicit operator DescArg(string val) => new(val);
    public static implicit operator DescArg(ComplexDescribable val) => new(val);
}