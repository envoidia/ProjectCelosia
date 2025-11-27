using System;
using System.Collections.Generic;
using System.Text;
using API.Extensions;
using API.Graphics;
using API.Input;
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

    private const int TeamCount = 2;
    internal const int StatTypeCount = 3;
    internal const int TeamSize = 4;
    internal const int StatCount = 6;
    internal const int UnitCount = TeamSize * TeamCount;

    #endregion

    #region Display Fields

    private static readonly Label Queue = new(Core.StageBattle) {
        Alignment = Alignment.Center,
        Position = new Vector2(World.W2, 180)
    };

    private static readonly Label[] BloomLabels = new Label[TeamCount];

    // Per-unit Labels
    internal static readonly Label[] StatsL = new Label[UnitCount];
    private static readonly Label[] BuffsL = new Label[UnitCount];
    internal static readonly Label[] Moves = new Label[UnitCount];
    private static readonly Label[] UnitNames = new Label[UnitCount];

    #endregion

    #region Logic Fields

    internal static readonly List<Move> CurMoves = new(16);

    /// <summary>
    /// Pos of the Unit that's currently selecting their move. <c>ExecutionPhase</c> = moves are executing
    /// </summary>
    internal static int selectingMove = 0;

    internal static SkillInstance selectedSkillInstance; // todo

    // Menu navigation
    internal static int indexSkill = 0;
    internal static int indexTarget = 0;

    // todo replays
    // serialize each unit and then just store each move as (starting Pos of Self, index in Self's Skill list, target Pos)?
    // instead of serializing the units, just store short lookups for each component?

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
            int x1 = 75;
            int x2 = 600;
            int y = 450 + (450 * i);

            if (i >= PosLib.LowestOpp) {
                x1 = World.W - 500;
                x2 = World.W - 825;
                y = 450 + (450 * (i - PosLib.LowestOpp));
            }

            StatsL[i] = new Label(Core.StageBattle) { Position = new Vector2(x1, y) };
            BuffsL[i] = new Label(Core.StageBattle) { Position = new Vector2(x1, y + 150) };
            Moves[i] = new Label(Core.StageBattle) { Position = new Vector2(x2, y + 50) };
            UnitNames[i] = new Label(Core.StageBattle) { Y = 52 };
        }

        // Todo inspect init

        // Sort stages
        Core.StageBattle.Sort();
        Core.StageInspect.Sort();
    }

    public static void StartBattle() {
        // temp setup teams
        Battle = Core.battle;

        MenuLog.Add($"{Colors.Turn}{Lang.Turn} 1{Colors.White}");

        UpdateStatDisplay(0);
    }

    public static void EndBattle() { }

    #endregion

    #region Update Methods

    public static void HandleDebug() {
        if (Core.Input.CheckInput(Keybinds.DebugDumpLog)) {
            Console.WriteLine(string.Join('\n', MenuLog.LogText));
        }
    }

    // Updates bloom labels, queue, and Unit nameplates
    internal static void UpdateStatDisplay(int curPos) {
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
                    sb.Append(buffInstance.Buff.Icon).Append(Colors.White).Append('x')
                            .Append(units[i].Defend.Format()).Append('(')
                            .Append(buffInstance.Turns).Append(") ");
                } else if (buffInstance.Buff == Buffs.Shield) {
                    sb.Append(buffInstance.Buff.Icon).Append(Colors.White).Append('x')
                            .Append(units[i].Shield.Format()).Append('(')
                            .Append(buffInstance.Turns).Append(") ");
                } else {
                    sb.Append(buffInstance.Buff.Icon).Append(Colors.White);
                    if (buffInstance.Buff.MaxStacks > 1) {
                        sb.Append('x').Append(buffInstance.Stacks);
                    }

                    sb.Append('(');
                    if (buffInstance.Turns < BuffInstance.InfiniteTurns) sb.Append(buffInstance.Turns);
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

    #region Utility Methods
    public static BuffType GetStageBuffType(int stacks) => stacks >= 0 ? BuffType.Buff : BuffType.Debuff;
    #endregion
}