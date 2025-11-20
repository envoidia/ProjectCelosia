using System;
using System.Resources;
using Microsoft.Xna.Framework;

namespace API.Modding;

public interface IGameMod {
    /// <summary>
    /// Unique string ID for this mod. Recommended to use the mod's display name followed by some random characters
    /// </summary>
    string Id { get; }

    /// <summary>
    /// IDs of all mods that this mod depends on
    /// </summary>
    string[] DependencyIds => [];

    /// <summary>
    /// Mod version
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// Mod's <c>Lang.ResourceManager</c>
    /// </summary>
    ResourceManager ResourceManager { get; }

    /// <summary>
    /// Called once, on mod load.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Called every frame
    /// </summary>
    void Update(GameTime gameTime);
}