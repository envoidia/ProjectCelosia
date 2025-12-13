using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API.Battle.BuffEffects;
using API.Battle.SkillEffects;
using API.Extensions;
using API.Graphics;
using API.Input;
using API.Menu;
using API.Menu.State;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Battle.State;

// todo finish + cleanup + reduce visibility when possible + rename to BattleLib
public static class BattleLib {
    public static Battle Battle { get; private set; } = null!; // todo

    #region Constants

    /// <summary>
    /// Multiplier for all stats
    /// </summary>
    public const int StatMult = 10;

    public const int TeamCount = 2;
    public const int TeamSize = 4;
    public const int StatCount = 6;
    public const int UnitCount = TeamSize * TeamCount;

    #endregion

    #region Display Fields

    private const int _ActorCount = 29;
    private static readonly List<IActor> _Actors = new(_ActorCount);

    private const int _AnimPrimActorCount = 0; // todo
    //private static readonly List<Actor> _AnimPrimActors = new(_AnimPrimActorCount);

    private static readonly string[] _UnitList = new string[UnitCount];
    internal static readonly TabBarWidget _Queue = new(new Vector2(World.W2, 100), UnitCount) {
        CheckInput = false,
        Alignment = Alignment.Center,
        Priority = RenderPriority.B2Med
    };

    private static readonly Label[] _BloomLabels = new Label[TeamCount];

    // Per-unit Labels
    internal static readonly Label[] _Stats = new Label[UnitCount];
    private static readonly Label[] _Buffs = new Label[UnitCount];
    internal static readonly Label[] _Moves = new Label[UnitCount];

    private static readonly Label _SkillsL = new();

    #endregion

    #region Logic Fields

    internal static readonly List<Move> _CurMoves = new(16);

    /// <summary>
    /// Pos of the Unit that's currently selecting their move. <c>ExecutionPhase</c> = moves are executing
    /// </summary>
    internal static int _selectingMove = 0;
    internal const int _ExecutionPhase = 100;

    /// <summary>
    /// Pos of the Unit that's currently using their move. <c>SelectionPhase</c> = moves are being selected
    /// </summary>
    internal static int _usingMove = _SelectionPhase;
    internal const int _SelectionPhase = 200;

    internal static SkillInstance _selectedSkillInstance = null!; // todo

    // Menu navigation
    internal static int _indexSkill = 0;
    internal static int _indexTarget = 0;

    // todo replays
    // serialize each unit and then just store each move as (starting Pos of Self, index in Self's Skill list, target Pos)?
    // instead of serializing the units, just store short lookups for each component?

    #endregion

    #region Move Execution Fields

    /// <summary>
    /// Index of the currently-applying SkillEffect of the current Move
    /// </summary>
    private static int _applyingEffect = 0;

    /// <summary>
    /// Previous SkillEffect resultTypes for each pos
    /// </summary>
    private static ResultType[] _prevResults = new ResultType[UnitCount];

    /// <summary>
    /// Amount of non-fail results for the current Move so far
    /// </summary>
    private static int _nonFails = 0;

    /// <summary>
    /// Time until the next battle action can occur
    /// </summary>
    private static TimeSpan _delay;

    #endregion

    #region Setup Methods

    static BattleLib() {
        // Add preinitialized actors
        _Actors.AddRange(_Queue, LogLib._BattleLog, _SkillsL);

        // Setup Labels
        for (int i = 0; i < TeamCount; i++) {
            // todo midgame translation
            _Actors.Add(_BloomLabels[i] = new Label() {
                Position = new Vector2(i == 1 ? World.W - 105 : 105, 135),
                Alignment = i == 1 ? Alignment.TopRight : Alignment.TopLeft
            });
        }

        // Per-unit Labels
        for (int i = 0; i < UnitCount; i++) {
            int x1 = 75;
            int x2 = 600;
            int y = 450 + (450 * i);

            if (i >= PosLib.LowestOpp) {
                x1 = World.W - 500;
                x2 = World.W - 825;
                y = 450 + (450 * (i - PosLib.LowestOpp));
            }

            _Actors.Add(_Stats[i] = new Label() { Position = new Vector2(x1, y) });
            _Actors.Add(_Buffs[i] = new Label() { Position = new Vector2(x1, y + 150) });
            _Actors.Add(_Moves[i] = new Label() { Position = new Vector2(x2, y + 50) });
        }

        Assert.LenIs(_Actors, _ActorCount);
    }

