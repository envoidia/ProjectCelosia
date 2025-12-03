using System;
using API.Battle.State;
using API.Extensions;
using API.Input;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

using static API.Input.InputPrompts;

public static class MainMenuLib {
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

    public static void Update(GameTime gameTime) {
        index = MenuLib.CheckMovement1D(index, OptCountMain);
        // todo update cursor

        if (Core.Input.CheckInput(Keybinds.Confirm)) {
            switch ((Options) index) {
                case Options.Start:
                    BattleLib.Initialize();
                    NavPath.Add(States.Battle);
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

    // todo
    public static void Draw(GameTime gameTime) { return; }
}
