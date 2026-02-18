using System;
using API.Battle;
using API.Input;
using API.Menu.Widget;

namespace API.Menu;

public static class MenuLib
{
    private const float _LogScrollDelayS = 0.01f;

    public static int CheckMovement1D(int index, int optCount, SelectionType dir = SelectionType.HorizVert)
    {
        if (InputLib.CheckRaw(Keybinds.Hotkey2))
        {
            return 0;
        }

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

    public static int CheckMovementTargeting(int index, int selectingMove, Battle.Range range)
    {
        // Lock cursor to self for self Ranges
        if ((range == Ranges.Self) || (range == Ranges.SelfUpDown))
        {
            return selectingMove;
        }

        int indexI = index;
        int newIndex = index;

        // Move selection
        if (InputLib.Check(Keybinds.Up, true))
        {
            if (index < PosLib.LowestOpp)
            {
                // On player side
                newIndex = (indexI - 1) < 0 ? PosLib.HighestAlly : index - 1;
            }
            else
            {
                newIndex = (indexI - 1) < PosLib.LowestOpp ? PosLib.HighestOpp : index - 1;
            }
        }
        else if (InputLib.Check(Keybinds.Down, true))
        {
            if (index < PosLib.LowestOpp)
            {
                // On player side
                newIndex = (indexI + 1) >= PosLib.LowestOpp ? 0 : index + 1;
            }
            else
            {
                newIndex = (indexI + 1) > PosLib.HighestOpp ? PosLib.LowestOpp : index + 1;
            }
        }
        else if (InputLib.Check(Keybinds.Left, Keybinds.Right, true))
        {
            newIndex = indexI < PosLib.LowestOpp ? index + PosLib.LowestOpp : index - PosLib.LowestOpp;
        }

        // Lock cursor to valid side
        if ((range.Side == Side.Both) || (range.Side == PosLib.GetRelativeSide(selectingMove, newIndex)))
        {
            return newIndex;
        }

        return index;
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
        // todo test
        if (InputLib.Check(Keybinds.Hotkey2, true))
        {
            return logScroll == lines - off ? Math.Max(lines - off, 0) : 0;
        }

        return logScroll;
    }
}