using API.Graphics;
using API.Input;
using API.Menu.State;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using API.Menu;

using static API.Input.InputPrompts;
using static API.Battle.State.BattleLib;
using System.Linq;
using API.Extensions;

namespace API.Battle.State;

// todo cleanup
internal sealed class _InspectLib {
    #region Display Fields

    private const int _ActorCount = 51; // todo
    private static readonly List<IActor> _Actors = new(_ActorCount);

    private const int _AnimPrimActorCount = 6; //todo merge;
    private static readonly List<IActor> _AnimPrimActors = new(_AnimPrimActorCount);

    internal static readonly Menu.Menu _Menu = new("Inspect") {
        OnCreate = static () => {
            _Queue.CheckInput = true;

            // Set selected unit in queue to current
            _Queue.Index = _GetQueueIndex(_indexTarget);
            _UpdateInspectUnitPage(_Queue.Index);

            _UpdateInspectPage(_PageTabs!.Index);
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
    private static readonly TabBarWidget _PageTabs = new(new(1135, 600), 8) {
        Priority = RenderPriority.B2Med,
        OnSelect = _UpdateInspectPage
    };

    // Items on current page
    private static readonly ListRightWidget _PageItems = new(new(60, 740), 16) {
        ItemPadding = new(40, 20, 10, 10),
        FixedWidth = 800
    };

    private static readonly LineActor _PageDivL = new(new(35, 590), new(635, 20));
    private static readonly LineActor _PageDivR = new(new(1600, 590), new(635, 20));
    // private static readonly GuiBoxChain _PageListBox = new(638, 446, 501) { Priority = RenderPriority.B2Med };

    // Unit tabs
    private static readonly string[] _UnitList = new string[UnitCount];

    //private static TabBarWidget _unitTabs = null!;
    //private static readonly GuiBoxChain _UnitListBox = new(518, 40, 106) { Priority = RenderPriority.B2Med };

    // Input prompts
    // todo remove
    private static readonly Label[] _Prompts = new Label[10];
    private static readonly InputPrompt[] _PromptTypes = [
            InspectStat, InspectAffinity,
            InspectEquip, InspectMult, InspectMod, InspectOther,
            InspectUnitL, InspectUnitR,
            InspectPageL, InspectPageR];

    // todo Dividing paths
    private const int _Y = 800;
    // private static readonly Path _PageDivL = new(new(30, _Y), new(370, _Y),
    //     RenderPriority.B2Med) { Speed = IAnimated.DefaultSpeed };
    // private static readonly Path _PageDivR = new(new(900, _Y), new(1450, _Y),
    //     RenderPriority.B2Med) { Speed = IAnimated.DefaultSpeed };

    // private static readonly Path _MultP = new(new(60, _Y), new(660, _Y),
    //     RenderPriority.B2Med) { Speed = IAnimated.DefaultSpeed };
    // private static readonly Path _ModP = new(new(60 + 675, _Y), new(660 + 675, _Y),
    //     RenderPriority.B2Med) { Speed = IAnimated.DefaultSpeed };
    // private static readonly Path _OtherP = new(new(60 + 1350, _Y), new(660 + 1350, _Y),
    //     RenderPriority.B2Med) { Speed = IAnimated.DefaultSpeed };

    // Current unit items
    private static readonly RectangleActor _UnitBounds = new() {
        Position = new(30, 30),
        Size = new(384),
        Priority = RenderPriority.B2Med
    };

    private const int _StatStartX = 450;
    private const int _StatGapX = 595;
    private const int _StatBarWidth = _StatGapX - 50;
    private const int _StatStartY = 175;
    private const int _StatGapY = 65;

    private static readonly Label _Lvl = new(RenderPriority.B2Med) {
        Position = new(_StatStartX, _StatStartY)
    };

    private static readonly HpBarWidget _Hp = new(new(_StatStartX, _StatStartY + _StatGapY),
        _StatBarWidth, RenderPriority.B2Med);
    // private static readonly Label _Hp = new(RenderPriority.B2Med) {
    //     Text = "HP",
    //     Position = new(_StatStartX, _StatStartY + _StatGapY)
    // };
    // private static readonly Label _HpAmt = new(RenderPriority.B2Med) {
    //     Position = new(900, _StatStartY + _StatGapY),
    //     Alignment = Alignment.TopRight
    // };
    //private static GuiBoxBar hpBar = coolRectBars[CoolRectBars.HP_INSPECT.ordinal()]; todo

    private static readonly StatBarWidget _Sp = new(new(_StatStartX, _StatStartY + (_StatGapY * 2)),
            _StatBarWidth, RenderPriority.B2Med, ThemeColor.Stat.Str() + "StatSp".GetLang()) {
        ColorLayer0 = ThemeColor.SpBack,
        ColorLayer1 = ThemeColor.Sp,
        MaxVal = 1000
    };
    // private static readonly Label _Sp = new(RenderPriority.B2Med) {
    //     Text = "SP",
    //     Position = new(_StatStartX, _StatStartY + (_StatGapY * 2))
    // };
    // private static readonly Label _SpAmt = new(RenderPriority.B2Med) {
    //     Position = new(900, _StatStartY + (_StatGapY * 2)),
    //     Alignment = Alignment.TopRight
    // };

    private static readonly Label _Equip = new(RenderPriority.B2Med) {
        Position = new(_StatStartX, _StatStartY + (_StatGapY * 3))
    };
    private static readonly Label _Affinities = new(RenderPriority.B2Med) {
        Position = new(_StatStartX, _StatStartY + (_StatGapY * 4))
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
    // todo remove
    private static readonly Label _PageItemList = new(RenderPriority.B2Med);
    private static readonly Label _PageItemRightList = new(RenderPriority.B2Med);
    private static readonly Label _DescHeader = new(RenderPriority.B2Med);
    private static readonly Label _Desc = new(RenderPriority.B2Med) {
        Position = new(950, 740)
    };

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
    //private static int _indexPageList = 0;
    //private static TimeSpan _timeOnSameTarget = TimeSpan.Zero;

    #endregion

    #region Setup Methods

    static _InspectLib() {
        // Add preinitialized actors
        _Actors.AddRange(_UnitBounds, _Equip, _Affinities, _Lvl, _Hp, _Sp,
            _PageItemList, _PageItemRightList, _DescHeader, _Desc, _PageDivL, _PageDivR);

        // todo hp/sp bars
        _AnimPrimActors.AddRange(Parellelograms.CoverLeft/*, _PageListBox, _UnitListBox,*/
            /*_PageDivL, _PageDivR, _MultP, _ModP, _OtherP*/);

        // Stat types
        for (int i = 0; i < _StatTypeCount; i++) {
            int x = 75 + (i * 675);

            _Actors.Add(_StatCategoryHeaders[i] = new Label(RenderPriority.B2Med) {
                Position = new(x, 720),
            });

            const int Y = 702;

            _Actors.Add(_StatsPage[i] = new Label(RenderPriority.B2Med) { Position = new(x, Y) });

            _Actors.Add(_StatsPageNum[i] = new Label(RenderPriority.B2Med) {
                Position = new(x + 585, Y),
                Alignment = Alignment.TopRight
            });
        }

        // Basic stat list
        for (int i = 0; i < StatCount; i++) {
            int x = _StatStartX + _StatGapX * (i > 2 ? 2 : 1);
            int y = _StatStartY + (_StatGapY * (i % 3));

            _Actors.Add(_StatsBasic[i] = new StatBarWidget(new(x, y), _StatBarWidth, RenderPriority.B2Med));
            // _Actors.Add(_StatsBasic[i] = new Label(RenderPriority.B2Med) {
            //     Position = new(x, y),
            //     Alignment = Alignment.TopLeft
            // });

            // _Actors.Add(_StatsBasicNum[i] = new Label(RenderPriority.B2Med) {
            //     Position = new(x + 550, y),
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

        // Initial translate and subscribe to event
        _Translate();
        Lang.Language.OnChange += _Translate;
    }

    /// <summary>
    /// Sets the text of menu elements
    /// </summary>
    // todo if unit names can change that part must be re-called on open
    internal static void _Translate() {
        // Stat types
        string[] names = ["InfoMult", "InfoMod", "InfoOther"];
        for (int i = 0; i < _StatTypeCount; i++) _StatCategoryHeaders[i].Text = names[i].GetLang();

        // Page list
        _PageTabs.SetText(["Skills".GetLang(), "Passives".GetLang(), "Buffs".GetLang(), "Stats".GetLang()]);

        // Basic stat list
        for (int i = 0; i < StatCount; i++) _StatsBasic[i].Title.Text = _StatList[i].GetName();
    }

    internal static void _LateInit() {
        // todo account for non-8 units?
        // todo unify for nameplates
        Unit[] u = BattleLib.Battle.GetAllUnits();
        for (int i = 0; i < UnitCount; i++) _UnitList[i] = u[i].FormatName(false);
        // todo set their X here

        _Menu.Setup([.. _AnimPrimActors, .. _Actors, _PageTabs, _PageItems]);
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
                u.SetStatMult(Stats.Agi, u.GetStatMult(Stats.Agi) + 240);
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

        _Lvl.Text = $"Lvl {ThemeColor.Imp.Str()}{u.Lvl + 1}";

        // HP and SP
        // todo account for infinite sp
        _Hp.Hp = u.Hp;
        _Hp.MaxHp = u.GetBaseStat(Stats.Hp);
        _Hp.Shield = u.Shield;

        _Sp.Val = u.Sp;

        _Equip.Text = u.GetEquipString();
        _Affinities.Text = u.GetAffinitiesString(true);

        // Basic stats
        for (int i = 0; i < StatCount; i++) {
            int curStat = u.GetStat(_StatList[i]);
            int baseStat = u.GetBaseStat(_StatList[i]);

            _StatsBasic[i].Val = curStat;
            _StatsBasic[i].MaxVal = baseStat;
        }

        _UpdateInspectPage(_PageTabs.Index);
    }

    private static void _UpdateInspectPage(int index) {
        Unit u = _GetUnitsSortedByAgi()[_Queue.Index];

        _SetStatVisibility((_InspectPage) index == _InspectPage.Stats);

        switch ((_InspectPage) index) {
            case _InspectPage.Skills:
                _PageItems.SetText([.. u.SkillInstances.Select(s => s.Skill.GetName(ThemeColor.White))]);
                _PageItems.SetRightText([.. u.SkillInstances.Select(s => s.GetCostCdFormatted())]);

                if (_PageItems.OptCount != 0) {
                    _Desc.Text = $"{u.SkillInstances[_PageItems.Index].Skill.GetName()}\n\n{u.SkillInstances[_PageItems.Index].Skill.GetFullDesc()}";
                }

                return;
            case _InspectPage.Passives:
                _PageItems.SetRightText(); // todo
                _PageItems.SetText([.. u.Passives.Select(s => s.GetName(ThemeColor.White))]);

                if (_PageItems.OptCount != 0) {
                    _Desc.Text = $"{u.Passives[_PageItems.Index].GetName()}\n\n{u.Passives[_PageItems.Index].GetFullDesc()}";
                }

                return;
            case _InspectPage.Buffs:
                _PageItems.SetText([.. u.BuffInstances.Select(b => b.Buff.GetName(ThemeColor.White))]);
                _PageItems.SetRightText([.. u.BuffInstances.Select(b => b.GetTurnsStacksFormatted())]);

                if (_PageItems.OptCount != 0) {
                    _Desc.Text = $"{u.BuffInstances[_PageItems.Index].Buff.GetName()}\n\n{u.BuffInstances[_PageItems.Index].Buff.GetFullDesc()}";
                }

                return;
            case _InspectPage.Stats:
                _PageItems.SetText();
                _PageItems.SetRightText();
                _Desc.Text = "";
                return;
        }
    }

    private static void _HandleInspectPage() { }

    private static void _SetStatVisibility(bool visible) {
        foreach (Label l in _StatCategoryHeaders) l.IsVisible = visible;
    }

    private static void _SetPageItemVisibility(bool visible) { }

    private static void _DeleteInspect() { }

    #endregion
}
