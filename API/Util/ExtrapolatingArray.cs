using System;

namespace API.Util;

public class ExtrapolatingArray(int[] core, int offset, int stepUp, int stepDown) {
    public int this[int i] {
        get {
            // Real index
            int index = i + offset;

            int value;

            // In bounds
            if ((index >= 0) && (index < core.Length)) {
                value = core[index];
            }
            // Above bounds
            else if (i >= core.Length) {
                value = core[^1] + ((stepUp * index) - (core.Length - 1));
            }
            // Below bounds
            else {
                value = core[0] + (stepDown * Math.Abs(index));
            }

            // Max to 0
            return Math.Max(value, 0);
        }
    }
}