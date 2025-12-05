using API.Battle.State;
using API.Input;

namespace API.Menu.State;

using static API.Input.InputPrompts;

public static class States {
    public static readonly State MainMenu = new("Main", MainMenuLib.Update, MainMenuLib.Draw,
    static () => State.GetInputPromptString(ScrollUpDown, Confirm));

    public static readonly State Popup = new("Popup",
    static _ => {
        if (InputLib.Check(Keybinds.Confirm, Keybinds.Back)) {
            NavPath.Remove();
            return;
        }
    },

    static gameTime => {
        // Draw the previous IState underneath
        NavPath.Path[^2].Draw(gameTime);

        Core.StagePopup.Draw(gameTime);
    },

static () => State.GetInputPromptString(Close));

    public static readonly State Battle = new("Battle", BattleLib.Update, Core.StageBattle.Draw,
    static () =>
        State.GetInputPromptString(ScrollUpDown, ScrollFaster, Confirm, Back, InputPrompts.Log, InputPrompts.Inspect)) {
        Create = BattleLib.StartBattle,
        Destroy = BattleLib.EndBattle
    };

    public static readonly State Targeting = new("Targeting", TargetingLib.Update, Core.StageBattle.Draw,
    static () => State.GetInputPromptString(Move, Confirm, Back, InputPrompts.Log));

    public static readonly State Log = new("Log",
        static _ => {
            BattleLib.HandleDebug();
            if (InputLib.Check(Keybinds.Back, Keybinds.Menu)) NavPath.Remove();
        },

        Core.StageBattle.Draw,
        static () => State.GetInputPromptString(ScrollUpDown, Top, Bottom, BackLog));

    public static readonly State InspectTargeting = new("InspectTargeting", TargetingLib.UpdateInspectTargeting,
    Core.StageBattle.Draw, static () => State.GetInputPromptString(Move, Confirm, Back, InputPrompts.Log));

    public static readonly State Inspect = new("Inspect", InspectLib.Update,
        static gameTime => {
            Core.StageBattle.Draw(gameTime);
            Core.StageInspect.Draw(gameTime);
        },

        static () => InspectLib.curPage == InspectLib._InspectPage.Stats
        ? State.GetInputPromptString(ScrollFaster, Back)
        : State.GetInputPromptString(ScrollUpDown, ScrollFaster, Back));
}