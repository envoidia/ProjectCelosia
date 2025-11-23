using System;
using System.Collections.Generic;
using System.Text;
using API.Battle.BuffEffects;
using API.Battle.SkillEffects;
using API.Extensions;
using API.Graphics;
using API.Input;
using API.Menu;
using API.Save;
using Microsoft.Xna.Framework;

namespace API.Battle;

// todo finish + cleanup + reduce visibility when possible
public static class BattleHandler {
    public static Battle Battle { get; private set; } // todo

    private const int TeamCount = 2;
    private const int StatTypeCount = 3;
    private const int TeamSize = 4;
    private const int StatCount = 6;
    private const int UnitCount = 8;

    #region Display Fields

    private static readonly Label Turn = new(Core.StageBattle, $"{Colors.Turn}{Lang.Turn} 1") {
        Alignment = Alignment.Center,
        Position = new Vector2(World.W2, 90)
    };

    private static readonly Label Queue = new(Core.StageBattle) {
        Alignment = Alignment.Center,
        Position = new Vector2(World.W2, 180)
    };

    private static readonly Label SkillsL = new(Core.StageBattle);

    private static readonly Label[] BloomLabels = new Label[TeamCount];

    // Per-unit Labels
    private static readonly Label[] StatsL = new Label[UnitCount];
    private static readonly Label[] BuffsL = new Label[UnitCount];
    private static readonly Label[] Moves = new Label[UnitCount];
    private static readonly Label[] UnitNames = new Label[UnitCount];

    #endregion

    #region Inspect Fields

    // todo
    private static int indexPage = 0;
    private static int indexPageList = 0;

    private enum InspectPage {
        Skills,
        Passives,
        Buffs,
        Stats
    }

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
        InputPrompts.InspectStat, InputPrompts.InspectAffinity,
        InputPrompts.InspectEquip, InputPrompts.InspectMult, InputPrompts.InspectMod, InputPrompts.InspectOther,
        InputPrompts.InspectUnitL, InputPrompts.InspectUnitR,
        InputPrompts.InspectPageL, InputPrompts.InspectPageR
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

    #region Log Fields

    private static readonly Label BattleLog = new(Core.StageBattle) { Position = new Vector2(World.W2 - 300, 405) };

    private static readonly List<string> LogText = new(1024); // todo decide capacity

    // Amount of lines scrolled upwards
    private static int logScroll = 0;

    #endregion

    #region Logic Fields

    private static readonly List<Move> CurMoves = new(128); // todo decide capacity

    internal static TimeSpan delay;

    // How many extra actions have been used for the currently acting Unit
    private static int extraActions = 0;

    // Pos of the Unit that's currently selecting their move. 100 = moves are executing
    private static int selectingMove = 0;
    private const int ExecutionPhase = 100;

    // Index of the currently-applying SkillEffect of the current Move
    private static int applyingEffect = 0;

    // Previous SkillEffect resultTypes for each pos
    private static ResultType[] prevResults = new ResultType[8];

    // Amount of non-fail results for the current Move so far
    private static int nonFails = 0;

    private static SkillInstance selectedSkillInstance;

    // Menu navigation
    private static int indexSkill = 0;
    private static int indexTarget = 0;
    private static float secondsOnSameTarget = 0;

    #endregion

    #region Setup Methods

    public static void Initialize() {
        // Setup Labels
        for (int i = 0; i < TeamCount; i++) {
            // todo midgame translation
            BloomLabels[i] = new Label(Core.StageBattle) {
                Position = new Vector2(i == 1 ? World.W - 105 : 105, 135),
                Alignment = i == 1 ? Alignment.TopRight : Alignment.TopLeft
            };
        }

        // Per-unit Labels
        for (int i = 0; i < UnitCount; i++) {
            int y = i >= 4 ? 450 + (450 * (i - 4)) : 450 + (450 * i);

            StatsL[i] = new Label(Core.StageBattle) { Position = new Vector2(i >= 4 ? World.W - 350 : 75, y) };
            BuffsL[i] = new Label(Core.StageBattle) { Position = new Vector2(i >= 4 ? World.W - 525 : 75, y - 95) };
            Moves[i] = new Label(Core.StageBattle) { Position = new Vector2(i >= 4 ? World.W - 825 : 600, y) };
            UnitNames[i] = new Label(Core.StageBattle) { Y = 52 };
        }

        // Todo inspect init

        // Sort stages
        Core.StageBattle.Sort();
        Core.StageInspect.Sort();
        Core.StageInspect.IsVisible = false;
    }

