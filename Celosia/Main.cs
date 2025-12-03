using System.Resources;
using Microsoft.Xna.Framework;
using API.Modding;
using System.Diagnostics.CodeAnalysis;
using API.Extensions;
using API;
using API.Battle;
using CBattle = Celosia.Battle; // temp

namespace Celosia;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public sealed class Main : IGameMod {
    /// <summary>
    /// Publically accessible instance of <c>Celosia.Main</c>
    /// </summary>
    public static IGameMod ModInstance { get; set; } = null!;

    public string Id => "Celosia";
    public Version Version => new(0, 1);
    public ResourceManager ResourceManager => Lang.ResourceManager;

    public void Initialize() {
        // Ensure that only 1 instance of Main is created
        if (ModInstance is not null) {
            throw new InvalidOperationException(string.Format(API.Lang.MultipleInstance, nameof(Main)));
        }

        ModInstance = this;

        Core.battle = new API.Battle.Battle(new Team(
            new Unit(CBattle.UnitTypes.Johny, 19, null, CBattle.Skills.Fireball, Skills.Defend),
            new Unit(UnitTypes.TestUnitType, 19, null, CBattle.Skills.Fireball, Skills.Defend),
            new Unit(UnitTypes.TestUnitType, 19, null, Skills.Nothing, Skills.Defend),
            new Unit(UnitTypes.TestUnitType, 19, null, CBattle.Skills.Fireball, Skills.Defend)),
            new Team(new Unit(UnitTypes.TestUnitType, 19, null, Skills.Nothing, Skills.Defend),
                new Unit(UnitTypes.TestUnitType, 19, null, CBattle.Skills.Fireball, Skills.Defend),
                new Unit(CBattle.UnitTypes.Johny, 19, null, Skills.Nothing, Skills.Defend),
                new Unit(UnitTypes.TestUnitType, 19, null, CBattle.Skills.Fireball, Skills.Defend)));
    }

    public void Update(GameTime gameTime) { }
}