using System;
using API.Input;
using API.Menu;
using Microsoft.Xna.Framework;
using static API.Battle.BattleHandlerLib;

namespace API.Battle;

public static class BattleHandler {
    public static TimeSpan Delay { get; private set; }

    public static void Init() {
        HandleSetup();
    }

    public static void Input(GameTime gameTime) {
        MenuType curMenu = Core.NavPath.Peek();

        if (curMenu == MenuType.Log) {
            HandleLog();
        } else if (curMenu == MenuType.Inspect) {
            HandleInspect();
        } else if (Core.Input.CheckInput(Keybinds.Menu)) {
            CreateLog();
        } else if (curMenu == MenuType.InspectTargeting) {
            HandleInspectTargeting();
        } else if (curMenu == MenuType.Targeting) {
            HandleTargeting();
        } else if (Core.Input.CheckInput(Keybinds.Map)) {
            CreateInspectTargeting();
        } else if (Delay > TimeSpan.Zero) {
            Delay -= gameTime.ElapsedGameTime;
        } else if (curMenu == MenuType.Battle) HandleBattle();
    }
}