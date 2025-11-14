using System.Collections.Generic;
using System.Text;
using API.Battle;
using API.Extensions;
using API.Graphics;
using API.Modding;
using OneOf;
using static API.Entity.DescriptionArgType;

namespace API.Entity;

public abstract class ComplexDescriptionEntity(string name, string keyDescription, string icon)
    : IconEntity(name, keyDescription, icon) {
    public DescriptionArg[] DescriptionArgs { private get; init; } = [];
    public HashSet<DescriptionEntity> DescriptionInclusions { protected get; init; } = [];

    private string[] GetDescriptionArgs(IGameMod? mod = null) {
        string[] args = new string[this.DescriptionArgs.Length];

        for (uint i = 0; i < this.DescriptionArgs.Length; i++) {
            args[i] = this.DescriptionArgs[i].GetString(mod);
        }

        return args;
    }

    protected virtual HashSet<DescriptionEntity> GetDescriptionInclusions() => this.DescriptionInclusions;

    protected string GetFormattedDescriptionInclusions(IGameMod? mod = null) {
        StringBuilder formattedInclusions = new(this.GetDescription(mod));
        if (this.DescriptionInclusions.Count > 0) formattedInclusions.Append('\n');

        foreach (DescriptionEntity entity in this.GetDescriptionInclusions()) {
            string color = entity is Skill ? Colors.Skill : Colors.Buff; // todo more thorough
            formattedInclusions.Append('\n').Append(Colors.White).Append('(').Append(entity.GetName(color, mod))
                .Append(Colors.White).Append(": ").Append(entity.GetDescription().Replace("\n", ". "))
                .Append(Colors.White).Append(')');
        }

        return formattedInclusions.ToString();
    }

    public abstract string GetDescriptionWithInclusions(IGameMod? mod = null);

    public override string GetDescription(IGameMod? mod = null) =>
        string.Format(base.GetDescription(mod), this.GetDescriptionArgs(mod));
}

public enum DescriptionArgType {
    PlainText,
    LangKey
}

// todo if base C# gets unions, use that
public class DescriptionArg(OneOf<string, NamedEntity> value, DescriptionArgType descriptionArgType = PlainText) {
    public string GetString(IGameMod? mod) => value.Match(
        str => descriptionArgType == PlainText ? str : str.GetLang(),
        ne => ne.GetName(mod));

    public static implicit operator DescriptionArg(string val) => new(val);
    public static implicit operator DescriptionArg(NamedEntity val) => new(val);
}