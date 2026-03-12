using API.Menu.Widget;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

internal static class _MainMenuLib
{
    private static int _index;

    private enum _Options
    {
        Start,
        Encyclopedia,
        Options,
        Mods,
        Credits,
        Quit
    }

    private const int _OptCount = (int) _Options.Quit;

    // internal static readonly Menu _MainMenu = new("Main", new TabBarWidget(new(1000, 500),
    //     "lorem", "ipsum", "dolor", "si", "amet",
    //     "foo", "bar", "among", "us", "impostor", "is", "sus"),
    //     new TabBarWidget(new(1000, 700), "lorem", "ipsum", "dolor", "si", "amet"));

    // internal static readonly TabBarWidget TestT1 = new(new(1000, 1100), "aaa", "bbbbb", "cccccc", "wjkdhas") {
    //     CurDir = SelectionType.Horiz
    // };


    private static readonly ListWidget _TestLR = new(new(900, 500), true, "yes R", "foo", "bar",
    "baz", "lorem", "ipsum", "dolor", "si", "amet")
    {
        FixedWidth = 400
    };

    private static readonly ListWidget _TestLS = new(new(1500, 500), false, "yes S no R", "foo", "bar",
    "baz", "lorem", "ipsum", "dolor", "si", "amet")
    {
        Slant = -15
    };

    private static readonly ListWidget _TestLRS = new(new(2100, 500), true, "yes R yes S", "foo", "bar",
   "baz", "lorem", "ipsum", "dolor", "si", "amet")
    {
        FixedWidth = 400,
        Slant = -10
    };

    private static readonly Menu _MainMenu = new("Main", _TestLS, _TestLRS, _TestLR);

    internal static void _Init()
    {
        _TestLR.SetTextR("A", "B", "C", "D", "E", "F", "G", "H", "I");

        _TestLR.CalcLayout();
        _TestLS.CalcLayout();
        _TestLRS.CalcLayout();

        // todo how am i supposed to change state when theres a menu up
        StateMachine.State.AddMenu(_MainMenu);
    }

    internal static void _Update(GameTime gt)
    {
        //StateMachine.Add(States.Battle);

        // RenderLib.DrawParallelogram(new(1500, 800),
        //             new(1200, 800),
        //             Point.Zero, Colors.Bg,
        //             Colors.Fg, 15f, 6, 6, new Progress(Math.Min(1f, i / 2000f)));
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
            if ((_Options) _index == _Options.Quit) {Core.Instance.Exit();
            }
            _index = (int) _Options.Quit;
        }*/
    }
}
