using System;
using System.Collections.Generic;
using System.Text;
using API.Entity;
using API.Extensions;
using API.Graphics;
using API.Modding;
using OneOf;

namespace API.Name;

public sealed class DescData(IComplexDescribable describable) {
    public DescArg[] DescriptionArgs { private get; init; } = [];
    public HashSet<IDescribable> DescriptionInclusions { get; init; } = [];

    public string _GetFormattedDescriptionInclusions(GameMod? mod = null) {
        StringBuilder formattedInclusions = new(describable.GetDesc(mod));
        if (this.DescriptionInclusions.Count > 0) formattedInclusions.Append('\n');

        foreach (IDescribable desc in this.DescriptionInclusions) {
            formattedInclusions.Append('\n').Append(Colors.White).Append('(').Append(desc.GetName(mod))
                .Append(Colors.White).Append(": ").Append(desc.GetDesc().Replace("\n", ". "))
                .Append(Colors.White).Append(')');
        }

        return formattedInclusions.ToString();
    }

    private string[] _GetDescriptionArgs(GameMod? mod = null) {
        string[] args = new string[this.DescriptionArgs.Length];

        for (int i = 0; i < this.DescriptionArgs.Length; i++) {
            args[i] = this.DescriptionArgs[i].GetString(mod);
        }

        return args;
    }
}

public enum DescArgType {
    PlainText,
    LangKey
}

// todo unions when
public sealed class DescArg(OneOf<string, INameable> value, DescArgType descriptionArgType = DescArgType.PlainText) {
    public string GetString(GameMod? mod) => value.Match(
        str => descriptionArgType == DescArgType.PlainText ? str : str.GetLang(),
        ne => ne.GetName(mod: mod));

    public static implicit operator DescArg(string val) => new(val);
    public static implicit operator DescArg(INameable val) => new(val);
}