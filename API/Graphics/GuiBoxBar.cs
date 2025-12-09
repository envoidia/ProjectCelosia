using System;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public sealed class GuiBoxBar(int l, int r, int t, int b, params Color[] colors) : IActor, IAnimatedPrimitive {
    public Progress[] BarProgs { get; set; } = new Progress[colors.Length];
    public Color[] BarColors { get; } = colors;

    #region Underlying

    /// <summary>
    /// Underlying <c>GuiBox</c>
    /// </summary>
    public GuiBox GuiBox { get; } = new(l, r, t, b);

    public ActorData Data { get => this.GuiBox.Data; }

    /// <inheritdoc cref="ActorData.Priority" />
    public RenderPriority Priority {
        get => this.GuiBox.Priority;
        set => this.GuiBox.Priority = value;
    }

    public Progress Prog {
        get => this.GuiBox.Prog;
        set => this.GuiBox.Prog = value;
    }
    public float Speed {
        get => this.GuiBox.Speed;
        set => this.GuiBox.Speed = value;
    }

    #endregion

    public void Draw(GameTime gameTime) {
        if (this.GuiBox.Prog == 0) return;

        draw(this.GuiBox.L, this.GuiBox.R, this.GuiBox.T, this.GuiBox.B, this.GuiBox.Color);

        // Draw overlay bars from longest to shortest so they're all visible
        Array.Sort(this.BarProgs, this.BarColors);

        for (int i = this.BarProgs.Length - 1; i >= 0; i--) {
            draw(this.GuiBox.L, (int) (this.GuiBox.L + ((this.GuiBox.R - this.GuiBox.L) *
                Math.Min((float) this.BarProgs[i], (float) this.Prog))), this.GuiBox.T, this.GuiBox.B,
                this.BarColors[i]);
        }

        void draw(int l, int r, int t, int b, Color color) =>
            RenderLib.DrawParallelogram(l, r, t, b, color, this.GuiBox.OutlineColor,
                this.GuiBox.OutlineThickness, this.GuiBox.SlantL, this.GuiBox.SlantR, this.Prog);
    }

    public void AddRoutine(Routine routine) => this.Data.AddRoutine(routine);
    public void MarkForRemoval() => this.Data.MarkForRemoval();
}