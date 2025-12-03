using System;
using API.Battle;
using API.Battle.State;
using API.Input;

namespace API.Menu.State;

using static API.Input.InputPrompts;

public static class States {
    public static readonly State MainMenu = new("Main") {
        Update = MainMenuLib.Update,
        Draw = MainMenuLib.Draw,
        GetInputPrompt = static () => State.GetInputPromptString(ScrollUpDown, Confirm)
    };

    public static readonly State Popup = new("Popup") {
        Update = static _ => {
            if (Core.Input.CheckInput(Keybinds.Back)) {
                NavPath.Remove();
                return;
            }
        },

        Draw = static gameTime => {
            // Draw the previous IState underneath
            NavPath.Path[^2].Draw(gameTime);

            // Draw popup
            Core.StagePopup.Draw(gameTime);
        },

        GetInputPrompt = static () => State.GetInputPromptString(Close)
    };

    public static readonly State Battle = new("Battle") {
        Create = BattleLib.StartBattle,
        Destroy = BattleLib.EndBattle,
        Update = BattleLib.Update,
        Draw = Core.StageBattle.Draw,

        GetInputPrompt = static () =>
            State.GetInputPromptString(ScrollUpDown, ScrollFaster, Confirm, Back, InputPrompts.Log, InputPrompts.Inspect)
    };

    public static readonly State Targeting = new("Targeting") {
        Update = TargetingLib.Update,
        Draw = Core.StageBattle.Draw,
        GetInputPrompt = static () => State.GetInputPromptString(Move, Confirm, Back, InputPrompts.Log)
    };

    public static readonly State Log = new("Log") {
        Update = static _ => {
            BattleLib.HandleDebug();
            if (Core.Input.CheckInput(Keybinds.Back, Keybinds.Menu)) NavPath.Remove();
        },

        Draw = Core.StageBattle.Draw,
        GetInputPrompt = static () => State.GetInputPromptString(ScrollUpDown, Top, Bot tom, BackLog)
    };

    public static readonly State InspectTargeting = new("InspectTargeting") {
        Update = TargetingLib.UpdateInspectTargeting,
        Draw = Core.StageBattle.Draw,
        GetInputPrompt = static () => State.GetInputPromptString(Move, Confirm, Back, InputPrompts.Log)
    };

    public static readonly State Inspect = new("Inspect") {
        Update = InspectLib.Update,

        Draw = static gameTime => {
            Core.StageBattle.Draw(gameTime);
            Core.StageInspect.Draw(gameTime);
        },

        GetInputPrompt = () => InspectLib.curPage == InspectLib.InspectPage.Stats
        ? State.GetInputPromptString(ScrollFaster, Back)
        : State.GetInputPromptString(ScrollUpDown, ScrollFaster, Back)
    };
}