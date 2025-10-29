using System;
using API.Input;

namespace API.Battle;

public class BattleHandlerLib {

    public static void HandleSetup() {

    }

    public static void CreateLog() {
        Core.NavPath.Push(Menu.MenuType.Log);
    }

    public static void HandleLog() {
        if(Core.Input.CheckInput(Keybind.Back, Keybind.Menu)) {
            Core.NavPath.Pop();
        }
    }

    public static void HandleTargeting() {
        if (Core.Input.CheckInput(Keybind.Back)) {
            Core.NavPath.Pop();
            return;
        }
    }

    public static void HandleInspect() {
        if (Core.Input.CheckInput(Keybind.Back)) {
            Core.NavPath.Pop();
            return;
        }
    }

    public static void CreateInspectTargeting() {
        Core.NavPath.Push(Menu.MenuType.InspectTargeting);
    }

    public static void HandleInspectTargeting() {
        if (Core.Input.CheckInput(Keybind.Back)) {
            Core.NavPath.Pop();
            return;
        }

        if (Core.Input.CheckInput(Keybind.Confirm, Keybind.Map)) {
            CreateInspect();
        }
    }
    
    public static void CreateInspect() {
        Core.NavPath.Pop();
        Core.NavPath.Push(Menu.MenuType.Inspect);
    }

    public static void HandleBattle() {
        
    }
}
