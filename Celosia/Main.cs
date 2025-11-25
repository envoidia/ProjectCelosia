using System.Resources;
using Microsoft.Xna.Framework;
using API.Modding;
using System.Diagnostics.CodeAnalysis;
using API.Extensions;

namespace Celosia;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public sealed class Main : IGameMod {
    /// <summary>
    /// Publicly accessible instance of <c>Main</c>
    /// </summary>
    public static IGameMod ModInstance { get; set; } = null!;

    public string Id => "Celosia";
    public Version Version => new(0, 1);
    public ResourceManager ResourceManager => Lang.ResourceManager;

    public void Initialize() {
        // Ensure that only 1 instance of Main is created
        if (ModInstance is not null) {
            throw new InvalidOperationException("MultipleInstance".FormatLang(nameof(Main)));
        }

        ModInstance = this;
    }

    public void Update(GameTime gameTime) { }
}