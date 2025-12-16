using API.Graphics;
using API.Input;
using API.Menu.State;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using API.Menu;
using API.Extensions;
using API.Util;

using static API.Input.InputPrompts;
using static API.Battle.State.BattleLib;
using System.Text;

namespace API.Battle.State;

// todo cleanup
internal sealed class _InspectLib {
    #region Display Fields

    private const int _ActorCount = 49; // todo
    private static readonly List<IActor> _Actors = new(_ActorCount);

    private const int _AnimPrimActorCount = 6; //todo;
    private static readonly List<IActor> _AnimPrimActors = new(_AnimPrimActorCount);

    internal static readonly Menu.Menu _Menu = new("Inspect") {
        OnCreate = static () => {
            _Queue.CheckInput = true;

            // Set selected unit in queue to current
            _Queue.Index = _GetQueueIndex(_indexTarget);
            _UpdateInspectUnitPage(_Queue.Index);
        },

        OnDestroy = static () => {
            _Queue.CheckInput = false;

            // Set selected unit in queue to what it was
            _Queue.Index = _GetQueueIndex(_GetQueuePos());
        },

        OnUpdate = _Update,

        InputWidgets = [_Queue]
    };

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
    private static readonly TabBarWidget _PageTabs = new(new Vector2(1135, 600), 8) {
        Priority = RenderPriority.B2Med
    };
    // private static readonly GuiBoxChain _PageListBox = new(638, 446, 501) { Priority = RenderPriority.B2Med };

    // Unit tabs
    private static readonly string[] _UnitList = new string[UnitCount];

    //private static TabBarWidget _unitTabs = null!;
    //private static readonly GuiBoxChain _UnitListBox = new(518, 40, 106) { Priority = RenderPriority.B2Med };

    // Input prompts
    private static readonly Label[] _Prompts = new Label[10];
    private static readonly InputPrompt[] _PromptTypes = [
            InspectStat, InspectAffinity,
            InspectEquip, InspectMult, InspectMod, InspectOther,
            InspectUnitL, InspectUnitR,
            InspectPageL, InspectPageR];

    // todo Dividing paths
    private const int _Y = 800;
    private static readonly Path _PageDivL = new(new(30, _Y), new(370, _Y),
        RenderPriority.B2Med) { Speed = IAnimated.DefaultSpeed };
    private static readonly Path _PageDivR = new(new(900, _Y), new(1450, _Y),
        RenderPriority.B2Med) { Speed = IAnimated.DefaultSpeed };

    private static readonly Path _MultP = new(new(60, _Y), new(660, _Y),
        RenderPriority.B2Med) { Speed = IAnimated.DefaultSpeed };
    private static readonly Path _ModP = new(new(60 + 675, _Y), new(660 + 675, _Y),
        RenderPriority.B2Med) { Speed = IAnimated.DefaultSpeed };
    private static readonly Path _OtherP = new(new(60 + 1350, _Y), new(660 + 1350, _Y),
        RenderPriority.B2Med) { Speed = IAnimated.DefaultSpeed };

    // Current unit items
    private static readonly RectangleActor _UnitBounds = new() {
        Position = new Vector2(30, 30),
        Size = new Point(384),
        Priority = RenderPriority.B2Med
    };

    private const int _StatStartX = 450;
    private const int _StatGapX = 595;
    private const int _StatBarWidth = _StatGapX - 50;
    private const int _StatStartY = 175;
    private const int _StatGapY = 65;

    private static readonly Label _Lvl = new(RenderPriority.B2Med) {
        Position = new Vector2(_StatStartX, _StatStartY)
    };

    private static readonly HpBarWidget _Hp = new(new Vector2(_StatStartX, _StatStartY + _StatGapY),
        _StatBarWidth, RenderPriority.B2Med);
    // private static readonly Label _Hp = new(RenderPriority.B2Med) {
    //     Text = "HP",
    //     Position = new Vector2(_StatStartX, _StatStartY + _StatGapY)
    // };
    // private static readonly Label _HpAmt = new(RenderPriority.B2Med) {
    //     Position = new Vector2(900, _StatStartY + _StatGapY),
    //     Alignment = Alignment.TopRight
    // };
    //private static GuiBoxBar hpBar = coolRectBars[CoolRectBars.HP_INSPECT.ordinal()]; todo