    internal static void _Create() {
        // temp setup teams
        Battle = Core.battle;

        // Setup unit names for queue
        // todo account for non-8 units?
        // todo unify for nameplates
        Unit[] u = Battle.GetAllUnits();
        for (int i = 0; i < UnitCount; i++) _UnitList[i] = u[i].FormatName(false);

        _Queue.SetText(_UnitList);

        _InspectLib._LateInit();

        LogLib.Add($"{ColorCode.Turn}{Lang.Turn} 1{ColorCode.White}");

        _UpdateStatDisplay(0);

        // setup stage
        Stage.AddRange(_Actors);
        Stage.Cleanup();
    }

    internal static void _Destroy() {
        foreach (IActor a in _Actors) a.MarkForRemoval();

        Stage.Cleanup();
    }

    #endregion

    #region Update Methods

    internal static void _Update(GameTime gameTime) {
        _CheckOpenLogInspect(true);

        if (_delay > TimeSpan.Zero) {
            _delay -= gameTime.ElapsedGameTime;
            return;
        }

        Assert.InRangeOr(_selectingMove, 0, PosLib.Highest, _ExecutionPhase);

        switch (_selectingMove) {
            case < PosLib.LowestOpp: _SelectPlayerMove(); return;
            case <= PosLib.Highest: _SelectOpponentMove(); return;
            case _ExecutionPhase: _ExecuteMove(); return;
        }
    }

    internal static void _CheckOpenLogInspect(bool changeTarget = false) {
        // todo fix it might still be possible to double the coverleft???
        if (Parellelograms.CoverLeft.Prog == 0) {
            if (InputLib.Check(Keybinds.Menu1)) {
                StateMachine.Add(States.Log);
                return;
            }

            if (InputLib.Check(Keybinds.Menu2)) {
                if (changeTarget) _indexTarget = _GetQueuePos();
                StateMachine.Add(States.Inspect);
                //_InspectLib._Create(); todo inspect Menu not State
                return;
            }
        }

    }

    /// <summary>
    /// Updates bloom labels, queue, and Unit nameplates
    /// </summary>
    internal static void _UpdateStatDisplay(int curPos) {
        Assert.InRange(curPos, 0, PosLib.Highest);

        // Update bloom labels
        for (int i = 0; i < TeamCount; i++) {
            // todo fix it getting confused by the /
            _BloomLabels[i].Text =
                $"{ColorCode.Stat}{Lang.Bloom}{ColorCode.White}: {ColorCode.Bloom}{Battle.GetTeamBySide((Side) i).Bloom}{ColorCode.White}//{ColorCode.Bloom}1,000";
        }

        Unit[] units = Battle.GetAllUnits();
        StringBuilder sb = new();

        // Update nameplates
        for (int i = 0; i < units.Length; i++) {
            // Stat display
            _Stats[i].Text = $"{units[i].FormatName(false)}\nHP: {units[i].Hp}{(units[i].Shield > 0 ? $"{units[i].Shield.Format(ColorCode.Shield, false)}{ColorCode.White}" : "")}//{units[i].GetBaseStat(Stats.Hp)}\nSP: {(units[i].IsBoolStat(BoolStats.InfiniteSp) ? '∞' : $"{units[i].Sp.Format(false)}//{1000.Format(false)}")}";

            // Buff display
            int buffCount = 0;

            // List stage changes
            foreach (StageType stageType in Core.StageTypes) {
                int stage = units[i].GetStage(stageType);
                if (stage != 0) {
                    if (buffCount > 0 && buffCount % 4 == 0) sb.Append('\n');

                    buffCount++;

                    sb.Append(stageType.Icon).Append(ColorCode.White).Append((stage >= 1) ? '+' : "")
                    .Append(stage).Append('(').Append(units[i].GetStageTurns(stageType)).Append(") ");
                }
            }

            // List buffs
            List<BuffInstance> buffInstances = units[i].BuffInstances;

            foreach (BuffInstance buffInstance in buffInstances) {
                if (buffCount > 0 && buffCount % 4 == 0) sb.Append('\n');

                buffCount++;

                if (buffInstance.Buff == Buffs.Defend) {
                    sb.Append(buffInstance.Buff.Icon).Append(ColorCode.White).Append('x')
                            .Append(units[i].Defend.Format()).Append('(')
                            .Append(buffInstance.Turns).Append(") ");
                } else if (buffInstance.Buff == Buffs.Shield) {
                    sb.Append(buffInstance.Buff.Icon).Append(ColorCode.White).Append('x')
                            .Append(units[i].Shield.Format()).Append('(')
                            .Append(buffInstance.Turns).Append(") ");
                } else {
                    sb.Append(buffInstance.Buff.Icon).Append(ColorCode.White);
                    if (buffInstance.Buff.MaxStacks > 1) {
                        sb.Append('x').Append(buffInstance.Stacks);
                    }

                    sb.Append('(');
                    if (buffInstance.Turns < BuffInstance.InfiniteTurns) sb.Append(buffInstance.Turns);
                    else sb.Append('∞');
                    sb.Append(") ");
                }

                _Buffs[i].Text = sb.ToString();
            }


        }

        // Update queue
        SortByAgi(units);
        _UpdateQueueIndex(curPos, units);
        _Queue.SetText([.. units.Select(u => u.FormatName(false))]);
    }

