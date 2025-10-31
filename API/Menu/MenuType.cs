using System.Text;
using API.Input;

namespace API.Menu;

using static MenuType;
using static InputPrompts;

public enum MenuType {
    None,
    Main,
    Popup,
    Battle,
    Targeting,
    Log,
    InspectTargeting,
    Inspect,
    Debug
}

public static class MenuTypeExtensions {
    extension(MenuType menuType) {
        public string GetInputPrompt() => GetInputPromptString(menuType switch {
            Main => [MoveUpDown, Confirm],
            Popup => [Close],
            Battle => [MoveUpDown, Confirm, Back, InputPrompts.Log, InputPrompts.Inspect],
            Targeting or InspectTargeting => [Move, Confirm, Back, InputPrompts.Log],
            MenuType.Log => [MoveUpDown, Top, Bottom, BackLog],
            MenuType.Inspect => [Back],
            _ => []
        });
    }

    private static string GetInputPromptString(params InputPrompt[] inputPrompts) {
        if (inputPrompts == null) return "";

        StringBuilder inputs = new();

        for (int i = 0; i < inputPrompts.Length; i++) {
            inputs.Append(inputPrompts[i].GetText());
            if (i != (inputPrompts.Length - 1)) inputs.Append("  ");
        }

        return inputs.ToString();
    }
}