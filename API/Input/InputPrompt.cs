using System.Text;
using API.Extensions;

namespace API.Input;

public sealed class InputPrompt(string keyName, params Keybind[] keybinds) {
    public MultiInputType multiInputType = MultiInputType.Or;

    public string GetText() {
        StringBuilder builder = new();

        for (int i = 0; i < keybinds.Length; i++) {
            builder.Append(keybinds[i].GetCurrentGlyph());

            // Divider
            if (i != keybinds.Length - 1) builder.Append(this.multiInputType == MultiInputType.Or ? "//" : '+');
        }

        return builder.Append(' ').Append(keyName.GetLang()).ToString();
    }
}

// Possible inputs not noted by any InputPrompt:
// - Debug hotkeys (once you find F1, the rest are listed from there)
// - If the keys are available, you can use L/R in U/D menus, and vice-versa
public static class InputPrompts {
    public static readonly InputPrompt Confirm = new("InputConfirm", Keybinds.Confirm);
    public static readonly InputPrompt ConfirmInspect = new("InputConfirm", Keybinds.Confirm, Keybinds.Map);
    public static readonly InputPrompt Back = new("InputBack", Keybinds.Back);
    public static readonly InputPrompt BackLog = new("InputBack", Keybinds.Back, Keybinds.Menu);
    public static readonly InputPrompt Close = new("InputClose", Keybinds.Confirm, Keybinds.Back);

    public static readonly InputPrompt Move = new("InputMove", Keybinds.LeftRightUpDown);
    public static readonly InputPrompt MoveLeftRight = new("InputMove", Keybinds.LeftRight);
    public static readonly InputPrompt ScrollUpDown = new("InputScroll", Keybinds.UpDown);
    public static readonly InputPrompt Faster = new("InputFaster", Keybinds.Hotkey);

    public static readonly InputPrompt Log = new("InputLog", Keybinds.Menu);
    public static readonly InputPrompt Inspect = new("InputInspect", Keybinds.Map);
    public static readonly InputPrompt InspectHere = new("InputInspectHere", Keybinds.Map, Keybinds.Hotkey) {
        multiInputType = MultiInputType.And
    };

    public static readonly InputPrompt Top = new("InputTop", Keybinds.PageL2);
    public static readonly InputPrompt Bottom = new("InputBottom", Keybinds.PageR2);

    // Inspect menu
    public static readonly InputPrompt InspectStat = new("InputInspectStat", Keybinds.Menu);
    public static readonly InputPrompt InspectAffinity = new("InputInspectAffinity", Keybinds.Map);
    public static readonly InputPrompt InspectEquip = new("InputInspectEquip", Keybinds.Confirm);
    public static readonly InputPrompt InspectMult = new("InputInspectMult", Keybinds.PageL1);
    public static readonly InputPrompt InspectMod = new("InputInspectMod", Keybinds.PageR1);
    public static readonly InputPrompt InspectOther = new("InputInspectOther", Keybinds.Up, Keybinds.Down);
    public static readonly InputPrompt InspectUnitL = new("Blank", Keybinds.PageL2);
    public static readonly InputPrompt InspectUnitR = new("Blank", Keybinds.PageR2);
    public static readonly InputPrompt InspectPageL = new("Blank", Keybinds.Left);
    public static readonly InputPrompt InspectPageR = new("Blank", Keybinds.Right);
}

public enum MultiInputType {
    Or,
    And
}