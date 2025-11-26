using System;
using API.Extensions;
using API.Graphics;
using API.Input;
using API.Menu;
using API.Menu.State;
using Microsoft.Xna.Framework;

namespace API.Battle.State;

// Significant using order
using static API.Input.InputPrompts;
using static API.Battle.State.BattleLib;


public sealed class MenuTargeting : IState {

    /// <summary>
    /// How many extra actions have been used for the currently acting Unit
    /// </summary>
    private static int extraActions = 0;

    public MenuTargeting() {
        if (Core.MenuTargeting is not null) {
            throw new InvalidOperationException(string.Format(Lang.MultipleInstance, nameof(MenuTargeting)));
        }
    }

    public void Update(GameTime gameTime) {
        HandleDebug();

        if (Core.Input.CheckInput(Keybinds.Back)) {
            //foreach (Label stat in stats) stat.Color = Colors.White;
            Moves[selectingMove].Text = "";

            Core.NavPath.Remove();
            return;
        }

        indexTarget = MenuLib.CheckMovementTargeting(indexTarget, selectingMove, selectedSkillInstance.Skill.Range);

        //MenuLib.handleOptColor(stats, indexTarget);

        if (!Core.Input.CheckInput(Keybinds.Confirm)) return;

        Unit self = Battle.PlayerTeam.Units[selectingMove];
        Unit target = indexTarget < PosLib.LowestOpp
            ? Battle.PlayerTeam.Units[indexTarget]
            : Battle.OpponentTeam.Units[indexTarget - PosLib.LowestOpp];
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

        Core.NavPath.Remove();
    }

    public void Draw(GameTime gameTime) {
        Core.StageBattle.Draw(gameTime);
    }

    public string GetInputPrompt() => IState.GetInputPromptString(Move, Confirm, Back, Log);
}
