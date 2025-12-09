using API.Modding;

namespace API.Name;

/// <summary>
/// An item that can be named and described
/// </summary>
public interface IDescribable : INameable {
    string KeyDesc { get; }

    string GetDesc(GameMod? mod = null);
}
