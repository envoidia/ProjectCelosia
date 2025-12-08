using API.Graphics;
using API.Input;
using API.Menu.State;
using Microsoft.Xna.Framework;
using static API.Input.InputPrompts;
using static API.Battle.State.BattleLib;
using System;
using System.Collections.Generic;
using API.Util;

namespace API.Battle.State;

public sealed class InspectLib {

    #region Display Fields

    // todo ensure this size is right
    private static readonly List<Actor> _Actors = new(55);
    private static readonly List<Actor> _AnimPrimActors = new(8);

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
    private static readonly GuiBoxChain _PageListBox = new(638, 446, 501, Priority.VeryHigh);

    // Basic stat list
    /// <summary>
    /// List of basic stats
    /// </summary>
    private static readonly Label[] _StatsBasic = new Label[StatCount];

    /// <inheritdoc cref="_StatsBasic" />
    private static readonly Label[] _StatsBasicNum = new Label[StatCount];

    // Unit list
    private static readonly Label[] _UnitList = new Label[UnitCount];
    private static readonly GuiBoxChain _UnitListBox = new(518, 40, 106, Priority.VeryHigh);

    // Input prompts
    private static readonly Label[] _Prompts = new Label[10];
    private static readonly InputPrompt[] _PromptTypes = [
            InspectStat, InspectAffinity,
            InspectEquip, InspectMult, InspectMod, InspectOther,
            InspectUnitL, InspectUnitR,
            InspectPageL, InspectPageR];

    // Dividing paths
    private const int _Y = 600;
    private static readonly Path _PageDivL = new(new(30, _Y), new(370, _Y), Priority.ExtremelyHigh);
    private static readonly Path _PageDivR = new(new(900, _Y), new(1450, _Y), Priority.ExtremelyHigh);

    private static readonly Path _MultP = new(new(60, _Y), new(660, _Y), Priority.ExtremelyHigh);
    private static readonly Path _ModP = new(new(60 + 675, _Y), new(660 + 675, _Y), Priority.ExtremelyHigh);
    private static readonly Path _OtherP = new(new(60 + 1350, _Y), new(660 + 1350, _Y), Priority.ExtremelyHigh);

    // Current unit items
    private static readonly Label _Equip = new(Priority.ExtremelyHigh) { Position = new Vector2(450, 320) };
    private static readonly Label _Affinities = new(Priority.ExtremelyHigh) { Position = new Vector2(1050, 320) };

    private static readonly Label _Hp = new(Priority.ExtremelyHigh) { Position = new Vector2(450, 165) };
    private static readonly Label _HpAmt = new(Priority.ExtremelyHigh) {
        Position = new Vector2(900, 165),
        Alignment = Alignment.TopRight
    };
    //private static GuiBoxBar hpBar = coolRectBars[CoolRectBars.HP_INSPECT.ordinal()]; todo

    private static readonly Label _Sp = new(Priority.ExtremelyHigh) { Position = new Vector2(450, 210) };
    private static readonly Label _SpAmt = new(Priority.ExtremelyHigh) {
        Position = new Vector2(450, 210),
        Alignment = Alignment.TopRight
    };
    //private static GuiBoxBar spBar = coolRectBars[CoolRectBars.SP_INSPECT.ordinal()];

    // Current page items
    private static readonly Label _PageItemList = new(Priority.ExtremelyHigh);
    private static readonly Label _PageItemRightList = new(Priority.ExtremelyHigh);
    private static readonly Label _DescHeader = new(Priority.ExtremelyHigh);
    private static readonly Label _Desc = new(Priority.ExtremelyHigh);

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

