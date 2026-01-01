using System;
using API.Battle;
using API.Input;

namespace API.Menu;

public static class MenuLib
{
    private const float _LogScrollDelayS = 0.01f;

    public static int CheckMovement1D(int index, int optCount, SelectionType dir = SelectionType.HorizVert)
    {
        if (InputLib.Check(dir.GetDec(), true))
        {
            if (InputLib.Check(Keybinds.Hotkey2))
            {
                return 0;
            }

            return index == 0 ? optCount - 1 : index - 1;
        }

        if (InputLib.Check(dir.GetInc(), true))
        {
            if (InputLib.Check(Keybinds.Hotkey2))
            {
                return optCount - 1;
            }

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

    public static int CheckLogScroll(int logScroll, int lines, int off)
    {
        // Up
        if (InputLib.Check(Keybinds.Up, true, _LogScrollDelayS))
        {
            return Math.Min(++logScroll, Math.Max(lines - off, 0));
        }

        // Down
        if (InputLib.Check(Keybinds.Down, true, _LogScrollDelayS))
        {
            return Math.Max(--logScroll, 0);
        }

        // To top
        /*if (InputLib.Check(Keybinds.PageL2, false)) {
            return Math.Max(lines - off, 0);
        }

        // To bottom
        if (InputLib.Check(Keybinds.PageR2, false)) {
            return 0;
        }*/

        return logScroll;
    }
}