using API.Graphics;
using API.Modding;

namespace API.Name;

/// <summary>
/// An item that can be named
/// </summary>
public interface INameable : IModItem {
    /// <summary>
    /// The lang key that holds this' name
    /// </summary>
    string KeyName { get; }

    /// <returns>
    /// This' name. Typical impl: <c>=> color.Str() + this.KeyName.GetLang(mod ?? this.Source)</c> or
    /// <c>=> $"{this.Icon} {color.Str()}{this.KeyName.GetLang(mod ?? this.Source)}"</c>
    /// </returns>
    string GetName(ThemeColor color, GameMod? mod = null);

    /// <returns>This' name. Should defer to GetName(ThemeColor, GameMod?) with default color</returns>
    string GetName(GameMod? mod = null);
}
