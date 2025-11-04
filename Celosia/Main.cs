using System.Resources;
using Microsoft.Xna.Framework;
using API.Modding;
using API.Graphics;
using Celosia.Battle;

namespace Celosia;

public class Main : GameMod {
    public override string ModId { get; } = "Celosia";
    
    public override string[] DependencyIds { get; } = [];

    public override ResourceManager ResourceManager { get; } = Lang.ResourceManager;

    private static readonly Label TestLabel = new() {
        Position = new Vector2(1800, 1400),
        Text = "",
        Width = 2000
    };

    public override void Initialize() {
        Accessories.Initialize(this);
        // todo Buffs
        Elements.Initialize(this);
        Mults.Initialize(this);
        Passives.Initialize(this);
        // todo Skills
        // todo UnitTypes
        // todo Weapons
        
        TestLabel.Text = this.GetLang("ElementIgnis");
    }

    public override void Update(GameTime gameTime) { }
}