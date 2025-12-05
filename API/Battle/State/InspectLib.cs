using API.Graphics;
using API.Input;
using API.Menu.State;
using Microsoft.Xna.Framework;
using static API.Input.InputPrompts;
using static API.Battle.State.BattleLib;
using System;

namespace API.Battle.State;

public sealed class InspectLib {

    #region Display Fields

    private static readonly Label[] _PageList = new Label[TeamSize];

    private const int Y = 600;
    private static readonly Vector2[] _PointsPageDivL = [new(30, Y), new(370, Y)];
    private static readonly Vector2[] _PointsPageDivR = [new(900, Y), new(1450, Y)];

    private static readonly Vector2[] _PointsMult = [new(60, Y), new(660, Y)];
    private static readonly Vector2[] _PointsMod = [new(60 + 675, Y), new(660 + 675, Y)];
    private static readonly Vector2[] _PointsOther = [new(60 + 1350, Y), new(660 + 1350, Y)];

    private static readonly Label _PageItemList = new(Core.StageInspect);
    private static readonly Label _PageItemRightList = new(Core.StageInspect);
    private static readonly Label _DescHeader = new(Core.StageInspect);
    private static readonly Label _Desc = new(Core.StageInspect);

    // Stat, Equip, Affinity, Mult, Mod, Other, LT, RT, L, R
    // todo does this get discarded immediately? does the jit stackalloc this?
    private static readonly int[] _PromptX = [960 + 330, 700, 300, 300, 750, 1125, 310, 0, 385, 857];

    private static readonly int[] _PromptY = [110 + 60, 245, 245, 385, 385, 385, 52, 52, 320, 320];

    private static readonly InputPrompt[] _PromptTypes = [
        InspectStat, InspectAffinity,
        InspectEquip, InspectMult, InspectMod, InspectOther,
        InspectUnitL, InspectUnitR,
        InspectPageL, InspectPageR
    ];

    private static readonly Label[] _Prompts = new Label[10];

    private static readonly Label _Equip = new(Core.StageInspect) { Position = new Vector2(450, 320) };

    private static readonly Label _Affinities = new(Core.StageInspect) { Position = new Vector2(1050, 320) };

    private static readonly Label[] _StatsBasic = new Label[StatCount];
    private static readonly Label[] _StatsBasicNum = new Label[StatCount];

    private static readonly Label _Hp = new(Core.StageInspect, Lang.Hp) { Position = new Vector2(450, 165) };

    private static readonly Label _HpAmt = new(Core.StageInspect) {
        Position = new Vector2(900, 165),
        Alignment = Alignment.TopRight
    };
    //private static GuiBoxBar hpBar = coolRectBars[CoolRectBars.HP_INSPECT.ordinal()]; todo

    private static readonly Label _Sp = new(Core.StageInspect, Lang.Sp) { Position = new Vector2(450, 210) };

    private static readonly Label _SpAmt = new(Core.StageInspect) {
        Position = new Vector2(450, 210),
        Alignment = Alignment.TopRight
    };
    //private static GuiBoxBar spBar = coolRectBars[CoolRectBars.SP_INSPECT.ordinal()];

    private static readonly Label[] _StatCategoryHeaders = new Label[StatTypeCount];

    /* todo
    private static readonly string multNames = getNamesAsMultiline(Mult.values(), Mult::getName);
    private static readonly string modNames = getNamesAsMultiline(Mod.values(), Mod::getName);
    private static readonly string otherNames = getNamesAsMultiline(new BooleanStat[] { BooleanStat.EFFECT_BLOCK,
                                           BooleanStat.INFINITE_SP, BooleanStat.UNABLE_TO_ACT, BooleanStat.EQUIP_DISABLED }, BooleanStat::getName) +
                                       "\n" + lang.get("extra_actions");
    private static readonly string[] statCategoryNames = [multNames, modNames, otherNames];*/

    private static readonly Label[] _StatsPage = new Label[StatTypeCount];
    private static readonly Label[] _StatsPageNum = new Label[StatTypeCount];

    // Names
    // Todo ensure can be retranslated midgame
    private static readonly string[] _PageNames = [Lang.Skills, Lang.Passives, Lang.Buffs, Lang.Stats];

    private static readonly string[] _StatNames =
        [Lang.StatStr, Lang.StatMag, Lang.StatFth, Lang.StatAmr, Lang.StatRes, Lang.StatAgi];

    private static readonly string[] _StatCategoryHeaderNames = [Lang.InfoMult, Lang.InfoMod, Lang.InfoOther];

    #endregion

    #region Logic Fields

    // todo
    internal enum _InspectPage {
        Skills,
        Passives,
        Buffs,
        Stats
    }

    internal static _InspectPage curPage = _InspectPage.Skills;
    private static int _indexPageList = 0;
    private static TimeSpan _timeOnSameTarget = TimeSpan.Zero;

    #endregion

    #region Methods

    public static void Update(GameTime gameTime) {
        HandleDebug();

        if (InputLib.Check(Keybinds.Back)) {
            NavPath.Remove();
            return;
        }

        // todo

        // UpdateInputPrompt on page change
    }

    private static void _HandleInspectPage() { }

    private static void _SetStatVisibility(bool isStatsPage) { }

    private static void _SetPageItemVisibility(bool visible) { }

    private static void _DeleteInspect() { }

    #endregion
}