    private static readonly StatBarWidget _Sp = new(new Vector2(_StatStartX, _StatStartY + (_StatGapY * 2)),
            _StatBarWidth, RenderPriority.B2Med, "SP") {
        ColorLayer0 = Colors.Pink,
        ColorLayer1 = Colors.LightPurple,
        MaxVal = 1000
    };
    // private static readonly Label _Sp = new(RenderPriority.B2Med) {
    //     Text = "SP",
    //     Position = new Vector2(_StatStartX, _StatStartY + (_StatGapY * 2))
    // };
    // private static readonly Label _SpAmt = new(RenderPriority.B2Med) {
    //     Position = new Vector2(900, _StatStartY + (_StatGapY * 2)),
    //     Alignment = Alignment.TopRight
    // };

    private static readonly Label _Equip = new(RenderPriority.B2Med) {
        Position = new Vector2(_StatStartX, _StatStartY + (_StatGapY * 3))
    };
    private static readonly Label _Affinities = new(RenderPriority.B2Med) {
        Position = new Vector2(_StatStartX, _StatStartY + (_StatGapY * 4))
    };

    /// <summary>
    /// List of basic stats
    /// </summary>
    private static readonly Stat[] _StatList = [Stats.Str, Stats.Mag, Stats.Fth, Stats.Amr, Stats.Res, Stats.Agi];

    /// <inheritdoc cref="_StatList"/>
    private static readonly StatBarWidget[] _StatsBasic = new StatBarWidget[StatCount];
    //private static readonly Label[] _StatsBasic = new Label[StatCount];

    /// <inheritdoc cref="_StatsBasic" />
    //private static readonly Label[] _StatsBasicNum = new Label[StatCount];
    //private static GuiBoxBar spBar = coolRectBars[CoolRectBars.SP_INSPECT.ordinal()];

    // Current page items
    private static readonly Label _PageItemList = new(RenderPriority.B2Med);
    private static readonly Label _PageItemRightList = new(RenderPriority.B2Med);
    private static readonly Label _DescHeader = new(RenderPriority.B2Med);
    private static readonly Label _Desc = new(RenderPriority.B2Med);

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
        _Actors.AddRange(_UnitBounds, _Equip, _Affinities, _Lvl, _Hp, _Sp,
            _PageItemList, _PageItemRightList, _DescHeader, _Desc);

        // todo hp/sp bars
        _AnimPrimActors.AddRange(Parellelograms.CoverLeft/*, _PageListBox, _UnitListBox,*/
            /*_PageDivL, _PageDivR, _MultP, _ModP, _OtherP*/);

        // Stat types
        for (int i = 0; i < _StatTypeCount; i++) {
            int x = 75 + (i * 675);

            _Actors.Add(_StatCategoryHeaders[i] = new Label(RenderPriority.B2Med) { Position = new Vector2(x, 650) });

            const int Y = 702;

            _Actors.Add(_StatsPage[i] = new Label(RenderPriority.B2Med) { Position = new Vector2(x, Y) });

            _Actors.Add(_StatsPageNum[i] = new Label(RenderPriority.B2Med) {
                Position = new Vector2(x + 585, Y),
                Alignment = Alignment.TopRight
            });
        }

        // Basic stat list
        for (int i = 0; i < StatCount; i++) {
            int x = _StatStartX + _StatGapX * (i > 2 ? 2 : 1);
            int y = _StatStartY + (_StatGapY * (i % 3));

            _Actors.Add(_StatsBasic[i] = new StatBarWidget(new Vector2(x, y), _StatBarWidth, RenderPriority.B2Med));
            // _Actors.Add(_StatsBasic[i] = new Label(RenderPriority.B2Med) {
            //     Position = new Vector2(x, y),
            //     Alignment = Alignment.TopLeft
            // });

            // _Actors.Add(_StatsBasicNum[i] = new Label(RenderPriority.B2Med) {
            //     Position = new Vector2(x + 550, y),
            //     Alignment = Alignment.TopRight
            // });
        }

