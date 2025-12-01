using System;
using System.Text;
using API.Battle.BuffEffects;
using API.Battle.SkillEffects;
using API.Extensions;
using API.Graphics;
using API.Input;
using API.Menu;
using API.Menu.State;
using API.Save;
using Microsoft.Xna.Framework;
using static API.Input.InputPrompts;

namespace API.Battle.State;

// Significant using order
using static API.Battle.State.BattleLib;

public sealed class MenuBattle : IState {
    #region Fields

    private static readonly Label SkillsL = new(Core.StageBattle);

    private static readonly Label Turn = new(Core.StageBattle, $"{Colors.Turn}{Lang.Turn} 1") {
        Alignment = Alignment.Center,
        Position = new Vector2(World.W2, 90)
    };

    /// <summary>
    /// Index of the currently-applying SkillEffect of the current Move
    /// </summary>
    private static int applyingEffect = 0;

    /// <summary>
    /// Previous SkillEffect resultTypes for each pos
    /// </summary>
    private static ResultType[] prevResults = new ResultType[UnitCount];

    /// <summary>
    /// Amount of non-fail results for the current Move so far
    /// </summary>
    private static int nonFails = 0;

    /// <summary>
    /// If <c>selectingMove</c> is this, then moves are currently executing
    /// </summary>
    private const int ExecutionPhase = 100;

    /// <summary>
    /// Time until the next battle action can occur
    /// </summary>
    private static TimeSpan delay;

    #endregion

    #region Impl

    public MenuBattle() {
        if (Core.MenuBattle is not null) {
            throw new InvalidOperationException(string.Format(Lang.MultipleInstance, nameof(MenuBattle)));
        }
    }

    public void Update(GameTime gameTime) {
        HandleDebug();

        if (delay > TimeSpan.Zero) {
            delay -= gameTime.ElapsedGameTime;
            return;
        }

        switch (selectingMove) {
            case < PosLib.LowestOpp: SelectPlayerMove(); return;
            case <= PosLib.HighestOpp: SelectOpponentMove(); return;
            default: ExecuteMove(); return;
        }
    }

    public void Draw(GameTime gameTime) => Core.StageBattle.Draw(gameTime);

    public string GetInputPrompt() => IState.GetInputPromptString(ScrollUpDown, ScrollFaster, Confirm, Back, Log, Inspect);

    #endregion

    #region Static

    private static void SelectPlayerMove() {
        if (selectingMove >= Battle.PlayerTeam.Units.Length) return;

        SkillsL.Position = new Vector2(600, 500 + (450 * selectingMove));
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

        if ((selectingMove - PosLib.LowestOpp) > Battle.OpponentTeam.Units.Length) {
            selectingMove = ExecutionPhase;
            return;
        }

        // temp
        Skill selectedSkill = Skills.Nothing;
        // todo AI
        Unit target = Battle.PlayerTeam.Units[0];
        // todo support ExA
        Moves[selectingMove].Text = $"{selectedSkill.GetName()} → {target.FormatName(false)}";
        CurMoves.Add(new Move(new SkillInstance(selectedSkill),
            Battle.OpponentTeam.Units[selectingMove - PosLib.LowestOpp], target.Pos));
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
        Core.NavPath.Add(Core.MenuTargeting);
    }