    public static void StartBattle() {
        // temp setup teams
        // todo remove pos from unit constructor, assign in battle constructor?
        Battle = new Battle(new Team(new Unit(UnitTypes.TestUnitType, 19, null, 0, Skills.Nothing, Skills.Defend),
            new Unit(UnitTypes.TestUnitType, 19, null, 1, Skills.Nothing, Skills.Defend),
            new Unit(UnitTypes.TestUnitType, 19, null, 2, Skills.Nothing, Skills.Defend),
            new Unit(UnitTypes.TestUnitType, 19, null, 3, Skills.Nothing, Skills.Defend)),
            new Team(new Unit(UnitTypes.TestUnitType, 19, null, 4, Skills.Nothing, Skills.Defend),
                new Unit(UnitTypes.TestUnitType, 19, null, 5, Skills.Nothing, Skills.Defend),
                new Unit(UnitTypes.TestUnitType, 19, null, 6, Skills.Nothing, Skills.Defend),
                new Unit(UnitTypes.TestUnitType, 19, null, 7, Skills.Nothing, Skills.Defend)));

        UpdateStatDisplay(0);

        Core.StageBattle.IsVisible = true;
    }

    public static void EndBattle() {
        Core.StageBattle.IsVisible = false;
    }

    #endregion

    #region Update Methods
    public static void Input(GameTime gameTime) {
        HandleDebug();

        MenuType curMenu = Core.NavPath.Peek();

        if (curMenu == MenuType.Log) {
            HandleLog();
        } else if (curMenu == MenuType.Inspect) {
            HandleInspect();
        } else if (Core.Input.CheckInput(Keybinds.Menu)) {
            CreateLog();
        } else if (curMenu == MenuType.InspectTargeting) {
            HandleInspectTargeting();
        } else if (curMenu == MenuType.Targeting) {
            HandleTargeting();
        } else if (Core.Input.CheckInput(Keybinds.Map)) {
            CreateInspectTargeting();
        } else if (delay > TimeSpan.Zero) {
            delay -= gameTime.ElapsedGameTime;
        } else if (curMenu == MenuType.Battle) HandleBattle();
    }

    public static void HandleDebug() {
        if (Core.Input.CheckInput(Keybinds.DebugDumpLog)) {
            Console.WriteLine(string.Join('\n', LogText));
        }
    }

