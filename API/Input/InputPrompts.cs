namespace API.Input;

public class InputPrompts {
    public static readonly InputPrompt Confirm = new("input.confirm", Keybind.Confirm);
    public static readonly InputPrompt Back = new("input.back", Keybind.Back);
    public static readonly InputPrompt Close = new("input.close", Keybind.Confirm, Keybind.Back);

    public static readonly InputPrompt MoveLeftRight = new("input.moveleftright", Keybind.LeftRight);
    // todo
}