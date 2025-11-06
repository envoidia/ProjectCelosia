using System.Resources;
using API;
using Microsoft.Xna.Framework;
using API.Modding;
using API.Graphics;
using Celosia.Battle;

namespace Celosia;

public class Main : IGameMod {
    public string Id => "Celosia";
    public string Version => BuildInfo.BuildDate;
    public ResourceManager ResourceManager => Lang.ResourceManager;

    private static readonly Label TestLabel = new() {
        Position = new Vector2(1800, 1400),
        Text = "",
        Width = 2000
    };

    public void Initialize() {
        Accessories.Initialize(this);
        // todo Buffs
        Elements.Initialize(this);
        Mults.Initialize(this);
        Passives.Initialize(this);
        // todo Skills
        // todo UnitTypes
        // todo Weapons

        TestLabel.Text = Lang.ElementIgnis + "/i[KF1]/i[fire-ring]";
    }

    public void Update(GameTime gameTime) { }
}