    // Updates bloom labels, queue, and Unit nameplates
    private static void UpdateStatDisplay(int curPos) {
        // Update bloom labels
        for (int i = 0; i < TeamCount; i++) {
            // todo fix it getting confused by the /
            BloomLabels[i].Text =
                $"{Colors.Stat}{Lang.Bloom}{Colors.White}: {Colors.Bloom}{Battle.GetTeamBySide((Side) i).Bloom}{Colors.White}//{Colors.Bloom}1,000";
        }

        Unit[] units = Battle.GetAllUnits();
        StringBuilder sb = new();

        // Update nameplates
        for (int i = 0; i < units.Length; i++) {
            // Stat display
            StatsL[i].Text = $"{units[i].FormatName(false)}\nHP: {units[i].Hp}{(units[i].Shield > 0 ? $"{units[i].Shield.Format(Colors.Shield, false)}{Colors.White}" : "")}//{units[i].GetBaseStat(Stats.Hp)}\nSP: {(units[i].IsBoolStat(BoolStats.InfiniteSp) ? '∞' : $"{units[i].Sp.Format(false)}//{1000.Format(false)}")}";

            // Buff display
            int buffCount = 0;

            // List stage changes
            foreach (StageType stageType in Core.StageTypes) {
                int stage = units[i].GetStage(stageType);
                if (stage != 0) {
                    if (buffCount > 0 && buffCount % 4 == 0) sb.Append('\n');

                    buffCount++;

                    sb.Append(stageType.Icon).Append(Colors.White).Append((stage >= 1) ? '+' : "").Append(stage)
                    .Append('(').Append(units[i].GetStageTurns(stageType)).Append(") ");
                }
            }

            // List buffs
            List<BuffInstance> buffInstances = units[i].BuffInstances;

            foreach (BuffInstance buffInstance in buffInstances) {
                if (buffCount > 0 && buffCount % 4 == 0) sb.Append('\n');

                buffCount++;

                if (buffInstance.Buff == Buffs.Defend) {
                    sb.Append(buffInstance.Buff.Icon).Append("[WHITE]").Append('x')
                            .Append(units[i].Defend.Format()).Append('(')
                            .Append(buffInstance.Turns).Append(") ");
                } else if (buffInstance.Buff == Buffs.Shield) {
                    sb.Append(buffInstance.Buff.Icon).Append("[WHITE]").Append('x')
                            .Append(units[i].Shield.Format()).Append('(')
                            .Append(buffInstance.Turns).Append(") ");
                } else {
                    sb.Append(buffInstance.Buff.Icon).Append("[WHITE]");
                    if (buffInstance.Buff.MaxStacks > 1) {
                        sb.Append('x').Append(buffInstance.Stacks);
                    }

                    // 1000+ turns = infinite
                    sb.Append('(');
                    if (buffInstance.Turns < 1000) sb.Append(buffInstance.Turns);
                    else sb.Append('∞');
                    sb.Append(") ");
                }

                BuffsL[i].Text = sb.ToString();
            }


        }

        // Update queue
        sb = new StringBuilder();
        units.Sort((a, b) => a.GetStat(Stats.Agi).CompareTo(b.GetStat(Stats.Agi)));

        for (int i = 0; i < units.Length; i++) {
            bool active = units[i].Pos == curPos;

            if (active) sb.Append('<');

            sb.Append(units[i].FormatName(false));

            if (active) sb.Append(Colors.White).Append('>');

            if (i != units.Length - 1) sb.Append(", ");
        }

        Queue.Text = sb.ToString();


    }
    #endregion

    #region Log Methods

    // todo
    internal static void CreateLog() {
        Core.AddMenu(MenuType.Log);
    }

    internal static void HandleLog() {
        if (Core.Input.CheckInput(Keybinds.Back, Keybinds.Menu)) {
            Core.RemoveMenu();
        }
    }

    private static void UpdateLog() {
        BattleLog.Text = FormatLog();

        // todo full log
    }

    private static string FormatLog() {
        /* todo
        int lines = 8;
        int scroll = 0;

        if(Core.NavPath.Peek() == MenuType.Log) {
            lines = 48;
            scroll = logScroll;
        }

        int start = Math.Max(0, LogText.Count - lines - scroll);
        int end = Math.Min(start + lines, LogText.Count);*/
        return string.Join("\n", LogText);
    }

    // todo limit size
    public static void AppendToLog(params List<string> str) {
        LogText.AddRange(str);
        logScroll = 0;
        UpdateLog();
    }

    public static void AppendToLog(string[] str) {
        LogText.AddRange(str);
        logScroll = 0;
        UpdateLog();
    }

    #endregion

    private static void HandleTargeting() {
        if (Core.Input.CheckInput(Keybinds.Back)) {
            //foreach (Label stat in stats) stat.Color = Colors.White;
            Moves[selectingMove].Text = "";

            Core.RemoveMenu();
            return;
        }

        indexTarget = MenuLib.CheckMovementTargeting(indexTarget, selectingMove, selectedSkillInstance.Skill.Range);

        //MenuLib.handleOptColor(stats, indexTarget);

        if (!Core.Input.CheckInput(Keybinds.Confirm)) return;

        Unit self = Battle.PlayerTeam.Units[selectingMove];
        Unit target = indexTarget < 4
            ? Battle.PlayerTeam.Units[indexTarget]
            : Battle.OpponentTeam.Units[indexTarget - 4];
        CurMoves.Add(new Move(selectedSkillInstance, self, target.Pos));
        // todo support ExA
        Moves[selectingMove].Text = $"{Moves[selectingMove].Text} → {target.FormatName(false)}";

        foreach (Label stat in StatsL) {
            //stat.Color = Colors.White;
        }

        // Move on to next Unit unless this one has extra actions
        if (extraActions < self.ExtraActions) {
            extraActions++;
        } else {
            extraActions = 0;
            selectingMove++;
        }

        indexSkill = 0;

        UpdateStatDisplay(selectingMove);

        Core.NavPath.Pop();
    }

