using System;
using API.Battle;
using API.Input;

namespace API.Menu;

public static class MenuLib {
    private static readonly TimeSpan LogScrollDelay = TimeSpan.FromSeconds(0.005f);

    // todo fix uint fuckery
    public static uint CheckMovement1D(uint index, uint optCount) {
        if (Core.Input.CheckInput(true, Keybinds.Up, Keybinds.Left)) {
            return index == 0 ? optCount - 1 : index - 1;
        }

        if (Core.Input.CheckInput(true, Keybinds.Down, Keybinds.Right)) {
            return index == (optCount - 1) ? 0 : index + 1;
        }

        return Math.Min(index, optCount - 1);
    }

    public static uint CheckMovement1D(uint index, uint optCount, Keybind dec, Keybind inc) {
        if (Core.Input.CheckInput(true, dec)) {
            return index == 0 ? optCount - 1 : index - 1;
        }

        if (Core.Input.CheckInput(true, inc)) {
            return index == (optCount - 1) ? 0 : index + 1;
        }

        return Math.Min(index, optCount - 1);
    }

    public static uint CheckMovementTargeting(uint index, uint selectingMove, Battle.Range range) {
        // Lock cursor to self for self Ranges
        if ((range == Ranges.Self) || (range == Ranges.SelfUpDown)) {
            return selectingMove;
        }

        int indexI = (int) index;
        uint newIndex = index;

        // Move selection
        if (Core.Input.CheckInput(true, Keybinds.Up)) {
            if (index < 4) {
                // On player side
                newIndex = (indexI - 1) < 0 ? 3 : index - 1;
            } else {
                newIndex = (indexI - 1) < 4 ? 7 : index - 1;
            }
        } else if (Core.Input.CheckInput(true, Keybinds.Down)) {
            if (index < 4) {
                // On player side
                newIndex = (indexI + 1) >= 4 ? 0 : index + 1;
            } else {
                newIndex = (indexI + 1) >= 8 ? 4 : index + 1;
            }
        } else if (Core.Input.CheckInput(true, Keybinds.Left, Keybinds.Right)) {
            newIndex = indexI < 4 ? index + 4 : index - 4;
        }

        // Lock cursor to valid side
        if ((range.Side == Side.Both) || (range.Side == PosLib.GetRelativeSide(selectingMove, newIndex))) {
            return newIndex;
        }

        return index;
    }

    public static uint CheckLogScroll(uint logScroll, uint lines, uint off) {
        // Up
        if (Core.Input.CheckInput(true, LogScrollDelay, Keybinds.Up)) {
            return Math.Min(++logScroll, Math.Max(lines - off, 0));
        }

        // Down
        if (Core.Input.CheckInput(true, LogScrollDelay, Keybinds.Down)) {
            return Math.Max(--logScroll, 0);
        }

        // To top
        if (Core.Input.CheckInput(false, Keybinds.PageL2)) {
            return Math.Max(lines - off, 0);
        }

        // To bottom
        if (Core.Input.CheckInput(false, Keybinds.PageR2)) {
            return 0;
        }

        return logScroll;
    }
}