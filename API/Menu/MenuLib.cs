using System;
using API.Input;

namespace API.Menu;

public static class MenuLib {
    private static readonly TimeSpan LogScrollDelay = TimeSpan.FromSeconds(0.005f);

    public static int CheckMovement1D(int index, int optCount) {
        if (Core.Input.CheckInput(true, Keybind.Up, Keybind.Left)) {
            return --index < 0 ? optCount - 1 : index;
        }

        if (Core.Input.CheckInput(true, Keybind.Down, Keybind.Right)) {
            return ++index >= optCount ? 0 : index;
        }

        return Math.Min(index, optCount - 1);
    }

    public static int CheckMovement1D(int index, int optCount, Keybind dec, Keybind inc) {
        if (Core.Input.CheckInput(true, dec)) {
            return --index < 0 ? optCount - 1 : index;
        }

        if (Core.Input.CheckInput(true, inc)) {
            return ++index >= optCount ? 0 : index;
        }

        return Math.Min(index, optCount - 1);
    }

    public static int CheckMovementTargeting(int index, int selectingMove /*, Range range*/) {
        // Lock cursor to self for self Ranges
        /*todo if (range == Ranges.SELF || range == Ranges.SELF_UP_DOWN) {
            return selectingMove;
        }*/

        int newIndex = index;

        // Move selection
        if (Core.Input.CheckInput(true, Keybind.Up)) {
            if (index < 4) {
                // On player side
                newIndex = (index - 1) < 0 ? 3 : index - 1;
            } else {
                newIndex = (index - 1) < 4 ? 7 : index - 1;
            }
        } else if (Core.Input.CheckInput(true, Keybind.Down)) {
            if (index < 4) {
                // On player side
                newIndex = (index + 1) >= 4 ? 0 : index + 1;
            } else {
                newIndex = (index + 1) >= 8 ? 4 : index + 1;
            }
        } else if (Core.Input.CheckInput(true, Keybind.Left, Keybind.Right)) {
            newIndex = index < 4 ? index + 4 : index - 4;
        }

        // Lock cursor to valid side
        /* if (range.side() == Side.BOTH) {
             return newIndex;
         }

         if (range.side() == getRelativeSide(selectingMove, newIndex)) {
             return newIndex;
         }*/

        return index;
    }

    public static int CheckLogScroll(int logScroll, int lines, int off) {
        // Up
        if (Core.Input.CheckInput(true, LogScrollDelay, Keybind.Up)) {
            return Math.Min(++logScroll, Math.Max(lines - off, 0));
        }

        // Down
        if (Core.Input.CheckInput(true, LogScrollDelay, Keybind.Down)) {
            return Math.Max(--logScroll, 0);
        }

        // To top
        if (Core.Input.CheckInput(false, Keybind.PageL2)) {
            return Math.Max(lines - off, 0);
        }

        // To bottom
        if (Core.Input.CheckInput(false, Keybind.PageR2)) {
            return 0;
        }

        return logScroll;
    }
}