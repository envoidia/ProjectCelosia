using API.Modding;
using System.Diagnostics.CodeAnalysis;
using API;
using API.Battle;
using CBattle = Celosia.Battle; // temp

namespace Celosia;

[ModEntryPoint]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public static class Main {
    /// <summary>
    /// ModId of this
    /// </summary>
    public const string Id = "Celosia";

    public static int Foo { get; set; } = 15;
    public static int Bar { get; set; } = 15;

    /// <summary>
    /// <c>GameMod</c> instance
    /// </summary>
    public static GameMod Mod { get; } = new(Id, new Version(0, 1), Lang.ResourceManager);

    static Main() {
        // Really gross temporary initialize for testing battles
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
}