using API.Extensions;
using OneOf;

namespace API.Name;

// todo unions when
// todo must take IDescribable

/// <summary>
/// A formatting argument for the description. Can be an <c>IDescribable</c> or a <c>string</c>
/// </summary>
// todo make way more robust and stuff
// goals:
// -more types
// -dont always supply as string
// -auto-insert some args (like buff turn counts)
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