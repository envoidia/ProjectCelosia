using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public class GuiBox : IActor, IAnimatedPrimitive {
    public int L { get; set; }
    public int R { get; set; }
    public int T { get; set; }
    public int B { get; set; }

    public Color Color { get; set; } = Color.Black;

    public float OutlineThickness { get; set; }
    public Color OutlineColor { get; set; } = Color.White;

    /// <summary>
    /// Move X by 1 for every slant Y
    /// </summary>
    public int SlantL { get; set; } = 6;

    /// <inheritdoc cref="SlantL" />
    public int SlantR { get; set; } = 6;

    public ActorData Data { get; }

    /// <inheritdoc cref="ActorData.Priority" />
    public RenderPriority Priority {
        get => this.Data.Priority;
        set => this.Data.Priority = value;
    }

    public Progress Prog { get; set; } = new();
    public float Speed { get; set; } = 2f;

    public GuiBox(int l, int r, int t, int b, float outlineThickness = 10,
        RenderPriority priority = RenderPriority.B1Med) {
        this.L = l;
        this.R = r;
        this.T = t;
        this.B = b;
        this.OutlineThickness = outlineThickness;
        this.Data = new ActorData(this, priority);
    }

    public virtual void Draw(GameTime gameTime) {
        if (this.Prog == 0) return;

        RenderLib.DrawParallelogram(this.L, this.R, this.T, this.B, this.Color, this.OutlineColor,
            this.OutlineThickness, this.SlantL, this.SlantR, this.Prog);
    }
}

public static class GuiBoxes {
    /// <summary>
    /// <c>GuiBox</c> that covers most of the left half of the screen
    /// </summary>
    // todo how far offscreen is needed
    public static readonly GuiBox CoverLeft = new(8, 1750, 0, World.H) {
        Speed = 4f,
        SlantL = 0,
        Priority = RenderPriority.B2Low
    };
}