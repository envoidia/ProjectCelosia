using API.Modding;
using System.Diagnostics.CodeAnalysis;
using API;
using API.Battle;
using CBattle = Celosia.Battle;

namespace Celosia;

[ModEntryPoint]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public static class Main
{
    /// <summary>
    /// <c>GameMod</c> instance
    /// </summary>
    public static GameMod Mod { get; } = new(Core.BaseModId, new Version(0, 1))
    {
        OnInit = _Init
    };

    private static void _Init()
    {
        // Really gross temporary initialize for testing battles
        const int Lvl = 14;
        Core.Battle = new API.Battle.Battle(new Team(
            new Unit(CBattle.UnitTypes.Johny, Lvl, null, CBattle.Skills.Fireball,
            CBattle.Skills.Fireball, CBattle.Skills.Fireball, CBattle.Skills.Fireball, CBattle.Skills.ChainLightning, Skills.Defend),
            new Unit(UnitTypes.TestUnitType, Lvl, null, CBattle.Skills.Fireball, Skills.Defend),
            new Unit(UnitTypes.TestUnitType, Lvl, null, Skills.Nothing, Skills.Defend),
            new Unit(UnitTypes.TestUnitType, Lvl, null, CBattle.Skills.Fireball, Skills.Defend)),
            new Team(new Unit(UnitTypes.TestUnitType, Lvl, null, Skills.Nothing, Skills.Defend),
                new Unit(UnitTypes.TestUnitType, Lvl, null, CBattle.Skills.Fireball, Skills.Defend),
                new Unit(CBattle.UnitTypes.Johny, Lvl, null, Skills.Nothing, Skills.Defend),
                new Unit(UnitTypes.TestUnitType, Lvl, null, CBattle.Skills.Fireball, Skills.Defend)));

        _ReloadLang();
        API.Lang.Language.OnReload += _ReloadLang;
    }

    private static void _ReloadLang()
    {
        API.Lang.Language.AddLangFile(API.Lang.Language.EnUS, Core.BaseModId,
            "Lang/CelosiaLang.en-US.properties", false);
    }
}