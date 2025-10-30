using API.Input;

namespace API.Battle;

public static class BattleHandlerLib {
    public static void HandleSetup() { }

    public static void CreateLog() {
        Core.AddMenu(Menu.MenuType.Log);
    }

    public static void HandleLog() {
        if (Core.Input.CheckInput(Keybind.Back, Keybind.Menu)) {
            Core.RemoveMenu();
        }
    }

    public static void HandleTargeting() {
        if (Core.Input.CheckInput(Keybind.Back)) {
            Core.RemoveMenu();
            return;
        }
    }

    public static void HandleInspect() {
        if (Core.Input.CheckInput(Keybind.Back)) {
            Core.RemoveMenu();
            return;
        }
    }

    public static void CreateInspectTargeting() {
        Core.AddMenu(Menu.MenuType.InspectTargeting);
    }

    public static void HandleInspectTargeting() {
        if (Core.Input.CheckInput(Keybind.Back)) {
            Core.RemoveMenu();
            return;
        }

        if (Core.Input.CheckInput(Keybind.Confirm, Keybind.Map)) {
            CreateInspect();
        }
    }

    public static void CreateInspect() {
        Core.RemoveMenu();
        Core.AddMenu(Menu.MenuType.Inspect);
    }

    public static void HandleBattle() { }
}