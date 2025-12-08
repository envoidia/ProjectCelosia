using API.Battle.State;
using API.Graphics;
using API.Input;

namespace API.Menu.State;

using static API.Input.InputPrompts;

public static class States {
    public static readonly State MainMenu = new("Main", () => { }, () => { }, _MainMenuLib._Update,
    static () => State.GetInputPromptString(ScrollUpDown, Faster, Confirm));

    public static readonly State Popup = new("Popup", _PopupLib._Create, _PopupLib._Destroy,
    static _ => {
        if (InputLib.Check(Keybinds.Confirm, Keybinds.Back)) {
            StateMachine.Remove();
            return;
        }
    },

static () => State.GetInputPromptString(Close));

    public static readonly State Battle = new("Battle", BattleLib._Create, BattleLib._Destroy, BattleLib._Update,
    static () =>
        State.GetInputPromptString(ScrollUpDown, Faster, Confirm,
            Back, InputPrompts.Log, InputPrompts.Inspect, InspectHere));

    public static readonly State Targeting = new("Targeting", () => { }, () => { }, _TargetingLib._Update,
    static () => State.GetInputPromptString(Move, Faster, Confirm, Back, InputPrompts.Log, InputPrompts.Inspect));

    public static readonly State Log = new("Log", () => { }, () => { },
        static _ => {
            if (InputLib.Check(Keybinds.Back, Keybinds.Menu)) StateMachine.Remove();
        },

        static () => State.GetInputPromptString(ScrollUpDown, Faster, Top, Bottom, BackLog));

    public static readonly State InspectTargeting = new("InspectTargeting", () => { }, () => { },
    _TargetingLib._UpdateInspectTargeting, static () =>
        State.GetInputPromptString(Move, Faster, ConfirmInspect, Back, InputPrompts.Log));

    public static readonly State Inspect = new("Inspect", _InspectLib._Create, _InspectLib._Destroy,
        _InspectLib._Update, _InspectLib._GetInputPrompt);
}