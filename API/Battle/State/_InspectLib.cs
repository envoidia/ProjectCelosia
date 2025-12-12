using API.Graphics;
using API.Input;
using API.Menu.State;
using Microsoft.Xna.Framework;
using static API.Input.InputPrompts;
using static API.Battle.State.BattleLib;
using System;
using System.Collections.Generic;
using API.Util;
using API.Menu;

namespace API.Battle.State;

internal sealed class _InspectLib {

    #region Display Fields

    // todo ensure this size is right
    private const int _ActorCount = 53;
    private static readonly List<IActor> _Actors = new(_ActorCount);

    private const int _AnimPrimActorCount = 6; //8;
    private static readonly List<IActor> _AnimPrimActors = new(_AnimPrimActorCount);

    private static Menu.Menu _menu = null!;

    // Stat types
    private static readonly Label[] _StatCategoryHeaders = new Label[_StatTypeCount];

    /* todo
    private static readonly string multNames = getNamesAsMultiline(Mult.values(), Mult::getName);
    private static readonly string modNames = getNamesAsMultiline(Mod.values(), Mod::getName);
    private static readonly string otherNames = getNamesAsMultiline(new BooleanStat[] { BooleanStat.EFFECT_BLOCK,
                                           BooleanStat.INFINITE_SP, BooleanStat.UNABLE_TO_ACT, BooleanStat.EQUIP_DISABLED }, BooleanStat::getName) +
                                       "\n" + lang.get("extra_actions");
    private static readonly string[] statCategoryNames = [multNames, modNames, otherNames];*/

    /// <summary>
    /// List of complex stats (mults and etc)
    /// </summary>
    private static readonly Label[] _StatsPage = new Label[_StatTypeCount];

    /// <inheritdoc cref="_StatsPage" />
    private static readonly Label[] _StatsPageNum = new Label[_StatTypeCount];

    // Page list
    private static readonly Label[] _PageList = new Label[TeamSize];
    private static TabBarWidget _pageTabs = null!;
    // private static readonly GuiBoxChain _PageListBox = new(638, 446, 501) { Priority = RenderPriority.B2Med };

    // Basic stat list
    /// <summary>
    /// List of basic stats
    /// </summary>
    private static readonly Label[] _StatsBasic = new Label[StatCount];

    /// <inheritdoc cref="_StatsBasic" />
    private static readonly Label[] _StatsBasicNum = new Label[StatCount];

    // Unit tabs
    private static readonly string[] _UnitList = new string[UnitCount];

    private static TabBarWidget _unitTabs = null!;
    //private static readonly GuiBoxChain _UnitListBox = new(518, 40, 106) { Priority = RenderPriority.B2Med };

    // Input prompts
    private static readonly Label[] _Prompts = new Label[10];
    private static readonly InputPrompt[] _PromptTypes = [
            InspectStat, InspectAffinity,
            InspectEquip, InspectMult, InspectMod, InspectOther,
            InspectUnitL, InspectUnitR,
            InspectPageL, InspectPageR];

    // Dividing paths
    private const int _Y = 600;
    private static readonly Path _PageDivL = new(new(30, _Y), new(370, _Y),
        RenderPriority.B2High) { Speed = 4f };
    private static readonly Path _PageDivR = new(new(900, _Y), new(1450, _Y),
        RenderPriority.B2High) { Speed = 4f };

    private static readonly Path _MultP = new(new(60, _Y), new(660, _Y),
        RenderPriority.B2High) { Speed = 4f };
    private static readonly Path _ModP = new(new(60 + 675, _Y), new(660 + 675, _Y),
        RenderPriority.B2High) { Speed = 4f };
    private static readonly Path _OtherP = new(new(60 + 1350, _Y), new(660 + 1350, _Y),
        RenderPriority.B2High) { Speed = 4f };

    // Current unit items
    private static readonly Label _Equip = new(RenderPriority.B2High) { Position = new Vector2(450, 320) };
    private static readonly Label _Affinities = new(RenderPriority.B2High) { Position = new Vector2(1050, 320) };

    private static readonly Label _Hp = new(RenderPriority.B2High) { Position = new Vector2(450, 165) };
    private static readonly Label _HpAmt = new(RenderPriority.B2High) {
        Position = new Vector2(900, 165),
        Alignment = Alignment.TopRight
    };
    //private static GuiBoxBar hpBar = coolRectBars[CoolRectBars.HP_INSPECT.ordinal()]; todo

    private static readonly Label _Sp = new(RenderPriority.B2High) { Position = new Vector2(450, 210) };
    private static readonly Label _SpAmt = new(RenderPriority.B2High) {
        Position = new Vector2(450, 210),
        Alignment = Alignment.TopRight
    };
    //private static GuiBoxBar spBar = coolRectBars[CoolRectBars.SP_INSPECT.ordinal()];

    // Current page items
    private static readonly Label _PageItemList = new(RenderPriority.B2High);
    private static readonly Label _PageItemRightList = new(RenderPriority.B2High);
    private static readonly Label _DescHeader = new(RenderPriority.B2High);
    private static readonly Label _Desc = new(RenderPriority.B2High);

    #endregion

    #region Logic Fields

    // todo
    private enum _InspectPage {
        Skills,
        Passives,
        Buffs,
        Stats
    }

    private const int _StatTypeCount = 3;
    private const int _PageCount = 4;
    private const int _PromptCount = 10;

    private static _InspectPage _curPage = _InspectPage.Skills;
    private static int _indexPageList = 0;
    private static TimeSpan _timeOnSameTarget = TimeSpan.Zero;

    #endregion

