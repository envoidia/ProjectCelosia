using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API.Battle.BuffEffects;
using API.Battle.SkillEffects;
using API.Debug;
using API.Extensions;
using API.Graphics;
using API.Input;
using API.Menu.State;
using API.Menu.Widget;
using API.Modding;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Battle.State;

// todo finish + cleanup + reduce visibility when possible
public static class BattleLib
{
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

    private const int _ActorCount = 38;
    private static readonly List<IActor> _Actors = new(_ActorCount);

    private const int _AnimPrimActorCount = 0; // todo
    //private static readonly List<Actor> _AnimPrimActors = new(_AnimPrimActorCount);

    private static readonly string[] _UnitList = new string[UnitCount];
    internal static readonly TabBarWidget _Queue = new(new(World.W2, 90), UnitCount)
    {
        CheckInput = false,
        Priority = RenderPriority.B3Med,
        OnChangeIndex = _InspectLib._UpdateInspectUnitPage,
        AnimFromDir = Dir.Up
    };

    private static readonly Label[] _BloomLabels = new Label[TeamCount];


    // Per-unit graphics
    private static readonly ARectangle[] _Sprites = new ARectangle[UnitCount];
    internal static readonly Label[] _Stats = new Label[UnitCount];
    private static readonly Label[] _Buffs = new Label[UnitCount];
    internal static readonly Label[] _Moves = new Label[UnitCount];

    internal static readonly Label _SkillDesc = new(RenderPriority.B2Low)
    {
        MaxWidth = 1150,
        Padding = new(10),
        BackgroundType = BackgroundType.Parellelogram
    };

    internal static readonly ListWidget _SkillList = new(new(), true,
        7, RenderPriority.B2Low)
    {
        FixedWidth = 800,
        HasBackground = true,
        Slant = ListWidget.NormalSlant,
        Priority = RenderPriority.B1High,
        OnChangeIndex = static index =>
        {
            Unit u = Battle.GetUnitAtPos(_selectingMove);
            List<SkillInstance> skills = u.SkillInstances;
            _SkillDesc.Text = skills[index].Skill.GetFullDesc();
            _SkillDesc.X = _GetSkillDescX();
        }
    };

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
    private static SkillResultType[] _prevResults = new SkillResultType[UnitCount];

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

    internal static void _Init()
    {
        // Add preinitialized actors
        _Actors.AddRange(_Queue, LogLib._BattleLog, _SkillDesc, _SkillList);

        // Setup Labels
        for (int i = 0; i < TeamCount; i++)
        {
            // todo midgame translation
            _Actors.Add(_BloomLabels[i] = new Label()
            {
                Position = new(i == 1 ? World.W - 105 : 105, 135),
                Alignment = i == 1 ? Alignment.TopRight : Alignment.TopLeft
            });
        }

        // Per-unit graphics
        for (int i = 0; i < UnitCount; i++)
        {
            int x1 = 75;
            int x2 = 500;
            int y = 300 + (450 * i);
            Dir dir = i > PosLib.HighestAlly ? Dir.Right : Dir.Left;

            if (i >= PosLib.LowestOpp)
            {
                x1 = World.W - 500;
                x2 = World.W - 965;
                y = 300 + (450 * (i - PosLib.LowestOpp));
            }

            _Actors.Add(_Sprites[i] = new(ThemeColor.TransBlack)
            {
                Position = new(x2, y),
                Size = new(RenderLib.UnitSpriteSize),
                AnimFromDir = dir,
                OutlineColor = ThemeColor.White
            });
            _Actors.Add(_Stats[i] = new()
            {
                Position = new(x1, y),
                AnimFromDir = dir
            });
            _Actors.Add(_Buffs[i] = new()
            {
                Position = new(x1, y + 150),
                AnimFromDir = dir
            });
            _Actors.Add(_Moves[i] = new()
            {
                Position = new(x2 + RenderLib.UnitSpriteSize + 20, y),
                AnimFromDir = dir,
                BackgroundType = BackgroundType.Parellelogram
            });
        }

        _SkillList.CurDir = SelectionType.HorizVert;

        Assert.LenIs(_Actors, _ActorCount);
    }
    internal static void _Create()
    {
        // temp setup teams
        Battle = Core.Battle;

        // Setup unit names for queue
        // todo account for non-8 units?
        // todo unify for nameplates
        Unit[] u = Battle.GetAllUnits();

        for (int i = 0; i < UnitCount; i++)
        {
            _UnitList[i] = u[i].FormatName(false);
        }

        _Queue.SetText(_UnitList);

        _InspectLib._LateInit();

        LogLib.Add(_GetTurnString(1));

        _UpdateStatDisplay(0);

        // setup stage
        Stage.AddRange(_Actors);
        Stage.Sort();

        _SetupSkillList(_selectingMove);

        // todo better fix
        for (int i = 0; i < _SkillList.LabelsL.Count; i++)
        {
            _SkillList.LabelsL[i].Prog = Progress.One;
            _SkillList.LabelsR[i].Prog = Progress.One;
        }
    }