        // Input prompts
        // Stat, Equip, Affinity, Mult, Mod,
        // Other, LT, RT, L, R
        Vector2[] promptPos = [new(1290, 170), new(700, 245), new(300, 245), new(300, 385), new(750, 385),
            new(1125, 385), new(310, 52), new(0, 52), new(385, 320), new(857, 320)];

        for (int i = 0; i < _PromptCount; i++) {
            //todo _Actors.Add(_Prompts[i] = new Label(RenderPriority.B2Med) { Position = promptPos[i] });
        }

        // todo
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
        _PageTabs.SetText([Lang.Skills, Lang.Passives, Lang.Buffs, Lang.Stats]);

        // Basic stat list
        for (int i = 0; i < StatCount; i++) _StatsBasic[i].Title.Text = _StatList[i].GetName();
    }

    internal static void _LateInit() {
        // todo account for non-8 units?
        // todo unify for nameplates
        Unit[] u = BattleLib.Battle.GetAllUnits();
        for (int i = 0; i < UnitCount; i++) _UnitList[i] = u[i].FormatName(false);
        // todo set their X here

        _Menu.Setup([.. _AnimPrimActors, .. _Actors, _PageTabs]);
    }

    // todo remove
    internal static void _Create() {
        States.Battle.AddMenu(_Menu);
    }

    internal static void _Destroy() => States.Inspect.RemoveMenu();

    #endregion

    #region Update Methods

    internal static void _Update(GameTime gameTime) {
        if (Parellelograms.CoverLeft.Prog == 1 && InputLib.Check(Keybinds.Back)) {
            //StateMachine.Remove();
            States.Battle.RemoveMenu();
            return;
        }

        if (InputLib.IsKeyJustPressed(Microsoft.Xna.Framework.Input.Keys.Q)) {
            foreach (Unit u in BattleLib.Battle.GetAllUnits()) {
                u.SetStatMult(Stats.Agi, u.GetStatMult(Stats.Agi) + 80);
                u.Shield += 500;
            }

            _UpdateInspectUnitPage(_Queue.Index);
        }

        if (InputLib.IsKeyJustPressed(Microsoft.Xna.Framework.Input.Keys.W)) {
            foreach (Unit u in BattleLib.Battle.GetAllUnits()) {
                u.Hp += 500;
            }

            _UpdateInspectUnitPage(_Queue.Index);
        }

        // todo

        // UpdateInputPrompt on page change
    }

    internal static string _GetInputPrompt() => _curPage == _InspectPage.Stats
        ? Menu.State.State.GetInputPromptString(Faster, Jump, Back)
        : Menu.State.State.GetInputPromptString(ScrollUpDown, Faster, Jump, Back);

    internal static void _UpdateInspectUnitPage(int index) {
        Unit u = _GetUnitsSortedByAgi()[index];

        _Lvl.Text = $"Lvl {ColorCode.Num}{u.Lvl + 1}";

        // HP and SP
        // todo hp bar
        // todo account for infinite sp
        _Hp.Hp = u.Hp;
        _Hp.MaxHp = u.GetBaseStat(Stats.Hp);
        _Hp.Shield = u.Shield;

        _Sp.Val = u.Sp;
        //_HpAmt.Text = $"{u.Hp.Format(ColorCode.White, false)}//{u.GetBaseStat(Stats.Hp)
        //    .Format(ColorCode.White, false)}";
        //_SpAmt.Text = $"{u.Sp.Format(ColorCode.White, false)}/2,000";

        _Equip.Text = u.GetEquipString();
        _Affinities.Text = u.GetAffinitiesString(true);

        // Basic stats
        for (int i = 0; i < StatCount; i++) {
            int curStat = u.GetStat(_StatList[i]);
            int baseStat = u.GetBaseStat(_StatList[i]);
            // _StatsBasicNum[i].Text = $"{u.GetStat(_StatList[i]).Format(false)}//{baseStat
            //     .Format(false)}";

            _StatsBasic[i].Val = curStat;
            _StatsBasic[i].MaxVal = baseStat;
        }
    }

    private static void _HandleInspectPage() { }

    private static void _SetStatVisibility(bool isStatsPage) { }

    private static void _SetPageItemVisibility(bool visible) { }

    private static void _DeleteInspect() { }

    #endregion
}
