using API.Graphics;
using API.Input;
using API.Menu;
using API.Menu.State;
using Microsoft.Xna.Framework;

namespace API.Battle.State;

// Significant using order
using static API.Battle.State.BattleLib;


public static class TargetingLib {
    /// <summary>
    /// How many extra actions have been used for the currently acting Unit
    /// </summary>
    private static int extraActions = 0;

    public static void Update(GameTime gameTime) {
        HandleDebug();

        if (Core.Input.CheckInput(Keybinds.Back)) {
            //foreach (Label stat in stats) stat.Color = Colors.White;
            Moves[selectingMove].Text = "";

            NavPath.Remove();
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

        NavPath.Remove();
    }

    // todo merge
    public static void UpdateInspectTargeting(GameTime gameTime) {
        HandleDebug();

        if (Core.Input.CheckInput(Keybinds.Back)) {
            NavPath.Remove();
            return;
        }

        if (Core.Input.CheckInput(Keybinds.Confirm, Keybinds.Map)) {
            NavPath.Remove();
            NavPath.Add(States.Inspect);
        }
    }
}
