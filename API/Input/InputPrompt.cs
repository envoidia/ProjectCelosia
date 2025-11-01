using System.Text;
using API.Extensions;

namespace API.Input;

public class InputPrompt(string keyName, params Keybind[] keybinds) {
    public Keybind[] Keybinds { get; } = keybinds;

    public string GetText() {
        StringBuilder builder = new();

        foreach (Keybind keybind in this.Keybinds) {
            builder.Append(keybind.GetCurrentGlyph());
        }

        return builder.Append(' ').Append(keyName.GetLang()).ToString();
    }
}

public class InputPrompts {
    public static readonly InputPrompt Confirm = new("InputConfirm", Keybinds.Confirm);
    public static readonly InputPrompt Back = new("InputBack", Keybinds.Back);
    public static readonly InputPrompt BackLog = new("InputBack", Keybinds.Back, Keybinds.Menu);
    public static readonly InputPrompt Close = new("InputClose", Keybinds.Confirm, Keybinds.Back);

    public static readonly InputPrompt MoveLeftRight = new("InputMove", Keybinds.LeftRight);
    public static readonly InputPrompt MoveUpDown = new("InputMove", Keybinds.UpDown);
    public static readonly InputPrompt Move = new("InputMove", Keybinds.LeftRightUpDown);

    public static readonly InputPrompt Log = new("InputLog", Keybinds.Menu);
    public static readonly InputPrompt Inspect = new("InputInspect", Keybinds.Map);

    public static readonly InputPrompt Top = new("InputTop", Keybinds.PageL2);

    public static readonly InputPrompt Bottom = new("InputBottom", Keybinds.PageR2);
    // todo
}