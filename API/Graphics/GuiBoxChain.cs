using System;
using API.Extensions;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public sealed class GuiBoxChain(int l, int t, int b, RenderPriority priority = RenderPriority.B1Med,
    params int[] divisions) : GuiBox(l, -1, t, b, priority: priority) {
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
            this._selectedProg[i] = (Progress) Math.Clamp((float) this._selectedProg[i] +
                (float) (gameTime.ElapsedGameTime.TotalSeconds * this._selectedDir[i] * (this.Speed * 2)), 0f, 1f);

            draw(l, r, t, b);

            // Cursor
            Progress cursorProg = (Progress) Math.Min((float) this._selectedProg[i], (float) this.Prog);
            if (cursorProg != 0) drawP(l, r, t, b, this._selectedColor, cursorProg);

            void drawP(int l, int r, int t, int b, Color color, Progress prog) =>
                RenderLib.DrawParallelogram(l, r, t, b, color, this.OutlineColor,
                    this.OutlineThickness, this.SlantL, this.SlantR, prog);

            void draw(int l, int r, int t, int b) => drawP(l, r, t, b, this.Color, this.Prog);
        }
    }

    public void AddRoutine(Routine routine) => this.Data.AddRoutine(routine);
    public void MarkForRemoval() => this.Data.MarkForRemoval();
}