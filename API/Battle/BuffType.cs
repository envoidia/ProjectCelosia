using API.Extensions;
using API.Util;

namespace API.Battle;

// Member names must match lang key names
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
            return @this.ToString().GetLang();
        }
    }
}