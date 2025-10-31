namespace API.Input;

public class InputPrompts {
    public static readonly InputPrompt Confirm = new(Lang.InputConfirm, Keybind.Confirm);
    public static readonly InputPrompt Back = new(Lang.InputBack, Keybind.Back);
    public static readonly InputPrompt BackLog = new(Lang.InputBack, Keybind.Back, Keybind.Menu);
    public static readonly InputPrompt Close = new(Lang.InputClose, Keybind.Confirm, Keybind.Back);

    public static readonly InputPrompt MoveLeftRight = new(Lang.InputMove, Keybind.LeftRight);
    public static readonly InputPrompt MoveUpDown = new(Lang.InputMove, Keybind.UpDown);
    public static readonly InputPrompt Move = new(Lang.InputMove, Keybind.LeftRightUpDown);

    public static readonly InputPrompt Log = new(Lang.InputLog, Keybind.Menu);
    public static readonly InputPrompt Inspect = new(Lang.InputInspect, Keybind.Map);

    public static readonly InputPrompt Top = new(Lang.InputTop, Keybind.PageL2);

    public static readonly InputPrompt Bottom = new(Lang.InputBottom, Keybind.PageR2);
    // todo
}