using API.Battle.State;
using API.Graphics;
using API.Input;

namespace API.Menu.State;

using static API.Input.InputPrompts;

public static class States {
    public static readonly State MainMenu = new("Main", () => { }, () => { }, MainMenuLib._Update,
    static () => State.GetInputPromptString(ScrollUpDown, Confirm));

    public static readonly State Popup = new("Popup", PopupLib._Create, PopupLib._Destroy,
    static _ => {
        if (InputLib.Check(Keybinds.Confirm, Keybinds.Back)) {
            StateMachine.Remove();
            return;
        }
    },

static () => State.GetInputPromptString(Close));

    public static readonly State Battle = new("Battle", BattleLib._Create, BattleLib._Destroy, BattleLib._Update,
    static () =>
        State.GetInputPromptString(ScrollUpDown, ScrollFaster, Confirm, Back, InputPrompts.Log, InputPrompts.Inspect));

    public static readonly State Targeting = new("Targeting", () => { }, () => { }, TargetingLib.Update,
    static () => State.GetInputPromptString(Move, Confirm, Back, InputPrompts.Log));

    public static readonly State Log = new("Log", () => { }, () => { },
        static _ => {
            if (InputLib.Check(Keybinds.Back, Keybinds.Menu)) StateMachine.Remove();
        },

        static () => State.GetInputPromptString(ScrollUpDown, Top, Bottom, BackLog));

    public static readonly State InspectTargeting = new("InspectTargeting", () => { }, () => { },
    TargetingLib.UpdateInspectTargeting, static () => State.GetInputPromptString(Move, Confirm, Back, InputPrompts.Log));

    public static readonly State Inspect = new("Inspect", InspectLib._Create, InspectLib._Destroy,
        InspectLib._Update, InspectLib._GetInputPrompt);
}