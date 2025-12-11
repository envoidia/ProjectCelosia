using API.Graphics;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

internal static class _MainMenuLib {
    private static int _index;

    private enum _Options {
        Start,
        Encyclopedia,
        Options,
        Mods,
        Credits,
        Quit
    }

    private const int _OptCount = (int) _Options.Quit;

    internal static readonly Menu _MainMenu = new(new ListWidget("lorem", "ipsum", "dolor", "si", "amet",
        "foo", "bar", "among", "us", "impostor", "is", "sus") {
        Position = new Vector2(1200, 800)
    });

    private static readonly Label _L = new() {
        Position = new Vector2(2000, 400),
        Text = "awawawawawawawaAAAAAAAAAAAAAAAAAAAAA",
        Padding = new(30, 30, 30, 30)
    };

    static _MainMenuLib() {
        Stage.Add(_L);
    }

    internal static void _Update(GameTime gameTime) {
        RenderLib.DrawParallelogram(new Vector2(1600, 400),
                    new Point(400, 60),
                    Point.Zero, Settings.ColorAccent,
                    Settings.ColorAccent, 0f, 6, 6, new Progress(1f));
        /*_index = MenuLib.CheckMovement1D(_index, _OptCount);
        // todo update cursor

        if (InputLib.Check(Keybinds.Confirm)) {
            switch ((_Options) _index) {
                case _Options.Start:
                    StateMachine.Add(States.Battle);
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
            if ((_Options) _index == _Options.Quit) Core.Instance.Exit();
            _index = (int) _Options.Quit;
        }*/
    }
}
