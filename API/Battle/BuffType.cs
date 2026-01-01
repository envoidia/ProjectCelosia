using API.Extensions;
using API.Util;

namespace API.Battle;

// todo name
public enum BuffType
{
    Buff,
    Debuff
}

public static class BuffTypeExtensions
{
    extension(BuffType @this)
    {
        public string GetName()
        {
            return @this switch
            {
                BuffType.Buff => "Buff".GetLang(),
                BuffType.Debuff => "Debuff".GetLang(),
                _ => throw new ClosedEnumsWhenException()
            };
        }
    }
}