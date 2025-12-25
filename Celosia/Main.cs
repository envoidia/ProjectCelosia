using API.Modding;
using System.Diagnostics.CodeAnalysis;
using API;
using API.Battle;
using CBattle = Celosia.Battle;

namespace Celosia;

[ModEntryPoint]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public static class Main {
    /// <summary>
    /// <c>GameMod</c> instance
    /// </summary>
    public static GameMod Mod { get; } = new(Core.BaseModId, new Version(0, 1));

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

        // Add lang file
        API.Lang.Language.AddLangFile(API.Lang.Language.EnUS, Core.BaseModId, "Lang/CelosiaLang.en-US.properties");
    }
}