    #region Setup Methods

    static _InspectLib() {
        // Add preinitialized actors
        _Actors.AddRange(_Equip, _Affinities, _Hp, _HpAmt, _Sp, _SpAmt,
            _PageItemList, _PageItemRightList, _DescHeader, _Desc);

        // todo hp/sp bars
        _AnimPrimActors.AddRange(Parellelograms.CoverLeft, /*_PageListBox, _UnitListBox,*/
            _PageDivL, _PageDivR, _MultP, _ModP, _OtherP);

        // Stat types
        for (int i = 0; i < _StatTypeCount; i++) {
            int x = 75 + (i * 675);

            _Actors.Add(_StatCategoryHeaders[i] = new Label(RenderPriority.B2High) { Position = new Vector2(x, 570) });

            const int Y = 622;

            _Actors.Add(_StatsPage[i] = new Label(RenderPriority.B2High) { Position = new Vector2(x, Y) });

            _Actors.Add(_StatsPageNum[i] = new Label(RenderPriority.B2High) {
                Position = new Vector2(x + 585, Y),
                Alignment = Alignment.TopRight
            });
        }

        // Page list
        for (int i = 0; i < _PageCount; i++) {
            _Actors.Add(_PageList[i] = new Label(RenderPriority.B2High) { Y = 480 });
        }

        // Basic stat list
        for (int i = 0; i < StatCount; i++) {
            int x = i > 2 ? 1440 : 945;
            int y = 165 - (45 * (i % 3));

            _Actors.Add(_StatsBasic[i] = new Label(RenderPriority.B2High) {
                Position = new Vector2(x, y),
                Alignment = Alignment.TopLeft
            });

            _Actors.Add(_StatsBasicNum[i] = new Label(RenderPriority.B2High) {
                Position = new Vector2(x + 450, y),
                Alignment = Alignment.TopRight
            });
        }

        // Input prompts
        // Stat, Equip, Affinity, Mult, Mod,
        // Other, LT, RT, L, R
        Vector2[] promptPos = [new(1290, 170), new(700, 245), new(300, 245), new(300, 385), new(750, 385),
            new(1125, 385), new(310, 52), new(0, 52), new(385, 320), new(857, 320)];

        for (int i = 0; i < _PromptCount; i++) {
            _Actors.Add(_Prompts[i] = new Label(RenderPriority.B2High) { Position = promptPos[i] });
        }

        // Assert.LenIs(_Actors, _ActorCount);
        // Assert.LenIs(_AnimPrimActors, _AnimPrimActorCount);

        _Translate();
    }

    /// <summary>
    /// Sets the text of menu elements
    /// </summary>
    // todo how often should this be called? currently just once, should also be re-called
    // on lang change and if unit names can change that part must be re-called on open
    // ontranslate event sent out to listeners?
    internal static void _Translate() {
        // Stat types
        string[] names = [Lang.InfoMult, Lang.InfoMod, Lang.InfoOther];
        for (int i = 0; i < _StatTypeCount; i++) _StatCategoryHeaders[i].Text = names[i];

        // Page list
        names = [Lang.Skills, Lang.Passives, Lang.Buffs, Lang.Stats];

        _pageTabs = new(_menu, new Vector2(638, 446), names) {
            Priority = RenderPriority.B2Med
        };

        // int[] divs = new int[4];
        // int divTotal = 0;

        for (int i = 0; i < _PageCount; i++) {
            _PageList[i].Text = names[i];
            // _PageList[i].X = 655 * divTotal;

            // int div = _PageList[i].Size.X + 20;
            // divs[i] = div;
            // divTotal += div;
        }

        //_PageListBox.Divisions = divs;

        // Basic stat list
        names = [Lang.StatStr, Lang.StatMag, Lang.StatFth, Lang.StatAmr, Lang.StatRes, Lang.StatAgi];
        for (int i = 0; i < StatCount; i++) _StatsBasic[i].Text = names[i];

        // Current unit items
        _Hp.Text = Lang.Hp;
        _Sp.Text = Lang.Sp;
    }

    internal static void _LateInit() {
        // todo lighter name color? account for non-8 units?
        // todo unify for nameplates
        Unit[] u = BattleLib.Battle.GetAllUnits();
        for (int i = 0; i < UnitCount; i++) _UnitList[i] = u[i].FormatName(false);
        // todo set their X here

        _unitTabs = new TabBarWidget(_menu, new Vector2(518, 40), _UnitList) {
            Priority = RenderPriority.B2Med
        };

        _menu = new Menu.Menu([.. _AnimPrimActors, .. _Actors, _pageTabs, _unitTabs]);
    }

    internal static void _Create() {
        States.Inspect.Menus.Add(_menu);

        Stage.Cleanup();
    }

    internal static void _Destroy() {
        States.Inspect.RemoveMenu();
    }

    #endregion

    #region Update Methods

    internal static void _Update(GameTime gameTime) {
        if (Parellelograms.CoverLeft.Prog == 1 && InputLib.Check(Keybinds.Back)) {
            StateMachine.Remove();
            return;
        }

        // todo

        // UpdateInputPrompt on page change
    }

    internal static string _GetInputPrompt() => _curPage == _InspectPage.Stats
        ? Menu.State.State.GetInputPromptString(Faster, Back)
        : Menu.State.State.GetInputPromptString(ScrollUpDown, Faster, Back);

    private static void _HandleInspectPage() { }

    private static void _SetStatVisibility(bool isStatsPage) { }

    private static void _SetPageItemVisibility(bool visible) { }

    private static void _DeleteInspect() { }

    #endregion
}
