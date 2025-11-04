using System.Resources;
using Microsoft.Xna.Framework;
using API.Battle;
using API.Extensions;
using API.Modding;
using API.Graphics;

namespace Celosia;

public class Main : GameMod {
    public override string ModId { get; } = "Celosia";
    public override string[] DependencyIds { get; } = [];

    public override ResourceManager ResourceManager { get; } = Lang.ResourceManager;

    public static Element Vis;

    private static readonly Label TestLabel = new() {
        Position = new Vector2(1800, 1400),
        Text = "",
        Width = 2000
    };

    public override void Initialize() {
        Vis = new Element(this, "ElementVis", "ElementVisDesc", "/c[lightGray]/i[rolling-energy]");
        TestLabel.Text = this.GetLang("ElementVis");
    }

    public override void Update(GameTime gameTime) { }
}