    // todo remove?
    internal static void _Destroy()
    {
        foreach (IActor a in _Actors)
        {
            a.Destroy();
        }
    }

    #endregion

    #region Update Methods

    internal static void _Update(GameTime gt)
    {
        //if (Parellelograms.CoverLeft.Prog == 0) {{_CheckOpenLogInspect(true);
        //}
        //else return;
        // todo convert skill selection to menu and remove this check (prevent dbgconsole from blocking battle exec
        // and targeting from blocking inspect)
        if (States.Battle.Menus.Count > 0)
        {
            return;
        }

        _CheckOpenLogInspect();

        if (_delay > TimeSpan.Zero)
        {
            _delay -= gt.ElapsedGameTime;
            return;
        }

        Assert.InRangeOr(_selectingMove, 0, PosLib.Highest, _ExecutionPhase);

        switch (_selectingMove)
        {
            case < PosLib.LowestOpp:
                _SelectPlayerMove(gt);
                return;
            case <= PosLib.Highest:
                _SelectOpponentMove(gt);
                return;
            case _ExecutionPhase:
                _ExecuteMove();
                return;
        }
    }

    internal static void _CheckOpenLogInspect(bool changeTarget = false)
    {
        // todo fix it might still be possible to double the coverleft???
        if (InputLib.Check(Keybinds.Menu1))
        {
            StateMachine.Add(States.Log);
            return;
        }

        if (InputLib.Check(Keybinds.Menu2))
        {
            if (changeTarget)
            {
                _indexTarget = _GetQueuePos();
            }

            //StateMachine.Add(States.Inspect);
            _InspectLib._Create();
            return;
        }
    }

