using API.Modding;

namespace API.Name;

/// <summary>
/// An item that can be named and described
/// </summary>
public interface IDescribable : INameable {
    /// <summary>
    /// The lang key that holds this' description
    /// </summary>
    string KeyDesc { get; }

    /// <returns>
    /// This' description. Typical impl: <c>=> this.KeyDesc.GetLang(mod ?? this.Source)</c>
    /// </returns>
    string GetDesc(GameMod? mod = null);
}
