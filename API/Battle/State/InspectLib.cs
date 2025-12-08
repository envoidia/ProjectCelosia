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
    private static readonly GuiBoxChain _PageListBox = new(Stages.Inspect, 638, 446, 501);

    // Basic stat list
    /// <summary>
    /// List of basic stats
    /// </summary>
    private static readonly Label[] _StatsBasic = new Label[StatCount];

    /// <inheritdoc cref="_StatsBasic" />
    private static readonly Label[] _StatsBasicNum = new Label[StatCount];

    // Unit list
    private static readonly Label[] _UnitList = new Label[UnitCount];
    private static readonly GuiBoxChain _UnitListBox = new(Stages.Inspect, 518, 40, 106);

    // Input prompts
    private static readonly Label[] _Prompts = new Label[10];
    private static readonly InputPrompt[] _PromptTypes = [
            InspectStat, InspectAffinity,
            InspectEquip, InspectMult, InspectMod, InspectOther,
            InspectUnitL, InspectUnitR,
            InspectPageL, InspectPageR];

    // Dividing paths
    private const int _Y = 600;
    private static readonly Path _PageDivL = new(Stages.Inspect, new(30, _Y), new(370, _Y));
    private static readonly Path _PageDivR = new(Stages.Inspect, new(900, _Y), new(1450, _Y));

    private static readonly Path _MultP = new(Stages.Inspect, new(60, _Y), new(660, _Y));
    private static readonly Path _ModP = new(Stages.Inspect, new(60 + 675, _Y), new(660 + 675, _Y));
    private static readonly Path _OtherP = new(Stages.Inspect, new(60 + 1350, _Y), new(660 + 1350, _Y));

    // Current unit items
    private static readonly Label _Equip = new(Stages.Inspect) { Position = new Vector2(450, 320) };
    private static readonly Label _Affinities = new(Stages.Inspect) { Position = new Vector2(1050, 320) };

    private static readonly Label _Hp = new(Stages.Inspect) { Position = new Vector2(450, 165) };
    private static readonly Label _Test = new(Stages.Inspect) { Position = new Vector2(450, 165) };
    private static readonly Label _HpAmt = new(Stages.Inspect) {
        Position = new Vector2(900, 165),
        Alignment = Alignment.TopRight
    };
    //private static GuiBoxBar hpBar = coolRectBars[CoolRectBars.HP_INSPECT.ordinal()]; todo

    private static readonly Label _Sp = new(Stages.Inspect) { Position = new Vector2(450, 210) };
    private static readonly Label _SpAmt = new(Stages.Inspect) {
        Position = new Vector2(450, 210),
        Alignment = Alignment.TopRight
    };
    //private static GuiBoxBar spBar = coolRectBars[CoolRectBars.SP_INSPECT.ordinal()];

    // Current page items
    private static readonly Label _PageItemList = new(Stages.Inspect);
    private static readonly Label _PageItemRightList = new(Stages.Inspect);
    private static readonly Label _DescHeader = new(Stages.Inspect);
    private static readonly Label _Desc = new(Stages.Inspect);

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

    // test
    public static Label inspectSt = new(Stages.Inspect) { Position = new(1000, 1000) };

    #endregion

    #region Setup Methods

    public static void Initialize() {
        // Stat types
        for (int i = 0; i < _StatTypeCount; i++) {
            int x = 75 + (i * 675);

            _StatCategoryHeaders[i] = new Label(Stages.Inspect) { Position = new Vector2(x, 570) };

            const int Y = 622;

            _StatsPage[i] = new Label(Stages.Inspect) { Position = new Vector2(x, Y) };

            _StatsPageNum[i] = new Label(Stages.Inspect) {
                Position = new Vector2(x + 585, Y),
                Alignment = Alignment.TopRight
            };
        }

        // Page list
        int[] divs = new int[4];
        int divTotal = 0;
        for (int i = 0; i < _PageCount; i++) {
            _PageList[i] = new Label(Stages.Inspect) { Position = new Vector2(655 * divTotal, 480) };

            int div = _PageList[i].Size.X + 20;
            divs[i] = div;
            divTotal += div;
        }

        _PageListBox.Divisions = divs;

        // Basic stat list
        for (int i = 0; i < StatCount; i++) {
            int x = i > 2 ? 1440 : 945;
            int y = 165 - (45 * (i % 3));

            _StatsBasic[i] = new Label(Stages.Inspect) {
                Position = new Vector2(x, y),
                Alignment = Alignment.TopLeft
            };

            _StatsBasicNum[i] = new Label(Stages.Inspect) {
                Position = new Vector2(x + 450, y),
                Alignment = Alignment.TopRight
            };
        }

        // Unit list
        for (int i = 0; i < UnitCount; i++) {
            _UnitList[i] = new Label(Stages.Battle) { Y = 52 };
        }

        // Input prompts
        // Stat, Equip, Affinity, Mult, Mod,
        // Other, LT, RT, L, R
        Vector2[] promptPos = [new(1290, 170), new(700, 245), new(300, 245), new(300, 385), new(750, 385),
            new(1125, 385), new(310, 52), new(0, 52), new(385, 320), new(857, 320)];

        for (int i = 0; i < _PromptCount; i++) {
            _Prompts[i] = new Label(Stages.Inspect) { Position = promptPos[i] };
        }

        Stages.Inspect.Sort();
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
        for (int i = 0; i < _PageCount; i++) _PageList[i].Text = names[i];

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

    #endregion

    #region Update Methods

    public static void Update(GameTime gameTime) {
        HandleDebug();

        if (InputLib.Check(Keybinds.Back)) {
            NavPath.Remove();
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
