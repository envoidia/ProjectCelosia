using System;
using System.Collections.Generic;
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

    /// <summary>
    /// If <c>selectingMove</c> is this, then moves are currently executing
    /// </summary>
    private const int _ExecutionPhase = 100;

    #endregion

    #region Display Fields

    private const int _ActorCount = 30;
    private static readonly List<IActor> _Actors = new(_ActorCount);

    private const int _AnimPrimActorCount = 0; // todo
    //private static readonly List<Actor> _AnimPrimActors = new(_AnimPrimActorCount);

    private static readonly Label _Queue = new() {
        Alignment = Alignment.Center,
        Position = new Vector2(World.W2, 180)
    };

    private static readonly Label[] _BloomLabels = new Label[TeamCount];

    // Per-unit Labels
    internal static readonly Label[] _Stats = new Label[UnitCount];
    private static readonly Label[] _Buffs = new Label[UnitCount];
    internal static readonly Label[] _Moves = new Label[UnitCount];

    private static readonly Label _SkillsL = new();

    private static readonly Label _Turn = new() {
        Text = $"{ColorCode.Turn}{Lang.Turn} 1",
        Alignment = Alignment.Center,
        Position = new Vector2(World.W2, 90)
    };

    #endregion

    #region Logic Fields

    internal static readonly List<Move> _CurMoves = new(16);

    /// <summary>
    /// Pos of the Unit that's currently selecting their move. <c>ExecutionPhase</c> = moves are executing
    /// </summary>
    internal static int _selectingMove = 0;

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
        _Actors.AddRange(_Queue, LogLib._BattleLog, _SkillsL, _Turn);

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

        _InspectLib._TranslateUnitNames();

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
        if (InputLib.Check(Keybinds.Menu1)) {
            StateMachine.Add(States.Log);
            return;
        }

        if (InputLib.Check(Keybinds.Menu2)) {
            if (InputLib.Check(Keybinds.Hotkey1)) {
                _indexTarget = _selectingMove;
                StateMachine.Add(States.Inspect);
            } else StateMachine.Add(States.InspectTargeting);

            return;
        }

        if (_delay > TimeSpan.Zero) {
            _delay -= gameTime.ElapsedGameTime;
            return;
        }

        switch (_selectingMove) {
            case < PosLib.LowestOpp: _SelectPlayerMove(); return;
            case <= PosLib.HighestOpp: _SelectOpponentMove(); return;
            default: _ExecuteMove(); return;
        }
    }

    // Updates bloom labels, queue, and Unit nameplates
    internal static void _UpdateStatDisplay(int curPos) {
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

                    sb.Append(stageType.Icon).Append(ColorCode.White).Append((stage >= 1) ? '+' : "").Append(stage)
                    .Append('(').Append(units[i].GetStageTurns(stageType)).Append(") ");
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
        sb = new StringBuilder();
        units.Sort((a, b) => a.GetStat(Stats.Agi).CompareTo(b.GetStat(Stats.Agi)));

        for (int i = 0; i < units.Length; i++) {
            bool active = units[i].Pos == curPos;

            if (active) sb.Append('<');

            sb.Append(units[i].FormatName(false));

            if (active) sb.Append(ColorCode.White).Append('>');

            if (i != units.Length - 1) sb.Append(", ");
        }

        _Queue.Text = sb.ToString();
    }
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

        if ((_selectingMove - PosLib.LowestOpp) > Battle.OpponentTeam.Units.Length) {
            _selectingMove = _ExecutionPhase;
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
    }

    private static void _SelectMove() {
        // Cancel
        if (InputLib.Check(Keybinds.Back) && _selectingMove != 0) {
            // todo
            _SkillsL.Text = "";

            _selectingMove--;
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
        StateMachine.Add(States.Targeting);
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
        if (spNew < 0 || _applyingEffect == skillEffects.Length || (_nonFails == 0 && _applyingEffect > 0)) {
            _EndMove();
            _UpdateStatDisplay(self.Pos);
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
        Battle.Turn++;

        _Turn.Text = $"{ColorCode.Turn}{Lang.Turn} {Battle.Turn}";

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
    #endregion
}