using System;
using API.Battle.State;
using API.Extensions;
using API.Input;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

using static API.Input.InputPrompts;

public sealed class MenuMain : IState {
    private static int index;

    private enum Options {
        Start,
        Encyclopedia,
        Options,
        Mods,
        Credits,
        Quit,

        // Marker
        LastValue
    }

    private const int OptCountMain = (int) Options.LastValue - 1;

    public MenuMain() {
        if (Core.MenuMain is not null) {
            throw new InvalidOperationException("MultipleInstance".FormatLang(nameof(MenuMain)));
        }
    }

    public void Update(GameTime gameTime) {
        index = MenuLib.CheckMovement1D(index, OptCountMain);
        // todo update cursor

        if (Core.Input.CheckInput(Keybinds.Confirm)) {
            switch ((Options) index) {
                case Options.Start:
                    Core.NavPath.Add(Core.MenuBattle);
                    BattleHandler.Initialize();
                    BattleHandler.StartBattle();
                    return;
                case Options.Encyclopedia:
                    // todo
                    return;
                case Options.Options:
                    // todo
                    return;
                case Options.Mods:
                    // todo
                    return;
                case Options.Credits:
                    // todo
                    return;
                case Options.Quit:
                    // todo
                    return;
            }
        }

        if (Core.Input.CheckInput(Keybinds.Back)) {
            if ((Options) index == Options.Quit) {
                Core.sInstance.Exit();
            } else {
                index = (int) Options.Quit;
            }
        }
    }

    public void Draw(GameTime gameTime) {

    }

    public string GetInputPrompt() => IState.GetInputPromptString(MoveUpDown, Confirm);
}
