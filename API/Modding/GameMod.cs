using Microsoft.Xna.Framework;

namespace API.Modding;

public abstract class GameMod {
    public const string ModNameKey = "ModName";

    public abstract string ModName { get; }

    /// <summary>
    /// Called once, on mod load
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Called every frame
    /// </summary>
    public abstract void Update(GameTime gameTime);
}