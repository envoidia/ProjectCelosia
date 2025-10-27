using System;

namespace API.Util;

public record ExtrapolatingArray(int[] Core, int Offset, int StepUp, int StepDown) {
    public int Get(int index) {
        // Real index
        int i = index + this.Offset;

        int value;

        // In bounds
        if ((i >= 0) && (i < this.Core.Length)) {
            value = this.Core[i];
        }
        // Above bounds
        else if (i >= this.Core.Length) {
            value = this.Core[^1] + ((this.StepUp * i) - (this.Core.Length - 1));
        }
        // Below bounds
        else {
            value = this.Core[0] + (this.StepDown * Math.Abs(i));
        }

        // Max to 0
        return Math.Max(value, 0);
    }
}