    /// <summary>
    /// Updates bloom labels, queue, and Unit nameplates
    /// </summary>
    internal static void _UpdateStatDisplay(int curPos)
    {
        Assert.InRange(curPos, 0, PosLib.Highest);

        // Update bloom labels
        for (int i = 0; i < TeamCount; i++)
        {
            // todo fix it getting confused by the /
            _BloomLabels[i].Text =
                $"{ThemeColor.Stat.Str}{"Bloom".GetLang()}{ThemeColor.White.Str}: {ThemeColor.Bloom.Str}{Battle.GetTeamBySide((Side) i).Bloom}{ThemeColor.White.Str}//{ThemeColor.Bloom.Str}1,000";
        }

        Unit[] units = Battle.GetAllUnits();

        const int Cap = 48;
        StringBuilder sb = new(Cap);

        // Update nameplates
        for (int i = 0; i < units.Length; i++)
        {
            // Stat display
            _Stats[i].Text = $"{units[i].FormatName(false)}\n{"StatHp".GetLang()}: {units[i].Hp}{(units[i].Shield > 0 ? $"{units[i].Shield.Format(ThemeColor.Shield, false)}{ThemeColor.White.Str}" : "")}//{units[i].GetBaseStat(Stats.Hp)}\n{"StatSp".GetLang()}: {(units[i].IsBoolStat(BoolStats.InfiniteSp) ? '∞' : $"{units[i].Sp.Format(false)}//{1000.Format(false)}")}";

            // Buff display
            int buffCount = 0;

            // List stage changes
            foreach (StageType stageType in Registry.Of<StageType>())
            {
                int stage = units[i].GetStage(stageType);
                if (stage != 0)
                {
                    if (buffCount > 0 && buffCount % 4 == 0)
                    {
                        sb.Append('\n');
                    }

                    buffCount++;

                    sb.Append(stageType.Icon).Append(ThemeColor.White.Str).Append((stage >= 1) ? '+' : "")
                    .Append(stage).Append('(').Append(units[i].GetStageTurns(stageType)).Append(") ");
                }
            }

            // List buffs
            List<BuffInstance> buffInstances = units[i].BuffInstances;

            foreach (BuffInstance buffInstance in buffInstances)
            {
                if (buffCount > 0 && buffCount % 4 == 0)
                {
                    sb.Append('\n');
                }

                buffCount++;

                if (buffInstance.Buff == Buffs.Defend)
                {
                    sb.Append(buffInstance.Buff.Icon).Append(ThemeColor.White.Str).Append('x')
                            .Append(units[i].Defend.Format()).Append('(')
                            .Append(buffInstance.Turns).Append(") ");
                }
                else if (buffInstance.Buff == Buffs.Shield)
                {
                    sb.Append(buffInstance.Buff.Icon).Append(ThemeColor.White.Str).Append('x')
                            .Append(units[i].Shield.Format()).Append('(')
                            .Append(buffInstance.Turns).Append(") ");
                }
                else
                {
                    sb.Append(buffInstance.Buff.Icon).Append(ThemeColor.White.Str);

                    if (buffInstance.Buff.MaxStacks > 1)
                    {
                        sb.Append('x').Append(buffInstance.Stacks);
                    }

                    sb.Append('(');

                    if (buffInstance.Turns < BuffInstance.InfiniteTurns)
                    {
                        sb.Append(buffInstance.Turns);
                    }
                    else
                    {
                        sb.Append('∞');
                    }

                    sb.Append(") ");
                }

                Assert.CapIs(sb, Cap); // todo remove before final release
                _Buffs[i].Text = sb.ToString();
            }
        }

        // Update queue
        SortByAgi(units);
        _Queue.Index = _GetQueueIndex(curPos, units);
        _Queue.SetText([.. units.Select(static u => u.FormatName(false))]);
    }

    internal static Unit[] _GetUnitsSortedByAgi()
    {
        Unit[] units = Battle.GetAllUnits();
        SortByAgi(units);
        return units;
    }

    /// <summary>
    /// Gets the queue index of the Unit currently acting or selecting their move
    /// </summary>
    internal static int _GetQueueIndex(int curPos, Unit[]? units = null)
    {
        units ??= _GetUnitsSortedByAgi();
        return units.IndexOf(units.FirstOrDefault(u => u.Pos == curPos));
    }

    /// <returns>
    /// Pos of the Unit that the queue should currently be focused on
    /// </returns>
    internal static int _GetQueuePos()
    {
        return _selectingMove == _ExecutionPhase ? _usingMove : _selectingMove;
    }

    #endregion

    #region Move Execution Methods

    private static void _SelectPlayerMove(GameTime gt)
    {
        if (_selectingMove >= Battle.PlayerTeam.Units.Length)
        {
            return;
        }

        _SelectMove(gt);
    }

