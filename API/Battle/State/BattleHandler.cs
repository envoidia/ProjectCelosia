using System;
using System.Collections.Generic;
using System.Text;
using API.Extensions;
using API.Graphics;
using API.Input;
using Microsoft.Xna.Framework;

namespace API.Battle.State;

// todo finish + cleanup + reduce visibility when possible
public static class BattleHandler {
    public static Battle Battle { get; private set; } // todo

    private const int TeamCount = 2;
    internal const int StatTypeCount = 3;
    internal const int TeamSize = 4;
    internal const int StatCount = 6;
    internal const int UnitCount = 8;

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

    internal static readonly List<Move> CurMoves = new(128); // todo decide capacity

    /// <summary>
    /// Pos of the Unit that's currently selecting their move. <c>ExecutionPhase</c> = moves are executing
    /// </summary>
    internal static int selectingMove = 0;

    internal static SkillInstance selectedSkillInstance;

    // Menu navigation
    internal static int indexSkill = 0;
    internal static int indexTarget = 0;

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
}