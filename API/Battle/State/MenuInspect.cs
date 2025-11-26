using API.Graphics;
using API.Input;
using API.Menu.State;
using Microsoft.Xna.Framework;
using static API.Input.InputPrompts;
using static API.Battle.State.BattleHandler;
using System;
using API.Extensions;

namespace API.Battle.State;

public sealed class MenuInspect : IState {

    #region Display Fields
    private static readonly Label[] PageList = new Label[TeamSize];

    private const int Y = 600;
    private static readonly Vector2[] PointsPageDivL = [new(30, Y), new(370, Y)];
    private static readonly Vector2[] PointsPageDivR = [new(900, Y), new(1450, Y)];

    private static readonly Vector2[] PointsMult = [new(60, Y), new(660, Y)];
    private static readonly Vector2[] PointsMod = [new(60 + 675, Y), new(660 + 675, Y)];
    private static readonly Vector2[] PointsOther = [new(60 + 1350, Y), new(660 + 1350, Y)];

    private static readonly Label PageItemList = new(Core.StageInspect);
    private static readonly Label PageItemRightList = new(Core.StageInspect);
    private static readonly Label DescHeader = new(Core.StageInspect);
    private static readonly Label Desc = new(Core.StageInspect);

    // Stat, Equip, Affinity, Mult, Mod, Other, LT, RT, L, R
    // todo does this get discarded immediately? does the jit stackalloc this?
    private static readonly int[] PromptX = [960 + 330, 700, 300, 300, 750, 1125, 310, 0, 385, 857];

    private static readonly int[] PromptY = [110 + 60, 245, 245, 385, 385, 385, 52, 52, 320, 320];

    private static readonly InputPrompt[] PromptTypes = [
        InspectStat, InspectAffinity,
        InspectEquip, InspectMult, InspectMod, InspectOther,
        InspectUnitL, InspectUnitR,
        InspectPageL, InspectPageR
    ];

    private static readonly Label[] Prompts = new Label[10];

    private static readonly Label Equip = new(Core.StageInspect) { Position = new Vector2(450, 320) };

    private static readonly Label Affinities = new(Core.StageInspect) { Position = new Vector2(1050, 320) };

    private static readonly Label[] StatsBasic = new Label[StatCount];
    private static readonly Label[] StatsBasicNum = new Label[StatCount];

    private static readonly Label Hp = new(Core.StageInspect, Lang.Hp) { Position = new Vector2(450, 165) };

    private static readonly Label HpAmt = new(Core.StageInspect) {
        Position = new Vector2(900, 165),
        Alignment = Alignment.TopRight
    };
    //private static GuiBoxBar hpBar = coolRectBars[CoolRectBars.HP_INSPECT.ordinal()]; todo

    private static readonly Label Sp = new(Core.StageInspect, Lang.Sp) { Position = new Vector2(450, 210) };

    private static readonly Label SpAmt = new(Core.StageInspect) {
        Position = new Vector2(450, 210),
        Alignment = Alignment.TopRight
    };
    //private static GuiBoxBar spBar = coolRectBars[CoolRectBars.SP_INSPECT.ordinal()];

    private static readonly Label[] StatCategoryHeaders = new Label[StatTypeCount];

    /* todo
    private static readonly string multNames = getNamesAsMultiline(Mult.values(), Mult::getName);
    private static readonly string modNames = getNamesAsMultiline(Mod.values(), Mod::getName);
    private static readonly string otherNames = getNamesAsMultiline(new BooleanStat[] { BooleanStat.EFFECT_BLOCK,
                                           BooleanStat.INFINITE_SP, BooleanStat.UNABLE_TO_ACT, BooleanStat.EQUIP_DISABLED }, BooleanStat::getName) +
                                       "\n" + lang.get("extra_actions");
    private static readonly string[] statCategoryNames = [multNames, modNames, otherNames];*/

    private static readonly Label[] StatsPage = new Label[StatTypeCount];
    private static readonly Label[] StatsPageNum = new Label[StatTypeCount];

    // Names
    // Todo ensure can be retranslated midgame
    private static readonly string[] PageNames = [Lang.Skills, Lang.Passives, Lang.Buffs, Lang.Stats];

    private static readonly string[] StatNames =
        [Lang.StatStr, Lang.StatMag, Lang.StatFth, Lang.StatAmr, Lang.StatRes, Lang.StatAgi];

    private static readonly string[] StatCategoryHeaderNames = [Lang.InfoMult, Lang.InfoMod, Lang.InfoOther];

    #endregion

    #region Logic Fields
    // todo
    private static int indexPage = 0;
    private static int indexPageList = 0;
    private static TimeSpan timeOnSameTarget = TimeSpan.Zero;

    private enum InspectPage {
        Skills,
        Passives,
        Buffs,
        Stats
    }

    #endregion

    public MenuInspect() {
        if (Core.MenuInspect is not null) {
            throw new InvalidOperationException(string.Format(Lang.MultipleInstance, nameof(MenuInspect)));
        }
    }

    public void Update(GameTime gameTime) {
        HandleDebug();

        if (Core.Input.CheckInput(Keybinds.Back)) {
            Core.NavPath.Remove();
            return;
        }

        // todo
    }

    public void Draw(GameTime gameTime) {
        Core.StageBattle.Draw(gameTime);
        Core.StageInspect.Draw(gameTime);
    }

    public string GetInputPrompt() => IState.GetInputPromptString(Back);

    private static void HandleInspectPage() { }

    private static void SetStatVisibility(bool isStatsPage) { }

    private static void SetPageItemVisibility(bool visible) { }

    private static void DeleteInspect() { }
}
