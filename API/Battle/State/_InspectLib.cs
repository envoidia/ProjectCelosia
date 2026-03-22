using API.Graphics;
using API.Input;
using API.Menu.State;
using Microsoft.Xna.Framework;

using static API.Input.InputPrompts;
using static API.Battle.State.BattleLib;
using System.Linq;
using API.Extensions;
using API.Name;
using API.Util;
using API.Menu.Widget;
using System;
using Microsoft.Xna.Framework.Input;

namespace API.Battle.State;

// todo cleanup
internal sealed class _InspectLib
{
    #region Display Fields

    private const int _ActorCount = 51; // todo
    private static readonly SizedArr<IActor> _Actors = new(_ActorCount);

    private const int _AnimPrimActorCount = 1; //6 todo; //todo merge;
    private static readonly SizedArr<IActor> _AnimPrimActors = new(_AnimPrimActorCount);

    internal static readonly Menu.Menu _Menu = new("Inspect")
    {
        OnCreate = static () =>
        {
            _Queue.CheckInput = true;

            // Set selected unit in queue to current
            _Queue.Index = _GetQueueIndex(_indexTarget);
            _UpdateInspectUnitPage(_Queue.Index);

            _UpdateInspectPage(_PageTabs!.Index);
        },

        OnDestroy = static () =>
        {
            _Queue.CheckInput = false;

            // Set selected unit in queue to what it was
            _Queue.Index = _GetQueueIndex(_GetQueuePos());
        },

        OnUpdate = _Update,

        GetInputPrompt = static () => Menu.State.State.GetInputPromptString(ScrollUpDown,
            ChangePage, ChangeUnit, Back),

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
    private static readonly TabBarWidget _PageTabs = new(new(1135, 600), 8)
    {
        Priority = RenderPriority.B3Med,
        OnChangeIndex = _UpdateInspectPage
    };

    // Items on current page
    private static readonly ListWidget _PageItems = new(new(60, 740), true,
        16, RenderPriority.B3Med)
    {
        FixedWidth = 800,
        OnChangeIndex = static i => _UpdatePageItemDesc(i, _PageTabs.Index)
    };

    private static readonly ASlantedLine _PageDivL = new(new(35, 590),
        new(635, 20), RenderPriority.B3Med);
    private static readonly ASlantedLine _PageDivR = new(new(1600, 590),
        new(635, 20), RenderPriority.B3Med);
    // private static readonly GuiBoxChain _PageListBox = new(638, 446, 501) { Priority = RenderPriority.B2Med };

    // Current unit items
    private static readonly ARectangle _UnitBounds = new(ThemeColor.TransBlack)
    {
        Position = new(40, 95),
        Size = new(RenderLib.UnitSpriteSize),
        Priority = RenderPriority.B3Med,
        OutlineColor = ThemeColor.White
    };

    private const int _StatStartX = 450;
    private const int _StatGapX = 595;
    private const int _StatBarWidth = _StatGapX - 50;
    private const int _StatStartY = 175;
    private const int _StatGapY = 65;

    private static readonly Label _Lvl = new(RenderPriority.B3Med)
    {
        Position = new(_StatStartX, _StatStartY)
    };

    private static readonly HpBarWidget _Hp = new(new(_StatStartX, _StatStartY + _StatGapY),
        _StatBarWidth, RenderPriority.B3Med);
    // private static readonly Label _Hp = new(RenderPriority.B2Med) {
    //     Text = "HP",
    //     Position = new(_StatStartX, _StatStartY + _StatGapY)
    // };
    // private static readonly Label _HpAmt = new(RenderPriority.B2Med) {
    //     Position = new(900, _StatStartY + _StatGapY),
    //     Alignment = Alignment.TopRight
    // };
    //private static GuiBoxBar hpBar = coolRectBars[CoolRectBars.HP_INSPECT.ordinal()]; todo

    private static readonly StatBarWidget _Sp = new(pos: new(_StatStartX, _StatStartY + (_StatGapY * 2)),
            _StatBarWidth, RenderPriority.B3Med, ThemeColor.Stat.Str + "StatSp".GetLang())
    {
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

    private static readonly Label _Equip = new(RenderPriority.B3Med)
    {
        Position = new(_StatStartX, _StatStartY + (_StatGapY * 3))
    };

    private static readonly Label _Affinities = new(RenderPriority.B3Med)
    {
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
    // private static readonly Label _DescHeader = new(RenderPriority.B2Med);
    private static readonly Label _Desc = new(RenderPriority.B3Med)
    {
        Position = new(950, 740),
        MaxWidth = 1150
    };

    #endregion

    #region Logic Fields

    // todo
    private enum _InspectPage
    {
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

    internal static void _Init()
    {
        // Add preinitialized actors
        _Actors.AddRange(_UnitBounds, _Equip, _Affinities, _Lvl, _Hp, _Sp,
            /*_DescHeader,*/ _Desc, _PageDivL, _PageDivR);

        // todo hp/sp bars
        _AnimPrimActors.AddRange(Parellelograms.CoverLeft/*, _PageListBox, _UnitListBox,*/
            /*_PageDivL, _PageDivR, _MultP, _ModP, _OtherP*/);

        // Stat types
        for (int i = 0; i < _StatTypeCount; i++)
        {
            int x = 75 + (i * 675);

            _Actors.Add(_StatCategoryHeaders[i] = new Label(RenderPriority.B2Med)
            {
                Position = new(x, 720),
            });

            const int Y = 702;

            _Actors.Add(_StatsPage[i] = new Label(RenderPriority.B2Med)
            {
                Position = new(x, Y)
            });

            _Actors.Add(_StatsPageNum[i] = new Label(RenderPriority.B2Med)
            {
                Position = new(x + 585, Y),
                Alignment = Alignment.TopRight
            });
        }

        // Basic stat list
        for (int i = 0; i < StatCount; i++)
        {
            int x = _StatStartX + _StatGapX * (i > 2 ? 2 : 1);
            int y = _StatStartY + (_StatGapY * (i % 3));

            _Actors.Add(_StatsBasic[i] = new StatBarWidget(new(x, y), _StatBarWidth,
                RenderPriority.B3Med));
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
        //ReadOnlySpan<Vector2> promptPos = [new(1290, 170), new(700, 245), new(300, 245), new(300, 385), new(750, 385),
        //   new(1125, 385), new(310, 52), new(0, 52), new(385, 320), new(857, 320)];

        for (int i = 0; i < _PromptCount; i++)
        {
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
    internal static void _Translate()
    {
        // Stat types
        ReadOnlySpan<string> names = ["InfoMult", "InfoMod", "InfoOther"];

        for (int i = 0; i < _StatTypeCount; i++)
        {
            _StatCategoryHeaders[i].Text = names[i].GetLang();
        }

        // Page list
        _PageTabs.SetText(["Skills".GetLang(), "Passives".GetLang(), "Buffs".GetLang(), "Stats".GetLang()]);

        // Basic stat list
        for (int i = 0; i < StatCount; i++)
        {
            _StatsBasic[i].Title.Text = _StatList[i].GetName();
        }
    }

    internal static void _LateInit()
    {
        // todo account for non-8 units?
        // todo unify for nameplates
        // Unit[] u = BattleLib.Battle.GetAllUnits();

        // for (int i = 0; i < UnitCount; i++)
        // {
        //     _UnitList[i] = u[i].FormatName(false);
        // }

        // todo set their X here

        _Menu.Setup([.. _AnimPrimActors, .. _Actors, _PageTabs, _PageItems]);
    }

    // todo remove
    internal static void _Create()
    {
        States.Battle.AddMenu(_Menu);
    }

    #endregion

    #region Update Methods

    internal static void _Update(GameTime gt)
    {
        // todo better solution (snap to 0/1 when opening/closing) (or at least buffer inputs)
        if (Parellelograms.CoverLeft.Prog == 1 && InputLib.Check(Keybinds.Back))
        {
            //StateMachine.Remove();
            States.Battle.RemoveMenu();
            return;
        }

        if (InputLib.IsKeyJustPressed(Keys.Q))
        {
            foreach (Unit u in BattleLib.Battle.GetAllUnits())
            {
                u.SetStatMult(Stats.Agi, u.GetStatMult(Stats.Agi) + 240);
                u.Shield += 500;
            }

            _UpdateInspectUnitPage(_Queue.Index);
        }

        if (InputLib.IsKeyJustPressed(Keys.W))
        {
            foreach (Unit u in BattleLib.Battle.GetAllUnits())
            {
                u.Hp += 500;
            }

            _UpdateInspectUnitPage(_Queue.Index);
        }
        // todo

        // UpdateInputPrompt on page change
    }

    internal static string _GetInputPrompt()
    {
        return _curPage == _InspectPage.Stats
        ? Menu.State.State.GetInputPromptString(Back)
        : Menu.State.State.GetInputPromptString(ScrollUpDown, Back);
    }

    // todo fix some shit that fails to change
    internal static void _UpdateInspectUnitPage(int index)
    {
        Unit u = _GetUnitsSortedByAgi()[index];

        _Lvl.Text = $"Lvl {ThemeColor.Imp.Str}{u.Lvl + 1}";

        // HP and SP
        // todo account for infinite sp
        _Hp.Hp = u.Hp;
        _Hp.MaxHp = u.GetBaseStat(Stats.Hp);
        _Hp.Shield = u.Shield;

        _Sp.Val = u.Sp;

        _Equip.Text = u.GetEquipString();
        _Affinities.Text = u.GetAffinitiesString(true);

        // Basic stats
        for (int i = 0; i < StatCount; i++)
        {
            int curStat = u.GetStat(_StatList[i]);
            int baseStat = u.GetBaseStat(_StatList[i]);

            _StatsBasic[i].Val = curStat;
            _StatsBasic[i].MaxVal = baseStat;
        }

        _UpdateInspectPage(_PageTabs.Index);
    }

    private static void _UpdateInspectPage(int index)
    {
        Unit u = _GetUnitsSortedByAgi()[_Queue.Index];

        _SetStatVisibility((_InspectPage) index == _InspectPage.Stats);

        switch ((_InspectPage) index)
        {
            case _InspectPage.Skills:
                _PageItems.SetTextL([.. u.SkillInstances.Select(s
                    => s.Skill.GetName(ThemeColor.White))]);

                _PageItems.SetTextR([.. u.SkillInstances.Select(s
                    => s.GetCostCdFormatted(u))]);

                _PageItems.CalcLayout();

                _UpdatePageItemDesc(_PageItems.Index, index);

                return;

            case _InspectPage.Passives:
                _PageItems.SetTextR(); // todo

                _PageItems.SetTextL([.. u.Passives.Select(s
                    => s.GetName(ThemeColor.White))]);

                _PageItems.CalcLayout();

                _UpdatePageItemDesc(_PageItems.Index, index);

                return;

            case _InspectPage.Buffs:
                _PageItems.SetTextL([.. u.BuffInstances.Select(b
                    => b.Buff.GetName(ThemeColor.White))]);

                _PageItems.SetTextR([.. u.BuffInstances.Select(b
                    => b.GetTurnsStacksFormatted())]);

                _PageItems.CalcLayout();

                _UpdatePageItemDesc(_PageItems.Index, index);

                return;

            case _InspectPage.Stats:
                _PageItems.SetTextL();
                _PageItems.SetTextR();

                _PageItems.CalcLayout();

                _Desc.Text = "";

                return;
        }
    }

    private static void _UpdatePageItemDesc(int index, int inspectPageIndex)
    {
        if (_PageItems.OptCount == 0)
        {
            _Desc.Text = "";
            return;
        }

        Unit u = _GetUnitsSortedByAgi()[_Queue.Index];

        ComplexDescribable? cd = (_InspectPage) inspectPageIndex switch
        {
            _InspectPage.Skills => u.SkillInstances[index].Skill,
            _InspectPage.Passives => u.Passives[index],
            _InspectPage.Buffs => u.BuffInstances[index].Buff,
            _InspectPage.Stats => null,
            _ => throw new ClosedEnumsWhenException()
        };

        if (cd is null)
        {
            _Desc.Text = "";
            return;
        }

        _Desc.Text = $"{cd.GetName()}{ThemeColor.White.Str}\n\n{cd.GetFullDesc()}";
    }

    private static void _SetStatVisibility(bool visible)
    {
        foreach (Label l in _StatCategoryHeaders)
        {
            l.IsVisible = visible;
        }
    }

    #endregion
}
