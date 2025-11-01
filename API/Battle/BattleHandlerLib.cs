using API.Input;

namespace API.Battle;

public static class BattleHandlerLib {
    public static void HandleSetup() { }

    public static void CreateLog() {
        Core.AddMenu(Menu.MenuType.Log);
    }

    public static void HandleLog() {
        if (Core.Input.CheckInput(Keybinds.Back, Keybinds.Menu)) {
            Core.RemoveMenu();
        }
    }

    public static void HandleTargeting() {
        if (Core.Input.CheckInput(Keybinds.Back)) {
            Core.RemoveMenu();
            return;
        }
    }

    public static void HandleInspect() {
        if (Core.Input.CheckInput(Keybinds.Back)) {
            Core.RemoveMenu();
            return;
        }
    }

    public static void CreateInspectTargeting() {
        Core.AddMenu(Menu.MenuType.InspectTargeting);
    }

    public static void HandleInspectTargeting() {
        if (Core.Input.CheckInput(Keybinds.Back)) {
            Core.RemoveMenu();
            return;
        }

        if (Core.Input.CheckInput(Keybinds.Confirm, Keybinds.Map)) {
            CreateInspect();
        }
    }

    public static void CreateInspect() {
        Core.RemoveMenu();
        Core.AddMenu(Menu.MenuType.Inspect);
    }

    public static void HandleBattle() { }

    public static void AppendToLog(string str) {
        return;
    }
}