    #region Inspect Methods

    private static void CreateInspectTargeting() {
        Core.AddMenu(MenuType.InspectTargeting);
    }

    private static void HandleInspectTargeting() {
        if (Core.Input.CheckInput(Keybinds.Back)) {
            Core.RemoveMenu();
            return;
        }

        if (Core.Input.CheckInput(Keybinds.Confirm, Keybinds.Map)) {
            CreateInspect();
        }
    }

    private static void CreateInspect() {
        Core.RemoveMenu();
        Core.AddMenu(MenuType.Inspect);
        Core.StageInspect.IsVisible = true;
    }

    private static void HandleInspect() {
        if (Core.Input.CheckInput(Keybinds.Back)) {
            Core.RemoveMenu();
            Core.StageInspect.IsVisible = false;
            return;
        }
    }

    private static void HandleInspectPage() { }

    private static void SetStatVisibility(bool isStatsPage) { }

    private static void SetPageItemVisibility(bool visible) { }

    private static void DeleteInspect() { }

    #endregion

    #region Move Selection Methods

    private static void HandleBattle() {
        switch (selectingMove) {
            case < 4: SelectPlayerMove(); return;
            case < 8: SelectOpponentMove(); return;
            default: ExecuteMove(); return;
        }
    }

    private static void SelectPlayerMove() {
        if (selectingMove >= Battle.PlayerTeam.Units.Length) return;

        SkillsL.Position = new Vector2(600, World.H - 400 - (250 * selectingMove));
        // todo support ExA
        // todo do not assign every frame
        SkillsL.Text =
            // todo list all skills
            $"{Battle.PlayerTeam.Units[selectingMove].SkillInstances[0].Skill
                // temp
                // todo real skill description display
                .GetName()}({Colors.Cooldown}{Battle.PlayerTeam.Units[selectingMove].SkillInstances[0].Cooldown}{Colors.White})";

