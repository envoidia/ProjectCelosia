using System.Resources;
using API.Extensions;
using Microsoft.Xna.Framework;

namespace API.Modding;

public interface IGameMod {
    /// <summary>
    /// Lang key that should be used for the mod's display name
    /// </summary>
    const string ModNameKey = "ModName";

    /// <summary>
    /// Unique string ID for this mod. Recommended to use the mod's display name followed by some random characters
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Ids for all mods that this mod depends on
    /// </summary>
    string[] DependencyIds => [];

    /// <summary>
    /// Mod version
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Mod's Lang.ResourceManager
    /// </summary>
    ResourceManager ResourceManager { get; }

    /// <summary>
    /// Called once, on mod load
    /// </summary>
    void Initialize();

    /// <summary>
    /// Called every frame
    /// </summary>
    void Update(GameTime gameTime);

    /// <summary>
    /// Returns this mod's name
    /// </summary>
    string GetName() => ModNameKey.GetLang(this);
}