    private static void _SelectOpponentMove(GameTime gt)
    {
        if (Settings.SelectOpponentMoves)
        {
            _SelectMove(gt);
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

        if (_selectingMove > PosLib.Highest)
        {
            _selectingMove = _ExecutionPhase;
        }
    }

    private static void _SelectMove(GameTime gt)
    {
        // Cancel
        if (_selectingMove != 0 && InputLib.Check(Keybinds.Back))
        {
            _selectingMove--;
            _Queue.Index = _GetQueueIndex(_selectingMove);

            _SkillList.Index = 0;
            _Moves[_selectingMove].Text = "";

            for (int i = 0; i <= Battle.PlayerTeam.Units[_selectingMove].ExtraActions; i++)
            {
                _CurMoves.RemoveLast();
            }

            _SetupSkillList(_selectingMove);

            return;
        }

        _SkillList.Input(gt);

        if (!InputLib.Check(Keybinds.Confirm))
        {
            return;
        }

        // todo fix: crashes if selectopponentmoves is enabled
        _selectedSkillInstance = Battle.PlayerTeam.Units[_selectingMove].SkillInstances[_SkillList.Index];
        _indexTarget = _selectedSkillInstance.Skill.GetStartingIndex();

        _SkillList.IsVisible = false;
        _SkillDesc.IsVisible = false;
        _Moves[_selectingMove].Text = _selectedSkillInstance.Skill.GetName();

        States.Battle.AddMenu(_TargetingLib._Menu);
    }

    // todo move x over based on height
    internal static void _SetupSkillList(int index)
    {
        if (index > PosLib.HighestAlly && !Settings.SelectOpponentMoves)
        {
            _SkillList.IsVisible = false;
            _SkillDesc.IsVisible = false;
            return;
        }

        _SkillList.IsVisible = true;
        _SkillDesc.IsVisible = true;

        Unit u = Battle.GetUnitAtPos(index);
        List<SkillInstance> skills = u.SkillInstances;

        int y = 300 + (450 * index);

        _SkillList.SetTextL([.. skills.Select(s => s.Skill.GetName(ThemeColor.White))]);
        _SkillList.SetTextR([.. skills.Select(s => s.GetCostCdFormatted(u))]);
        _SkillList.Position = new(650 + RenderLib.UnitSpriteSize, y);

        // _SkillList.Y = y;
        // _SkillList.CalcLayout();
        // _SkillList.X = 650 + RenderLib.UnitSpriteSize;// - (_SkillList.Height / RenderLib.DefaultSlant);

        _SkillList.CalcLayout();

        _SkillDesc.Text = skills[0].Skill.GetFullDesc();
        _SkillDesc.Position = new(_GetSkillDescX(), y + 10);
    }

    // todo fix slightly inconsistent x w diff heights
    private static float _GetSkillDescX()
    {
        return 600 + RenderLib.UnitSpriteSize + 800 + 115 - (_SkillDesc.Height / (float) RenderLib.DefaultSlant);
    }

    private static void _ExecuteMove()
    {
        if (_CurMoves.Count == 0)
        {
            _EndTurn();
            return;
        }

        // Sort moves
        // todo test
        _CurMoves.Sort(static (a, b) =>
        {
            // Sort by Prio
            int prioComparison = a.SkillInstance.Skill.Prio.CompareTo(b.SkillInstance.Skill.Prio);
            if (prioComparison != 0)
            {
                return prioComparison;
            }

            // Sort by Agi
            int agiComparison = a.Self.GetStat(Stats.Agi).CompareTo(b.Self.GetStat(Stats.Agi));
            if (agiComparison != 0)
            {
                return agiComparison;
            }

            // Sort by Pos
            return a.Self.Pos.CompareTo(b.Self.Pos);
        });

        Move move = _CurMoves[0];
        Unit self = move.Self;
        _usingMove = self.Pos;

        _UpdateStatDisplay(self.Pos);

        if (self.IsBoolStat(BoolStats.UnableToAct))
        {
            LogLib.Add("LogSkillFailUnableToAct".FormatLang([move.GetTriesToUseString(),
            "LogButIsUnableToAct".FormatLang(self.GetBoolStat(BoolStats.UnableToAct).ToString())])); // todo test
            _EndMove();
            return;
        }

        int cd = move.SkillInstance.Cooldown;
        if (cd > 0 && _applyingEffect == 0)
        {
            LogLib.Add("LogSkillFailCooldown".FormatLang([move.GetTriesToUseString(),
                "LogButItsOnCooldown".IcuFormatLang(cd)]));
            _EndMove();
            return;
        }

        if (!move.IsInRange())
        {
            LogLib.Add("LogSkillFailRange".FormatLang([move.GetTriesToUseString(), "LogButCantReach".GetLang()]));
            _EndMove();
            return;
        }

        // SP after move executes. Invalid spNew will cancel move
        int spNew = 0;

        Skill skill = move.SkillInstance.Skill;

        if (_applyingEffect == 0)
        {
            move.SkillInstance.Cooldown = cd;

            Element element = skill.GetElement();

            Team team = self.Pos < PosLib.LowestOpp ? Battle.PlayerTeam : Battle.OpponentTeam;
            int cost = self.GetCost(skill);

            int change = (int) (skill.IsBloom ? cost : cost * self.GetMult(Mults.SpUse));
            spNew = skill.IsBloom ? team.Bloom - change : self.Sp - change;

            if (spNew < 0)
            {
                string msg = "LogSkillFailSp".FormatLang([move.GetTriesToUseString(),
                    "LogButDoesntHaveEnough".IcuFormatLang(Convert.ToInt32(skill.IsBloom))]);
                LogLib.Add(msg);
            }
            else
            {
                Unit target = Battle.GetUnitAtPos(move.TargetPos);

                int spOld = skill.IsBloom ? team.Bloom : self.Sp;
                change *= -1;
                string changeSp = "";

                if (spOld != spNew)
                {
                    changeSp = "LogSkillUseChangeSpBloom".IcuFormatLang([Convert.ToInt32(skill.IsBloom),
                        spOld.Format(ThemeColor.Sp, false),
                        spNew.Format(ThemeColor.Sp, false), change.Format()]);
                }

                if (skill.IsBloom)
                {
                    team.Bloom = spNew;
                }
                else
                {
                    self.Sp = spNew;
                }

                LogLib.Add("LogSkillUse".IcuFormatLang([self.FormatName(false),
                    skill.GetName(ThemeColor.Skill),
                    target.FormatName(false),
                    Convert.ToInt32(skill.IsRangeSelf()).ToString(), changeSp]));

                self.OnUseSkill(target, skill);

                // Color move for currently acting combatant (temp)
                for (int i = 0; i < UnitCount; i++)
                {
                    //moves[i].Color = (self.Pos == i) ? Color.Pink : Color.White;
                }

                _prevResults = new SkillResultType[UnitCount];
            }
        }

        SkillEffect[] skillEffects = skill.SkillEffects;

        Unit targetMain = Battle.GetUnitAtPos(move.TargetPos);

        // The check for reaching skillEffects.length will only apply here if the length is 0, because otherwise it'll
        // be applied at the end
        // todo should this be checking _applyingEffect > _nonFails
        if (spNew < 0 || _applyingEffect == skillEffects.Length || (_nonFails == 0 && _applyingEffect > 0))
        {
            _UpdateStatDisplay(self.Pos);
            _EndMove();
            return;
        }

        foreach (int targetPos in skill.Range.GetTargetPositions(self.Pos, move.TargetPos))
        {
            if (targetPos == PosLib.Invalid)
            {
                continue;
            }

            Unit targetCur = Battle.GetUnitAtPos(targetPos);
            if (_applyingEffect == 0)
            {
                _prevResults[targetPos] = SkillResultType.Success;

                targetCur.OnTargetedBySkill(self, skill);
            }

            if (_prevResults[targetPos] == SkillResultType.Fail)
            {
                continue;
            }

            _nonFails++;

            SkillResultType resultType = skillEffects[_applyingEffect]
                .Apply(self, targetCur, targetCur == targetMain, _prevResults[targetPos]);
            _prevResults[targetPos] = resultType;
        }

        if (!skillEffects[_applyingEffect].IsInstant)
        {
            _delay += TimeSpan.FromSeconds(0.25f * Settings.BattleSpeed);
        }

        _applyingEffect++;

        _UpdateStatDisplay(self.Pos);

        if (skillEffects.Length != _applyingEffect)
        {
            return;
        }

        _EndMove();
        move.SkillInstance.Cooldown = move.SkillInstance.Skill.Cooldown;
        // todo delete killed units
    }

    private static void _EndMove()
    {
        _applyingEffect = 0;
        _nonFails = 0;
        _CurMoves.RemoveFirst();
        _delay += TimeSpan.FromSeconds(1) * Settings.BattleSpeed;
    }

    private static void _EndTurn()
    {
        _selectingMove = 0;
        _SetupSkillList(_selectingMove);
        _usingMove = _SelectionPhase;
        Battle.Turn++;

        for (int i = 0; i < UnitCount; i++)
        {
            _Moves[i].Text = "";
            //moves[i].Color = Settings.Palette.White;
        }

        foreach (Unit unit in Battle.GetAllUnits())
        {
            unit.Sp = (int) Math.Min(unit.Sp + (100 * unit.GetMult(Mults.SpGain)), 1000);

            const int Cap = 192;

            foreach (Passive passive in unit.Passives)
            {
                foreach (IBuffEffect buffEffect in passive.BuffEffects)
                {
                    StringBuilder turnEnd = new(Cap);
                    string[] effectMsgs = buffEffect.OnTurnEnd(unit, 1);

                    foreach (string effectMsg in effectMsgs)
                    {
                        if (!string.IsNullOrEmpty(effectMsg))
                        {
                            turnEnd.Append(effectMsg);
                        }
                    }

                    if (turnEnd.Length > 0)
                    {
                        Assert.CapIs(turnEnd, Cap); // todo remove before final release
                        LogLib.Add("LogTurnEndEffect".FormatLang([unit.FormatName(),
                        ThemeColor.Passive + passive.GetName()]) + ' ' + turnEnd.ToString());
                    }
                }
            }

            foreach (BuffInstance buffInstance in unit.BuffInstances)
            {
                foreach (IBuffEffect buffEffect in buffInstance.Buff.BuffEffects)
                {
                    StringBuilder turnEnd = new(Cap);
                    string[] effectMsgs = buffEffect.OnTurnEnd(unit, buffInstance.Stacks);
                    foreach (string effectMsg in effectMsgs)
                    {
                        if (!string.IsNullOrEmpty(effectMsg))
                        {
                            turnEnd.Append(effectMsg);
                        }
                    }

                    if (turnEnd.Length > 0)
                    {
                        Assert.CapIs(turnEnd, Cap); // todo remove before final release
                        LogLib.Add("LogTurnEndEffect".FormatLang([unit.FormatName(),
                    buffInstance.Buff.GetName()]) + ' ' + turnEnd.ToString());
                    }
                }
            }

            // Decrement stage/shield/buff turns and remove expired stages/shields/buffs
            unit.DecrementTurns();
        }

        // todo is trailing white needed
        LogLib.Add(_GetTurnString(Battle.Turn));
        LogLib.Add("LogGainSpBloom".GetLang());

        // Increase bloom
        Battle.PlayerTeam.Bloom = Math.Min(Battle.PlayerTeam.Bloom + 100, 1000);
        Battle.OpponentTeam.Bloom = Math.Min(Battle.OpponentTeam.Bloom + 100, 1000);

        _UpdateStatDisplay(0);
    }

    #endregion

    #region Utility Methods

    public static BuffType GetStageBuffType(int stacks)
    {
        return stacks >= 0 ? BuffType.Buff : BuffType.Debuff;
    }

    public static void SortByAgi(Unit[] units)
    {
        units.Sort(static (a, b) => a.GetStat(Stats.Agi).CompareTo(b.GetStat(Stats.Agi)));
    }

    private static string _GetTurnString(int turn)
    {
        return $"{ThemeColor.Turn.Str}{"Turn".GetLang()} {turn}{ThemeColor.White.Str}";
    }

    #endregion
}