        SelectMove();
    }

    private static void SelectOpponentMove() {
        if (Debug.SelectOpponentMoves) {
            SelectMove();
            return;
        }

        if ((selectingMove - 4) > Battle.OpponentTeam.Units.Length) {
            selectingMove = ExecutionPhase;
            return;
        }

        // temp
        Skill selectedSkill = Skills.Nothing;
        // todo AI
        Unit target = Battle.PlayerTeam.Units[0];
        // todo support ExA
        Moves[selectingMove].Text = $"{selectedSkill.GetName()} → {target.FormatName(false)}";
        CurMoves.Add(new Move(new SkillInstance(selectedSkill), Battle.OpponentTeam.Units[selectingMove - 4], target.Pos));
        selectingMove++;
    }

    private static void SelectMove() {
        // Cancel
        if (Core.Input.CheckInput(Keybinds.Back) && (selectingMove != 0)) {
            // todo
            SkillsL.Text = "";

            selectingMove--;
            indexSkill = 0;
            Moves[selectingMove].Text = "";

            for (int i = 0; i <= Battle.PlayerTeam.Units[selectingMove].ExtraActions; i++) CurMoves.RemoveLast();

            return;
        }

        indexSkill = MenuLib.CheckMovement1D(indexSkill, 6); // todo

        //MenuLib.handleOptColor(skills, indexSkill); todo

        if (!Core.Input.CheckInput(Keybinds.Confirm)) return;

        selectedSkillInstance = Battle.PlayerTeam.Units[selectingMove].SkillInstances[indexSkill];
        Moves[selectingMove].Text = selectedSkillInstance.Skill.GetName();

        // todo
        SkillsL.Text = "";

        indexTarget = selectedSkillInstance.Skill.GetStartingIndex();
        Core.NavPath.Push(MenuType.Targeting);
    }

    private static void ExecuteMove() {
        if (CurMoves.Count == 0) {
            EndTurn();
            return;
        }

        // Sort moves
        // todo test
        CurMoves.Sort((a, b) => {
            // Sort by Prio
            int prioComparison = b.SkillInstance.Skill.Prio.CompareTo(a.SkillInstance.Skill.Prio);
            if (prioComparison != 0) return prioComparison;

            // Sort by Agi
            int agiComparison = b.Self.GetStat(Stats.Agi).CompareTo(a.Self.GetStat(Stats.Agi));
            if (agiComparison != 0) return agiComparison;

            // Sort by Pos
            return a.Self.Pos.CompareTo(b.Self.Pos);
        });

        Move move = CurMoves[0];
        Unit self = move.Self;

        if (self.IsBoolStat(BoolStats.UnableToAct)) {
            AppendToLog(Lang.LogSkillFailUnableToAct, move.GetTriesToUseString(),
            Lang.LogButIsUnableToAct, self.GetBoolStat(BoolStats.UnableToAct).ToString()); // todo test
            EndMove();
            return;
        }

        int cd = move.SkillInstance.Cooldown;
        if (cd > 0 && applyingEffect == 0) {
            AppendToLog(Lang.LogSkillFailCooldown.FormatLang(move.GetTriesToUseString(),
                Lang.LogButItsOnCooldown.FormatIcu(cd)));
            EndMove();
            return;
        }

        if (!move.IsInRange()) {
            AppendToLog(Lang.LogSkillFailRange.FormatLang(move.GetTriesToUseString(), Lang.LogButCantReach));
            EndMove();
            return;
        }

        // SP after move executes. Invalid spNew will cancel move
        int spNew = 0;

        Skill skill = move.SkillInstance.Skill;

        if (applyingEffect == 0) {
            move.SkillInstance.Cooldown = cd;

            Element element = skill.GetElement();
            bool isPlayerTeam = self.Pos < 4;
            Team team = isPlayerTeam ? Battle.PlayerTeam : Battle.OpponentTeam;
            int cost = self.IsBoolStat(BoolStats.InfiniteSp) && !skill.IsBloom ? 0 : skill.Cost;

            // Make sure cost doesn't go below 1 unless the skill has a base 0 SP cost
            int costMod =
                cost > 0 ? (int) Math.Max(cost * (AffLib.SpCost[self.GetAffinity(element)] / 1000d), 1) : 0;

            int change = (int) (skill.IsBloom ? costMod : costMod * self.GetMult(Mults.SpUse));
            spNew = skill.IsBloom ? team.Bloom - change : self.Sp - change;

            if (spNew < 0) {
                string msg = "LogSkillFailSp".FormatLang(move.GetTriesToUseString(),
                    "LogButDoesntHaveEnough".FormatIcu(skill.IsBloom.ToInt()));
                AppendToLog(msg);
            } else {
                Unit target = Battle.GetUnitAtPos(move.TargetPos);

                bool isBloom = skill.IsBloom;
                int spOld = isBloom ? team.Bloom : self.Sp;
                change *= -1;
                string changeSp = "";

                if (spOld != spNew) {
                    changeSp = "LogSkillUseChangeSpBloom".FormatIcu(isBloom.ToInt(), spOld, spNew, change);
                }

                if (isBloom) {
                    team.Bloom = spNew;
                } else {
                    self.Sp = spNew;
                }

                AppendToLog("LogSkillUse".FormatIcu(self.FormatName(false),
                    skill.GetName(Colors.Skill),
                    target.FormatName(false),
                    skill.IsRangeSelf().ToInt().ToString(), changeSp));

                self.OnUseSkill(target, skill);

                // Color move for currently acting combatant (temp)
                for (int i = 0; i < 8; i++) {
                    //moves[i].Color = (self.Pos == i) ? Color.Pink : Color.White;
                }

                prevResults = new ResultType[8];
            }
        }

        SkillEffect[] skillEffects = skill.SkillEffects;

        Unit targetMain = Battle.GetUnitAtPos(move.TargetPos);

        // The check for reaching skillEffects.length will only apply here if the length is 0, because otherwise it'll
        // be applied at the end
        if (spNew < 0 || applyingEffect == skillEffects.Length || (nonFails == 0 && applyingEffect > 0)) {
            EndMove();
            UpdateStatDisplay(self.Pos);
            return;
        }

        foreach (int targetPos in skill.Range.GetTargetPositions(self.Pos, move.TargetPos)) {
            if (targetPos == PosLib.InvalidPos) continue;

            Unit targetCur = Battle.GetUnitAtPos(targetPos);
            if (applyingEffect == 0) {
                prevResults[targetPos] = ResultType.Success;

                targetCur.OnTargetedBySkill(self, skill);
            }

            if (prevResults[targetPos] == ResultType.Fail) continue;

            nonFails++;

            ResultType resultType = skillEffects[applyingEffect]
                .Apply(self, targetCur, targetCur == targetMain, prevResults[targetPos]);
            prevResults[targetPos] = resultType;
        }

        if (!skillEffects[applyingEffect].IsInstant) {
            delay += TimeSpan.FromSeconds(0.25f * Settings.BattleSpeed);
        }

        applyingEffect++;

        UpdateStatDisplay(self.Pos);

        if (skillEffects.Length != applyingEffect) return;
        EndMove();
        move.SkillInstance.Cooldown = move.SkillInstance.Skill.Cooldown;
        // todo delete killed units
    }

    private static void EndMove() {
        applyingEffect = 0;
        nonFails = 0;
        CurMoves.RemoveFirst();
        delay += TimeSpan.FromSeconds(1) * Settings.BattleSpeed;
    }

    private static void EndTurn() {
        selectingMove = 0;
        Battle.Turn++;

        Turn.Text = $"{Colors.Turn}{Lang.Turn} {Battle.Turn + 1}";

        for (int i = 0; i < UnitCount; i++) {
            Moves[i].Text = "";
            //moves[i].Color = Colors.White;
        }

        foreach (Unit unit in Battle.GetAllUnits()) {
            unit.Sp = (int) Math.Min(unit.Sp + (100 * unit.GetMult(Mults.SpGain)), 1000);

            foreach (Passive passive in unit.Passives) {
                StringBuilder turnEnd1 =
                    new StringBuilder(Lang.LogTurnEndEffect.FormatLang(unit.FormatName(),
                        Colors.Passive + passive.GetName())).Append(' ');

                foreach (IBuffEffect buffEffect in passive.BuffEffects) {
                    StringBuilder turnEnd2 = new();
                    string[] effectMsgs = buffEffect.OnTurnEnd(unit, 1);

                    foreach (string effectMsg in effectMsgs) {
                        if (!string.IsNullOrEmpty(effectMsg)) {
                            turnEnd2.Append(effectMsg);
                        }
                    }

                    if (turnEnd2.Length > 0) AppendToLog(turnEnd1 + turnEnd2.ToString());
                }
            }

            foreach (BuffInstance buffInstance in unit.BuffInstances) {
                StringBuilder turnEnd1 =
                    new StringBuilder(Lang.LogTurnEndEffect.FormatLang(unit.FormatName(), buffInstance.Buff.GetName()))
                        .Append(' ');

                foreach (IBuffEffect buffEffect in buffInstance.Buff.BuffEffects) {
                    StringBuilder turnEnd2 = new();
                    string[] effectMsgs = buffEffect.OnTurnEnd(unit, buffInstance.Stacks);
                    foreach (string effectMsg in effectMsgs) {
                        if (!string.IsNullOrEmpty(effectMsg)) turnEnd2.Append(effectMsg);
                    }

                    if (turnEnd2.Length > 0) AppendToLog(turnEnd1 + turnEnd2.ToString());
                }
            }

            // Decrement stage/shield/buff turns and remove expired stages/shields/buffs
            unit.DecrementTurns();
        }

        // todo is trailing white needed
        AppendToLog($"{Colors.Turn}{Lang.Turn} {Battle.Turn + 1}{Colors.White}");
        AppendToLog(Lang.LogGainSpBloom);

        // Increase bloom
        Battle.PlayerTeam.Bloom = Math.Min(Battle.PlayerTeam.Bloom + 100, 1000);
        Battle.OpponentTeam.Bloom = Math.Min(Battle.OpponentTeam.Bloom + 100, 1000);

        UpdateStatDisplay(0);
    }

    #endregion
}