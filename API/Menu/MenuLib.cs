using System;
using API.Battle;
using API.Battle.State;
using API.Graphics;
using API.Input;
using API.Menu.Widget;
using API.Save;

namespace API.Menu;

public static class MenuLib
{
    private const float _LogScrollDelayS = 0.01f;

    public static int CheckMovement1D(int index, int optCount, SelectionType dir = SelectionType.HorizVert)
    {

        if (InputLib.Check(dir.GetDec(), true))
        {
            return index == 0 ? optCount - 1 : index - 1;
        }

        if (InputLib.Check(dir.GetInc(), true))
        {
            return index == (optCount - 1) ? 0 : index + 1;
        }

        return Math.Min(index, optCount - 1);
    }

    /// <param name="logScroll">Amount of lines scrolled down? probably?</param>
    /// <param name="lines">Total lines? i think?</param>
    /// <param name="off">Amount of lines to show? maybe?</param>
    // todo better docs
    public static int CheckLogScroll(int logScroll, int lines, int off)
    {
        // Up
        if (InputLib.Check(Keybinds.Up, true, _LogScrollDelayS))
        {
            return Math.Min(logScroll + 1, Math.Max(lines - off, 0));
        }

        // Down
        if (InputLib.Check(Keybinds.Down, true, _LogScrollDelayS))
        {
            return Math.Max(logScroll - 1, 0);
        }

        // To top/bottom
        if (InputLib.Check(Keybinds.Confirm, true))
        {
            return logScroll == 0 ? Math.Max(lines - off, 0) : 0;
        }

        return logScroll;
    }
}
