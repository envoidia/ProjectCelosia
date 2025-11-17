using System.Resources;
using API;
using Microsoft.Xna.Framework;
using API.Modding;
using JetBrains.Annotations;

namespace Celosia;

[UsedImplicitly]
public sealed class Main : IGameMod {
    /// <summary>
    /// Publicly accessible instance of <c>Main</c>
    /// </summary>
    public static IGameMod ModInstance { get; set; } = null!;

    public string Id => "Celosia";
    public string Version => BuildInfo.BuildDate;
    public ResourceManager ResourceManager => Lang.ResourceManager;

    public void Initialize() {
        // Ensure that only 1 instance of Main is created
        if (ModInstance is not null) {
            throw new InvalidOperationException(string.Format(API.Lang.ModMultipleInstance, Lang.ModName));
        }

        ModInstance = this;
    }

    public void Update(GameTime gameTime) { }
}