using API.Battle.State;
using API.Input;

namespace API.Menu.State;

using static API.Input.InputPrompts;

public static class States {
    public static readonly State MainMenu = new("Main", _MainMenuLib._Update,
    static () => State.GetInputPromptString(ScrollUpDown, Faster, Jump, Confirm)) {
        //OnCreate = () => MainMenu!.Menus.Add(_MainMenuLib._MainMenu)
    };

    public static readonly State Battle = new("Battle", BattleLib._Update,
    static () =>
        State.GetInputPromptString(ScrollUpDown, Faster, Jump, Confirm, Back, InputPrompts.Log, InputPrompts.Inspect)) {
        OnCreate = BattleLib._Create, OnDestroy = BattleLib._Destroy
    };

    public static readonly State Targeting = new("Targeting", _TargetingLib._Update,
    static () => State.GetInputPromptString(Move, Faster, Jump, Confirm, Back, InputPrompts.Log, InputPrompts.Inspect));

    public static readonly State Log = new("Log",
        static _ => {
            if (InputLib.Check(Keybinds.Back, Keybinds.Menu1)) StateMachine.Remove();
        },

        static () => State.GetInputPromptString(ScrollUpDown, Faster, Jump, Top, Bottom, BackLog));

    public static readonly State InspectTargeting = new("InspectTargeting",
    _TargetingLib._UpdateInspectTargeting, static () =>
        State.GetInputPromptString(Move, Faster, Jump, ConfirmInspect, Back, InputPrompts.Log));

    public static readonly State Inspect = new("Inspect",
        _InspectLib._Update, _InspectLib._GetInputPrompt) {
        OnCreate = _InspectLib._Create, OnDestroy = _InspectLib._Destroy
    };
}