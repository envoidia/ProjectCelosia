using API.Extensions;
using API.Name;

namespace API.Modding;

/// <summary>
/// An item that has a string ID and knows the string ID of the mod it's from, to be added to the <c>Registry</c>
/// </summary>
public interface IRegistrable
{
    /// <summary>
    /// ID of the <c>GameMod</c> this is from
    /// </summary>
    string ModId { get; }

    /// <summary>
    /// ID of this
    /// </summary>
    string ItemId { get; }
}

public static class RegistrableExtensions
{
    extension(IRegistrable @this)
    {
        /// <returns>
        /// ID of this with both mod and item parts
        /// </returns>
        public string GetId()
        {
            return $"{@this.ModId}:{@this.ItemId}";
        }

        /// <returns>
        /// Name of this. If this is <c>INameable</c>, uses its <c>KeyName</c>. Otherwise, just displays this' ID
        /// </returns>
        public string GetLang()
        {
            return (@this is INameable nameable)
            ? nameable.KeyName.GetLang(@this.ModId)
            : @this.GetId();
        }
    }
}
