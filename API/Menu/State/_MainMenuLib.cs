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

    internal static void _Init()
    {
        // todo how am i supposed to change state when theres a menu up
        // StateMachine.State.AddMenu(Debug.UiTestLib.Menu);
    }

    internal static void _Update(GameTime gt)
    {
        StateMachine.Add(States.Battle);
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