    public static void Initialize() {
        // Add preinitialized actors
        _Actors.AddRange(_Equip, _Affinities, _Hp, _HpAmt, _Sp, _SpAmt,
            _PageItemList, _PageItemRightList, _DescHeader, _Desc);

        // todo hp/sp bars
        _AnimPrimActors.AddRange(GuiBoxes.CoverLeft, _PageListBox, _UnitListBox,
            _PageDivL, _PageDivR, _MultP, _ModP, _OtherP);

        // Stat types
        for (int i = 0; i < _StatTypeCount; i++) {
            int x = 75 + (i * 675);

            _Actors.Add(_StatCategoryHeaders[i] = new Label(Priority.ExtremelyHigh) { Position = new Vector2(x, 570) });

            const int Y = 622;

            _Actors.Add(_StatsPage[i] = new Label(Priority.ExtremelyHigh) { Position = new Vector2(x, Y) });

            _Actors.Add(_StatsPageNum[i] = new Label(Priority.ExtremelyHigh) {
                Position = new Vector2(x + 585, Y),
                Alignment = Alignment.TopRight
            });
        }

        // Page list
        for (int i = 0; i < _PageCount; i++) {
            _Actors.Add(_PageList[i] = new Label(Priority.ExtremelyHigh) { Y = 480 });
        }

        // Basic stat list
        for (int i = 0; i < StatCount; i++) {
            int x = i > 2 ? 1440 : 945;
            int y = 165 - (45 * (i % 3));

            _Actors.Add(_StatsBasic[i] = new Label(Priority.ExtremelyHigh) {
                Position = new Vector2(x, y),
                Alignment = Alignment.TopLeft
            });

            _Actors.Add(_StatsBasicNum[i] = new Label(Priority.ExtremelyHigh) {
                Position = new Vector2(x + 450, y),
                Alignment = Alignment.TopRight
            });
        }

        // Unit list
        for (int i = 0; i < UnitCount; i++) {
            _Actors.Add(_UnitList[i] = new Label(Priority.ExtremelyHigh) { Y = 52 });
        }

        // Input prompts
        // Stat, Equip, Affinity, Mult, Mod,
        // Other, LT, RT, L, R
        Vector2[] promptPos = [new(1290, 170), new(700, 245), new(300, 245), new(300, 385), new(750, 385),
            new(1125, 385), new(310, 52), new(0, 52), new(385, 320), new(857, 320)];

        for (int i = 0; i < _PromptCount; i++) {
            _Actors.Add(_Prompts[i] = new Label(Priority.ExtremelyHigh) { Position = promptPos[i] });
        }

        Translate();
    }

    /// <summary>
    /// Sets the text of menu elements
    /// </summary>
    // todo how often should this be called? currently just once, should also be re-called
    // on lang change and if unit names can change that part must be re-called on open
    // ontranslate event sent out to listeners?
    public static void Translate() {
        // Stat types
        string[] names = [Lang.InfoMult, Lang.InfoMod, Lang.InfoOther];
        for (int i = 0; i < _StatTypeCount; i++) _StatCategoryHeaders[i].Text = names[i];

        // Page list
        names = [Lang.Skills, Lang.Passives, Lang.Buffs, Lang.Stats];

        int[] divs = new int[4];
        int divTotal = 0;

        for (int i = 0; i < _PageCount; i++) {
            _PageList[i].Text = names[i];
            _PageList[i].X = 655 * divTotal;

            int div = _PageList[i].Size.X + 20;
            divs[i] = div;
            divTotal += div;
        }

        _PageListBox.Divisions = divs;

        // Basic stat list
        names = [Lang.StatStr, Lang.StatMag, Lang.StatFth, Lang.StatAmr, Lang.StatRes, Lang.StatAgi];
        for (int i = 0; i < StatCount; i++) _StatsBasic[i].Text = names[i];

        // Current unit items
        _Hp.Text = Lang.Hp;
        _Sp.Text = Lang.Sp;
    }

    // todo unify for nameplates
    public static void TranslateUnitNames() {
        // todo lighter name color? account for non-8 units?
        Unit[] u = BattleLib.Battle.GetAllUnits();
        for (int i = 0; i < UnitCount; i++) _UnitList[i].Text = u[i].FormatName(false);

        // todo set their X here

    }

    public static void Create() {
        Stage.AddRange(_AnimPrimActors);
        foreach (Actor a in _AnimPrimActors) a.AddRoutine(IAnimatedPrimitive.In);

        Stage.AddRange(_Actors);

        Stage.Cleanup();
    }

    public static void Destroy() {
        foreach (Actor a in _Actors) a.MarkForRemoval();
        foreach (Actor a in _AnimPrimActors) a.AddRoutine(IAnimatedPrimitive.Out);

        Stage.Cleanup();
    }

    #endregion

    #region Update Methods

    public static void Update(GameTime gameTime) {
        if (InputLib.Check(Keybinds.Back)) {
            StateMachine.Remove();
            return;
        }

        // todo

        // UpdateInputPrompt on page change
    }

    public static string GetInputPrompt() => _curPage == _InspectPage.Stats
        ? Menu.State.State.GetInputPromptString(ScrollFaster, Back)
        : Menu.State.State.GetInputPromptString(ScrollUpDown, ScrollFaster, Back);

    private static void _HandleInspectPage() { }

    private static void _SetStatVisibility(bool isStatsPage) { }

    private static void _SetPageItemVisibility(bool visible) { }

    private static void _DeleteInspect() { }

    #endregion
}