    /// <summary>
    /// Set queue index to the Unit currently acting or selecting their move
    /// </summary>
    internal static void _UpdateQueueIndex(int curPos, Unit[]? units = null) {
        if (units is null) {
            units = Battle.GetAllUnits();
            SortByAgi(units);
        }

        _Queue.Index = units.IndexOf(units.FirstOrDefault(u => u.Pos == curPos));
    }

    internal static int _GetQueuePos() => _selectingMove == _ExecutionPhase ? _usingMove : _selectingMove;

    #endregion

    #region Move Execution Methods

    private static void _SelectPlayerMove() {
        if (_selectingMove >= Battle.PlayerTeam.Units.Length) return;

        _SkillsL.Position = new Vector2(600, 500 + (450 * _selectingMove));
        // todo support ExA
        // todo do not assign every frame
        _SkillsL.Text =
            // todo list all skills
            $"{Battle.PlayerTeam.Units[_selectingMove].SkillInstances[0].Skill
                // temp
                // todo real skill description display
                .GetName()}({ColorCode.Cooldown}{Battle.PlayerTeam.Units[_selectingMove].SkillInstances[0].Cooldown}{ColorCode.White})";

        _SelectMove();
    }

    private static void _SelectOpponentMove() {
        if (Settings.SelectOpponentMoves) {
            _SelectMove();
            return;
        }

        // temp
        Skill selectedSkill = Skills.Nothing;
        // todo AI
        Unit target = Battle.PlayerTeam.Units[0];
        // todo support ExA
        _Moves[_selectingMove].Text = $"{selectedSkill.GetName()} → {target.FormatName(false)}";
        _CurMoves.Add(new Move(new SkillInstance(selectedSkill),
            Battle.OpponentTeam.Units[_selectingMove - PosLib.LowestOpp], target.Pos));

        _selectingMove++;

        if (_selectingMove > PosLib.Highest) _selectingMove = _ExecutionPhase;
    }

    private static void _SelectMove() {
        // Cancel
        if (_selectingMove != 0 && InputLib.Check(Keybinds.Back)) {
            // todo
            _SkillsL.Text = "";

            _selectingMove--;
            _UpdateQueueIndex(_selectingMove);

            _indexSkill = 0;
            _Moves[_selectingMove].Text = "";

            for (int i = 0; i <= Battle.PlayerTeam.Units[_selectingMove].ExtraActions; i++) _CurMoves.RemoveLast();

            return;
        }

        _indexSkill = MenuLib.CheckMovement1D(_indexSkill, 6); // todo

        //MenuLib.handleOptColor(skills, indexSkill); todo

        if (!InputLib.Check(Keybinds.Confirm)) return;

        _selectedSkillInstance = Battle.PlayerTeam.Units[_selectingMove].SkillInstances[_indexSkill];
        _Moves[_selectingMove].Text = _selectedSkillInstance.Skill.GetName();

        // todo
        _SkillsL.Text = "";

        _indexTarget = _selectedSkillInstance.Skill.GetStartingIndex();
        States.Battle.AddMenu(_TargetingLib._Menu);
        //StateMachine.Add(States.Targeting);
    }

    // todo split out into multiple fns
    private static void _ExecuteMove() {
        if (_CurMoves.Count == 0) {
            _EndTurn();
            return;
        }

        // Sort moves
        // todo test
        _CurMoves.Sort((a, b) => {
            // Sort by Prio
            int prioComparison = a.SkillInstance.Skill.Prio.CompareTo(b.SkillInstance.Skill.Prio);
            if (prioComparison != 0) return prioComparison;

            // Sort by Agi
            int agiComparison = a.Self.GetStat(Stats.Agi).CompareTo(b.Self.GetStat(Stats.Agi));
            if (agiComparison != 0) return agiComparison;

            // Sort by Pos
            return a.Self.Pos.CompareTo(b.Self.Pos);
        });

        Move move = _CurMoves[0];
        Unit self = move.Self;
        _usingMove = self.Pos;

        _UpdateStatDisplay(self.Pos);

        if (self.IsBoolStat(BoolStats.UnableToAct)) {
            LogLib.Add(string.Format(Lang.LogSkillFailUnableToAct, move.GetTriesToUseString(),
            string.Format(Lang.LogButIsUnableToAct, self.GetBoolStat(BoolStats.UnableToAct).ToString()))); // todo test
            _EndMove();
            return;
        }

        int cd = move.SkillInstance.Cooldown;
        if (cd > 0 && _applyingEffect == 0) {
            LogLib.Add(string.Format(Lang.LogSkillFailCooldown, move.GetTriesToUseString(),
                Lang.LogButItsOnCooldown.FormatIcu(cd)));
            _EndMove();
            return;
        }

        if (!move.IsInRange()) {
            LogLib.Add(string.Format(Lang.LogSkillFailRange, move.GetTriesToUseString(), Lang.LogButCantReach));
            _EndMove();
            return;
        }

        // SP after move executes. Invalid spNew will cancel move
        int spNew = 0;

        Skill skill = move.SkillInstance.Skill;

        if (_applyingEffect == 0) {
            move.SkillInstance.Cooldown = cd;

            Element element = skill.GetElement();

            Team team = self.Pos < PosLib.LowestOpp ? Battle.PlayerTeam : Battle.OpponentTeam;
            int cost = self.IsBoolStat(BoolStats.InfiniteSp) && !skill.IsBloom ? 0 : skill.Cost;

            // Make sure cost doesn't go below 1 unless the skill has a base 0 SP cost
            int costMod = cost > 0 ? (int) Math.Max(cost * (self.GetElementSpCost(element) / 1000d), 1) : 0;

            int change = (int) (skill.IsBloom ? costMod : costMod * self.GetMult(Mults.SpUse));
            spNew = skill.IsBloom ? team.Bloom - change : self.Sp - change;

            if (spNew < 0) {
                string msg = string.Format(Lang.LogSkillFailSp, move.GetTriesToUseString(),
                    Lang.LogButDoesntHaveEnough.FormatIcu(Convert.ToInt32(skill.IsBloom)));
                LogLib.Add(msg);
            } else {
                Unit target = Battle.GetUnitAtPos(move.TargetPos);

                int spOld = skill.IsBloom ? team.Bloom : self.Sp;
                change *= -1;
                string changeSp = "";

                if (spOld != spNew) {
                    changeSp = Lang.LogSkillUseChangeSpBloom.FormatIcu(Convert.ToInt32(skill.IsBloom),
                        spOld.Format(ColorCode.Sp, false),
                        spNew.Format(ColorCode.Sp, false), change.Format());
                }

                if (skill.IsBloom) team.Bloom = spNew;
                else self.Sp = spNew;

                LogLib.Add(Lang.LogSkillUse.FormatIcu(self.FormatName(false),
                    skill.GetName(ColorCode.Skill),
                    target.FormatName(false),
                    Convert.ToInt32(skill.IsRangeSelf()).ToString(), changeSp));

                self.OnUseSkill(target, skill);

                // Color move for currently acting combatant (temp)
                for (int i = 0; i < UnitCount; i++) {
                    //moves[i].Color = (self.Pos == i) ? Color.Pink : Color.White;
                }

                _prevResults = new ResultType[UnitCount];
            }
        }

        SkillEffect[] skillEffects = skill.SkillEffects;

        Unit targetMain = Battle.GetUnitAtPos(move.TargetPos);

        // The check for reaching skillEffects.length will only apply here if the length is 0, because otherwise it'll
        // be applied at the end
        // todo should this be checking _applyingEffect > _nonFails
        if (spNew < 0 || _applyingEffect == skillEffects.Length || (_nonFails == 0 && _applyingEffect > 0)) {
            _UpdateStatDisplay(self.Pos);
            _EndMove();
            return;
        }

        foreach (int targetPos in skill.Range.GetTargetPositions(self.Pos, move.TargetPos)) {
            if (targetPos == PosLib.Invalid) continue;

            Unit targetCur = Battle.GetUnitAtPos(targetPos);
            if (_applyingEffect == 0) {
                _prevResults[targetPos] = ResultType.Success;

                targetCur.OnTargetedBySkill(self, skill);
            }

            if (_prevResults[targetPos] == ResultType.Fail) continue;

            _nonFails++;

            ResultType resultType = skillEffects[_applyingEffect]
                .Apply(self, targetCur, targetCur == targetMain, _prevResults[targetPos]);
            _prevResults[targetPos] = resultType;
        }

        if (!skillEffects[_applyingEffect].IsInstant) _delay += TimeSpan.FromSeconds(0.25f * Settings.BattleSpeed);

        _applyingEffect++;

        _UpdateStatDisplay(self.Pos);

        if (skillEffects.Length != _applyingEffect) return;

        _EndMove();
        move.SkillInstance.Cooldown = move.SkillInstance.Skill.Cooldown;
        // todo delete killed units
    }

    private static void _EndMove() {
        _applyingEffect = 0;
        _nonFails = 0;
        _CurMoves.RemoveFirst();
        _delay += TimeSpan.FromSeconds(1) * Settings.BattleSpeed;
    }

    private static void _EndTurn() {
        _selectingMove = 0;
        _usingMove = _SelectionPhase;
        Battle.Turn++;

        for (int i = 0; i < UnitCount; i++) {
            _Moves[i].Text = "";
            //moves[i].Color = ColorCode.White;
        }

        foreach (Unit unit in Battle.GetAllUnits()) {
            unit.Sp = (int) Math.Min(unit.Sp + (100 * unit.GetMult(Mults.SpGain)), 1000);

            foreach (Passive passive in unit.Passives) {
                StringBuilder turnEnd1 =
                    new StringBuilder(string.Format(Lang.LogTurnEndEffect, unit.FormatName(),
                        ColorCode.Passive + passive.GetName())).Append(' ');

                foreach (IBuffEffect buffEffect in passive.BuffEffects) {
                    StringBuilder turnEnd2 = new();
                    string[] effectMsgs = buffEffect.OnTurnEnd(unit, 1);

                    foreach (string effectMsg in effectMsgs) {
                        if (!string.IsNullOrEmpty(effectMsg)) turnEnd2.Append(effectMsg);
                    }

                    if (turnEnd2.Length > 0) LogLib.Add(turnEnd1 + turnEnd2.ToString());
                }
            }

            foreach (BuffInstance buffInstance in unit.BuffInstances) {
                StringBuilder turnEnd1 =
                    new StringBuilder(string.Format(Lang.LogTurnEndEffect, unit.FormatName(),
                    buffInstance.Buff.GetName())).Append(' ');

                foreach (IBuffEffect buffEffect in buffInstance.Buff.BuffEffects) {
                    StringBuilder turnEnd2 = new();
                    string[] effectMsgs = buffEffect.OnTurnEnd(unit, buffInstance.Stacks);
                    foreach (string effectMsg in effectMsgs) {
                        if (!string.IsNullOrEmpty(effectMsg)) turnEnd2.Append(effectMsg);
                    }

                    if (turnEnd2.Length > 0) LogLib.Add(turnEnd1 + turnEnd2.ToString());
                }
            }

            // Decrement stage/shield/buff turns and remove expired stages/shields/buffs
            unit.DecrementTurns();
        }

        // todo is trailing white needed
        LogLib.Add($"{ColorCode.Turn}{Lang.Turn} {Battle.Turn}{ColorCode.White}");
        LogLib.Add(Lang.LogGainSpBloom);

        // Increase bloom
        Battle.PlayerTeam.Bloom = Math.Min(Battle.PlayerTeam.Bloom + 100, 1000);
        Battle.OpponentTeam.Bloom = Math.Min(Battle.OpponentTeam.Bloom + 100, 1000);

        _UpdateStatDisplay(0);
    }

    #endregion

    #region Utility Methods

    public static BuffType GetStageBuffType(int stacks) => stacks >= 0 ? BuffType.Buff : BuffType.Debuff;

    public static void SortByAgi(Unit[] units) =>
        units.Sort((a, b) => a.GetStat(Stats.Agi).CompareTo(b.GetStat(Stats.Agi)));

    #endregion
}