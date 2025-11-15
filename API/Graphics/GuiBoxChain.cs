using System;
using API.Extensions;
using Microsoft.Xna.Framework;

namespace API.Graphics;

// todo cleanup
public class GuiBoxChain(int l, int r, int t, int b, params int[] divisions) : GuiBox(l, r, t, b, 2) {
    // Width of each division (not counting the first)
    public int[] Divisions {
        get;
        set {
            field = value;
            this._selectedProg = new float[value.Length];
            this._selectedDir = new int[value.Length];
        }
    } = divisions;

    // Currently selected division gets taller and has a highlight
    public int SelectedDiv { get; set; }

    // Y offset
    // todo can this be optional primary con param
    private int _selectedOffset = 10;

    private Color _selectedColor = Colors.VPurple;

    // todo is it really uninitialized tho
    private float[] _selectedProg = new float[divisions.Length];
    private int[] _selectedDir = new int[divisions.Length];

    public override void Draw(GameTime gameTime) {
        this.Update(gameTime);

        if (this.Prog == 0) return;

        int divTotal = 0;
        for (int i = 0; i < this.Divisions.Length; i++) {
            int offset = (int) (this._selectedOffset * this._selectedProg[i]);

            int l = this.L + divTotal;
            divTotal += this.Divisions[i];
            int r = (this.L + divTotal) - (offset / 5);

            int t = this.T - offset;
            int b = this.B + offset;

            this._selectedDir[i] = (this.SelectedDiv == i).ToSign();
            this._selectedProg[i] =
                Math.Clamp(
                    this._selectedProg[i] + (float) (gameTime.ElapsedGameTime.TotalSeconds * this._selectedDir[i] *
                                                     (this.Speed * 2)), 0f, 1f);

            this.DrawWithoutUpdate(l, r, t, b, this.Color);

            // Cursor
            float cursorProg = Math.Min(this._selectedProg[i], this.Prog);
            if (cursorProg != 0) this.DrawWithoutUpdate(l, r, t, b, this._selectedColor, cursorProg);
        }
    }
}