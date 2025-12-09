using API.Modding;

namespace API.Name;

/// <summary>
/// An item that can be named
/// </summary>
public interface INameable {
    string KeyName { get; }

    string GetName(string color, GameMod? mod = null);
    string GetName(GameMod? mod = null);

}
