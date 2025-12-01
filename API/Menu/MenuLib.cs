using System;
using API.Battle;
using API.Input;

namespace API.Menu;

public static class MenuLib {
    private const float LogScrollDelayS = 0.01f;

    public static int CheckMovement1D(int index, int optCount) {
        if (Core.Input.CheckInput(Keybinds.Up, Keybinds.Left, true)) {
            return index == 0 ? optCount - 1 : index - 1;
        }

        if (Core.Input.CheckInput(Keybinds.Down, Keybinds.Right, true)) {
            return index == (optCount - 1) ? 0 : index + 1;
        }

        return Math.Min(index, optCount - 1);
    }

    public static int CheckMovement1D(int index, int optCount, Keybind dec, Keybind inc) {
        if (Core.Input.CheckInput(dec, true)) {
            return index == 0 ? optCount - 1 : index - 1;
        }

        if (Core.Input.CheckInput(inc, true)) {
            return index == (optCount - 1) ? 0 : index + 1;
        }

        return Math.Min(index, optCount - 1);
    }

    public static int CheckMovementTargeting(int index, int selectingMove, Battle.Range range) {
        // Lock cursor to self for self Ranges
        if ((range == Ranges.Self) || (range == Ranges.SelfUpDown)) {
            return selectingMove;
        }

        int indexI = index;
        int newIndex = index;

        // Move selection
        if (Core.Input.CheckInput(Keybinds.Up, true)) {
            if (index < PosLib.LowestOpp) {
                // On player side
                newIndex = (indexI - 1) < 0 ? PosLib.HighestAlly : index - 1;
            } else {
                newIndex = (indexI - 1) < PosLib.LowestOpp ? PosLib.HighestOpp : index - 1;
            }
        } else if (Core.Input.CheckInput(Keybinds.Down, true)) {
            if (index < PosLib.LowestOpp) {
                // On player side
                newIndex = (indexI + 1) >= PosLib.LowestOpp ? 0 : index + 1;
            } else {
                newIndex = (indexI + 1) > PosLib.HighestOpp ? PosLib.LowestOpp : index + 1;
            }
        } else if (Core.Input.CheckInput(Keybinds.Left, Keybinds.Right, true)) {
            newIndex = indexI < PosLib.LowestOpp ? index + PosLib.LowestOpp : index - PosLib.LowestOpp;
        }

        // Lock cursor to valid side
        if ((range.Side == Side.Both) || (range.Side == PosLib.GetRelativeSide(selectingMove, newIndex))) {
            return newIndex;
        }

        return index;
    }

    public static int CheckLogScroll(int logScroll, int lines, int off) {
        // Up
        if (Core.Input.CheckInput(Keybinds.Up, true, LogScrollDelayS)) {
            return Math.Min(++logScroll, Math.Max(lines - off, 0));
        }

        // Down
        if (Core.Input.CheckInput(Keybinds.Down, true, LogScrollDelayS)) {
            return Math.Max(--logScroll, 0);
        }

        // To top
        if (Core.Input.CheckInput(Keybinds.PageL2, false)) {
            return Math.Max(lines - off, 0);
        }

        // To bottom
        if (Core.Input.CheckInput(Keybinds.PageR2, false)) {
            return 0;
        }

        return logScroll;
    }
}