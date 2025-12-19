using API.Graphics;

namespace API.Name;

/// <summary>
/// An item that can be named
/// </summary>
public interface INameable {
    /// <summary>
    /// The lang key that holds this' name
    /// </summary>
    string KeyName { get; }

    /// <returns>
    /// This' name
    /// </returns>
    string GetName(ThemeColor color);

    /// <returns>This' name</returns>
    string GetName();
}
