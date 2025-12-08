using API.Graphics;
using API.Input;
using API.Menu;
using API.Menu.State;
using Microsoft.Xna.Framework;

namespace API.Battle.State;

// Significant using order
using static API.Battle.State.BattleLib;


internal static class _TargetingLib {
    /// <summary>
    /// How many extra actions have been used for the currently acting Unit
    /// </summary>
    private static int _extraActions = 0;

    internal static void _Update(GameTime gameTime) {
        if (InputLib.Check(Keybinds.Menu)) {
            StateMachine.Add(States.Log);
            return;
        }

        if (InputLib.Check(Keybinds.Map)) {
            StateMachine.Add(States.Inspect);
            return;
        }

        if (InputLib.Check(Keybinds.Back)) {
            //foreach (Label stat in stats) stat.Color = Colors.White;
            _Moves[_selectingMove].Text = "";

            StateMachine.Remove();
            return;
        }

        _indexTarget = MenuLib.CheckMovementTargeting(_indexTarget, _selectingMove, _selectedSkillInstance.Skill.Range);

        //MenuLib.handleOptColor(stats, indexTarget);

        if (!InputLib.Check(Keybinds.Confirm)) return;

        Unit self = Battle.PlayerTeam.Units[_selectingMove];
        Unit target = _indexTarget < PosLib.LowestOpp
            ? Battle.PlayerTeam.Units[_indexTarget]
            : Battle.OpponentTeam.Units[_indexTarget - PosLib.LowestOpp];
        _CurMoves.Add(new Move(_selectedSkillInstance, self, target.Pos));
        // todo support ExA
        _Moves[_selectingMove].Text = $"{_Moves[_selectingMove].Text} /c[white]→ {target.FormatName(false)}";

        foreach (Label stat in _Stats) {
            //stat.Color = Colors.White;
        }

        // Move on to next Unit unless this one has extra actions
        if (_extraActions < self.ExtraActions) {
            _extraActions++;
        } else {
            _extraActions = 0;
            _selectingMove++;
        }

        _indexSkill = 0;

        _UpdateStatDisplay(_selectingMove);

        StateMachine.Remove();
    }

    // todo merge
    internal static void _UpdateInspectTargeting(GameTime gameTime) {
        if (InputLib.Check(Keybinds.Back)) {
            StateMachine.Remove();
            return;
        }

        if (InputLib.Check(Keybinds.Confirm, Keybinds.Map)) {
            StateMachine.Remove();
            StateMachine.Add(States.Inspect);
        }
    }
}
