using System;
using API.Extensions;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

// todo cleanup
public sealed class GuiBoxChain(Stage stage, int l, int t, int b, params int[] divisions)
    : GuiBox(stage, l, -1, t, b, 2) {
    /// <summary>
    /// Width of each division (not counting the first)
    /// </summary>
    public int[] Divisions {
        get;
        set {
            field = value;
            this._selectedProg = new Progress[value.Length];
            this._selectedDir = new int[value.Length];
        }
    } = divisions;

    /// <summary>
    /// Currently selected division gets taller and has a highlight
    /// </summary>
    public int SelectedDiv { get; set; }

    /// <summary>
    /// Y offset
    /// </summary>
    // todo settable prop?
    private const int _SelectedOffset = 10;

    // todo settable prop?
    private Color _selectedColor = Colors.VPurple;

    // todo is it really uninitialized tho
    private Progress[] _selectedProg = new Progress[divisions.Length];
    private int[] _selectedDir = new int[divisions.Length];

    public override void Draw(GameTime gameTime) {
        if (this.Prog == 0) return;

        int divTotal = 0;
        for (int i = 0; i < this.Divisions.Length; i++) {
            int offset = (int) (this._selectedProg[i] * _SelectedOffset);

            int l = this.L + divTotal;
            divTotal += this.Divisions[i];
            int r = (this.L + divTotal) - (offset / 5);

            int t = this.T - offset;
            int b = this.B + offset;

            this._selectedDir[i] = (this.SelectedDiv == i).ToSign();
            this._selectedProg[i] =
                (Progress) Math.Clamp(
                    (float) this._selectedProg[i] + (float) (gameTime.ElapsedGameTime.TotalSeconds * this._selectedDir[i] *
                                                     (this.Speed * 2)), 0f, 1f);

            this._DrawInternal(l, r, t, b, this.Color);

            // Cursor
            Progress cursorProg = (Progress) Math.Min((float) this._selectedProg[i], (float) this.Prog);
            if (cursorProg != 0) this._DrawInternal(l, r, t, b, this._selectedColor, cursorProg);
        }
    }
}