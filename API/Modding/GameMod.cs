using System.Resources;
using API.Extensions;
using Microsoft.Xna.Framework;

namespace API.Modding;

public abstract class GameMod {
    public const string ModNameKey = "ModName";
    
    /// <summary>
    /// Unique string ID for this mod. Recommended to use the mod's display name followed by some random characters
    /// </summary>
    public abstract string ModId { get; }
    
    /// <summary>
    /// ModIds for all mods that this mod depends on
    /// </summary>
    public abstract string[] DependencyIds { get; }
    
    /// <summary>
    /// Mod's Lang.ResourceManager
    /// </summary>
    public abstract ResourceManager ResourceManager { get; }

    /// <summary>
    /// Called once, on mod load
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Called every frame
    /// </summary>
    public abstract void Update(GameTime gameTime);

    /// <summary>
    /// Gets a string from a lang key from this mod's specified ResourceManager. Crashes on invalid key
    /// </summary>
    public string GetLang(string str) => str.GetLangRm(this.ResourceManager);

    /// <summary>
    /// Gets a formatted string from a lang key from this mod's ResourceManager. Crashes on invalid key or 0 args
    /// </summary>
    public string FormatLang(string str, params object?[] args) => str.FormatLangRm(this.ResourceManager, args);

    /// <summary>
    /// Gets an ICU MessageFormat-formatted string from a lang key from this mod's ResourceManager. Crashes on invalid key or 0 args
    /// </summary>
    public string FormatIcu(string str, params object?[] args) => str.FormatIcuRm(this.ResourceManager, args);
}