    // todo split out into multiple fns
    private static void ExecuteMove() {
        if (CurMoves.Count == 0) {
            EndTurn();
            return;
        }

        // Sort moves
        // todo test
        CurMoves.Sort((a, b) => {
            // Sort by Prio
            int prioComparison = a.SkillInstance.Skill.Prio.CompareTo(b.SkillInstance.Skill.Prio);
            if (prioComparison != 0) return prioComparison;

            // Sort by Agi
            int agiComparison = a.Self.GetStat(Stats.Agi).CompareTo(b.Self.GetStat(Stats.Agi));
            if (agiComparison != 0) return agiComparison;

            // Sort by Pos
            return a.Self.Pos.CompareTo(b.Self.Pos);
        });

        Move move = CurMoves[0];
        Unit self = move.Self;

        if (self.IsBoolStat(BoolStats.UnableToAct)) {
            MenuLog.Add(string.Format(Lang.LogSkillFailUnableToAct, move.GetTriesToUseString(),
            string.Format(Lang.LogButIsUnableToAct, self.GetBoolStat(BoolStats.UnableToAct).ToString()))); // todo test
            EndMove();
            return;
        }

        int cd = move.SkillInstance.Cooldown;
        if (cd > 0 && applyingEffect == 0) {
            MenuLog.Add(string.Format(Lang.LogSkillFailCooldown, move.GetTriesToUseString(),
                Lang.LogButItsOnCooldown.FormatIcu(cd)));
            EndMove();
            return;
        }

        if (!move.IsInRange()) {
            MenuLog.Add(string.Format(Lang.LogSkillFailRange, move.GetTriesToUseString(), Lang.LogButCantReach));
            EndMove();
            return;
        }

        // SP after move executes. Invalid spNew will cancel move
        int spNew = 0;

        Skill skill = move.SkillInstance.Skill;

        if (applyingEffect == 0) {
            move.SkillInstance.Cooldown = cd;

            Element element = skill.GetElement();

            Team team = self.Pos < PosLib.LowestOpp ? Battle.PlayerTeam : Battle.OpponentTeam;
            int cost = self.IsBoolStat(BoolStats.InfiniteSp) && !skill.IsBloom ? 0 : skill.Cost;

            // Make sure cost doesn't go below 1 unless the skill has a base 0 SP cost
            int costMod = cost > 0 ? (int) Math.Max(cost * (AffLib.SpCost[self.GetAffinity(element)] / 1000d), 1) : 0;

            int change = (int) (skill.IsBloom ? costMod : costMod * self.GetMult(Mults.SpUse));
            spNew = skill.IsBloom ? team.Bloom - change : self.Sp - change;

            if (spNew < 0) {
                string msg = string.Format(Lang.LogSkillFailSp, move.GetTriesToUseString(),
                    Lang.LogButDoesntHaveEnough.FormatIcu(skill.IsBloom.ToInt()));
                MenuLog.Add(msg);
            } else {
                Unit target = Battle.GetUnitAtPos(move.TargetPos);

                int spOld = skill.IsBloom ? team.Bloom : self.Sp;
                change *= -1;
                string changeSp = "";

                if (spOld != spNew) {
                    changeSp = Lang.LogSkillUseChangeSpBloom.FormatIcu(skill.IsBloom.ToInt(), spOld.Format(Colors.Sp, false),
                        spNew.Format(Colors.Sp, false), change.Format());
                }

                if (skill.IsBloom) team.Bloom = spNew;
                else self.Sp = spNew;

                MenuLog.Add(Lang.LogSkillUse.FormatIcu(self.FormatName(false),
                    skill.GetName(Colors.Skill),
                    target.FormatName(false),
                    skill.IsRangeSelf().ToInt().ToString(), changeSp));

                self.OnUseSkill(target, skill);

                // Color move for currently acting combatant (temp)
                for (int i = 0; i < UnitCount; i++) {
                    //moves[i].Color = (self.Pos == i) ? Color.Pink : Color.White;
                }

                prevResults = new ResultType[UnitCount];
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
            if (targetPos == PosLib.Invalid) continue;

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

        if (!skillEffects[applyingEffect].IsInstant) delay += TimeSpan.FromSeconds(0.25f * Settings.BattleSpeed);

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
                    new StringBuilder(string.Format(Lang.LogTurnEndEffect, unit.FormatName(),
                        Colors.Passive + passive.GetName())).Append(' ');

                foreach (IBuffEffect buffEffect in passive.BuffEffects) {
                    StringBuilder turnEnd2 = new();
                    string[] effectMsgs = buffEffect.OnTurnEnd(unit, 1);

                    foreach (string effectMsg in effectMsgs) {
                        if (!string.IsNullOrEmpty(effectMsg)) turnEnd2.Append(effectMsg);
                    }

                    if (turnEnd2.Length > 0) MenuLog.Add(turnEnd1 + turnEnd2.ToString());
                }
            }

            foreach (BuffInstance buffInstance in unit.BuffInstances) {
                StringBuilder turnEnd1 =
                    new StringBuilder(string.Format(Lang.LogTurnEndEffect, unit.FormatName(), buffInstance.Buff.GetName()))
                        .Append(' ');

                foreach (IBuffEffect buffEffect in buffInstance.Buff.BuffEffects) {
                    StringBuilder turnEnd2 = new();
                    string[] effectMsgs = buffEffect.OnTurnEnd(unit, buffInstance.Stacks);
                    foreach (string effectMsg in effectMsgs) {
                        if (!string.IsNullOrEmpty(effectMsg)) turnEnd2.Append(effectMsg);
                    }

                    if (turnEnd2.Length > 0) MenuLog.Add(turnEnd1 + turnEnd2.ToString());
                }
            }

            // Decrement stage/shield/buff turns and remove expired stages/shields/buffs
            unit.DecrementTurns();
        }

        // todo is trailing white needed
        MenuLog.Add($"{Colors.Turn}{Lang.Turn} {Battle.Turn + 1}{Colors.White}");
        MenuLog.Add(Lang.LogGainSpBloom);

        // Increase bloom
        Battle.PlayerTeam.Bloom = Math.Min(Battle.PlayerTeam.Bloom + 100, 1000);
        Battle.OpponentTeam.Bloom = Math.Min(Battle.OpponentTeam.Bloom + 100, 1000);

        UpdateStatDisplay(0);
    }

    #endregion
}
