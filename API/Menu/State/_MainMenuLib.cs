using API.Input;
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

    // internal static readonly Menu _MainMenu = new("Main", new TabBarWidget(new Vector2(1000, 500),
    //     "lorem", "ipsum", "dolor", "si", "amet",
    //     "foo", "bar", "among", "us", "impostor", "is", "sus"),
    //     new TabBarWidget(new Vector2(1000, 700), "lorem", "ipsum", "dolor", "si", "amet"));

    // internal static readonly TabBarWidget TestT1 = new(new Vector2(1000, 1100), "aaa", "bbbbb", "cccccc", "wjkdhas") {
    //     CurDir = SelectionType.Horiz
    // };

    internal static void _Update(GameTime gameTime) {
        StateMachine.Add(States.Battle);
        // RenderLib.DrawParallelogram(new Vector2(1500, 800),
        //             new Point(1200, 800),
        //             Point.Zero, Settings.ColorBg,
        //             Settings.ColorFg, 15f, 6, 6, new Progress(Math.Min(1f, i / 2000f)));
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
