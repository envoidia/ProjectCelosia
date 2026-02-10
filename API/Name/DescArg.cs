using System;
using API.Extensions;
using OneOf;

namespace API.Name;

// todo unions when
// todo must take IDescribable

/// <summary>
/// A formatting argument for the description. Can be an <c>IDescribable</c> or a <c>string</c>
/// </summary>
public sealed class DescArg(OneOf<string, ComplexDescribable> value, DescArgType descriptionArgType = DescArgType.PlainText)
{
    public string GetString()
    {
        return value.Match(
            str => descriptionArgType == DescArgType.PlainText ? str : str.GetLang(),
            ne => ne.GetName());
    }

    public static implicit operator DescArg(string val)
    {
        return new(val);
    }

    public static implicit operator DescArg(ComplexDescribable val)
    {
        return new(val);
    }
}