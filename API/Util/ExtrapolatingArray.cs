using System;

namespace API.Util;

/// <summary>
/// <c>int[]</c> that can be indexed out of bounds, using step values to extrapolate results
/// </summary>
/// <param name="core">Core array</param>
/// <param name="offset">Indices to offset core by</param>
/// <param name="stepUp">Amount to increase value by per index past last</param>
/// <param name="stepDown">Amount to increase value by per index before first</param>
public class ExtrapolatingArray(int[] core, int offset, int stepUp, int stepDown) {
    public int this[int i] {
        get {
            // Real index
            int index = i + offset;

            int value;

            // In bounds
            if (index >= 0 && index < core.Length) {
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