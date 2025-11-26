using System;
using API.Extensions;
using API.Input;
using API.Menu.State;
using Microsoft.Xna.Framework;
using static API.Battle.State.BattleLib;

namespace API.Battle.State;

// Significant using order
using static API.Input.InputPrompts;

public sealed class MenuInspectTargeting : IState {

    public MenuInspectTargeting() {
        if (Core.MenuInspectTargeting is not null) {
            throw new InvalidOperationException(string.Format(Lang.MultipleInstance, nameof(MenuInspectTargeting)));
        }
    }

    public void Update(GameTime gameTime) {
        HandleDebug();

        if (Core.Input.CheckInput(Keybinds.Back)) {
            Core.NavPath.Remove();
            return;
        }

        if (Core.Input.CheckInput(Keybinds.Confirm, Keybinds.Map)) {
            Core.NavPath.Remove();
            Core.NavPath.Add(Core.MenuInspect);
        }
    }

    public void Draw(GameTime gameTime) {
        Core.StageBattle.Draw(gameTime);
    }

    public string GetInputPrompt() => IState.GetInputPromptString(Move, Confirm, Back, Log);

    private static void CreateInspectTargeting() => Core.NavPath.Add(Core.MenuInspectTargeting);
}
