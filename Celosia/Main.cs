using System.Resources;
using API;
using Microsoft.Xna.Framework;
using API.Modding;
using API.Graphics;
using Celosia.Battle;
using JetBrains.Annotations;

namespace Celosia;

[UsedImplicitly]
public class Main : IGameMod {
    /// <summary>
    /// Globally accessible Main instance
    /// </summary>
    public static IGameMod ModInstance { get; private set; } = null!;

    public string Id => "Celosia";
    public string Version => BuildInfo.BuildDate;
    public ResourceManager ResourceManager => Lang.ResourceManager;

    private static readonly Label TestLabel = new() {
        Position = new Vector2(1800, 800),
        Text = "",
        Width = 2000
    };

    public void Initialize() {
        if (ModInstance is not null) {
            throw new InvalidOperationException(Lang.MultipleInstance);
        }

        ModInstance = this;

        TestLabel.Text = Accessories.FirebornRing.GetDescriptionWithInclusions(this);
    }

    public void Update(GameTime gameTime) { }
}