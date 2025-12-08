using System;
using API.Battle.State;
using API.Graphics;
using API.Input;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

public static class MainMenuLib {
    private static int _index;

    // temp
    private static readonly GuiBox gbTest = new(Stages.Super, 500, 1000, 500, 1000);

    private enum _Options {
        Start,
        Encyclopedia,
        Options,
        Mods,
        Credits,
        Quit
    }

    private const int _OptCount = (int) _Options.Quit;

    public static void Update(GameTime gameTime) {
        _index = MenuLib.CheckMovement1D(_index, _OptCount);
        // todo update cursor

        //temp testing
        if (InputLib.IsKeyJustPressed(Microsoft.Xna.Framework.Input.Keys.Q)) {
            gbTest.AddRoutine((gbTest.Prog == 1) ? IAnimatedPrimitive.Collapse : IAnimatedPrimitive.Unfold);
        }


        if (InputLib.Check(Keybinds.Confirm)) {
            switch ((_Options) _index) {
                case _Options.Start:
                    BattleLib.Initialize();
                    NavPath.Add(States.Battle);
                    return;
                case _Options.Encyclopedia:
                    // todo
                    return;
                case _Options.Options:
                    // todo
                    return;
                case _Options.Mods:
                    // todo
                    return;
                case _Options.Credits:
                    // todo
                    return;
                case _Options.Quit:
                    // todo
                    return;
            }
        }

        if (InputLib.Check(Keybinds.Back)) {
            if ((_Options) _index == _Options.Quit) {
                Core.Instance.Exit();
            } else {
                _index = (int) _Options.Quit;
            }
        }
    }

    // todo
    public static void Draw(GameTime gameTime) { return; }
}
