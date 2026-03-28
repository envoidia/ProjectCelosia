using API.Battle.State;
using API.Input;

namespace API.Menu.State;

using static API.Input.InputPrompts;

public static class States
{
    public static readonly State MainMenu = new("Main", _MainMenuLib._Update,
    static () => State.GetInputPromptString(ScrollUpDown, Confirm));

    public static readonly State Battle = new("Battle", BattleLib._Update,
    static () =>
        State.GetInputPromptString(ScrollUpDown, Confirm, Back, InputPrompts.Log, Inspect))
    {
        OnCreate = BattleLib._Create, OnDestroy = BattleLib._Destroy
    };
}