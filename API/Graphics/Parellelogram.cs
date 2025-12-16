using System;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// todo
/// </summary>
// todo use Position + deprecate
public class Parellelogram : IActor, IAnimated {
    public int L { get; set; }
    public int R { get; set; }
    public int T { get; set; }
    public int B { get; set; }

    public Color Color { get; set; } = Settings.ColorBg;

    public float OutlineThickness { get; set; }
    public Color OutlineColor { get; set; } = Settings.ColorFg;

    /// <summary>
    /// Move X by 1 for every slant Y
    /// </summary>
    public int SlantL { get; set; } = RenderLib.DefaultSlant;

    /// <inheritdoc cref="SlantL" />
    public int SlantR { get; set; } = RenderLib.DefaultSlant;

    public ActorData Data { get; }

    /// <inheritdoc cref="ActorData.Priority" />
    public RenderPriority Priority {
        get => this.Data.Priority;
        set => this.Data.Priority = value;
    }

    public Progress Prog { get; set; } = new();
    public float Speed { get; set; } = 2f;

    public Parellelogram(int l, int r, int t, int b, float outlineThickness = 10,
        RenderPriority renderPriority = RenderPriority.B1Med) {
        this.L = l;
        this.R = r;
        this.T = t;
        this.B = b;
        this.OutlineThickness = outlineThickness;
        this.Data = new ActorData(this, renderPriority);
    }

    public virtual void Draw(GameTime gameTime) {
        if (this.Prog == 0) return;

        RenderLib.DrawParallelogram(this.L, this.R, this.T, this.B, this.Color, this.OutlineColor,
            this.OutlineThickness, this.SlantL, this.SlantR, this.Prog);
    }

    public void Create() => this.AddRoutine(IAnimated.In);
    public void Destroy() => this.AddRoutine(IAnimated.Out);
}

public static class Parellelograms {
    /// <summary>
    /// <c>Parellelogram</c> that covers most of the left half of the screen
    /// </summary>
    // todo how far offscreen is needed
    public static readonly Parellelogram CoverLeft = new(10, 2000, 0, World.H) {
        Speed = IAnimated.DefaultSpeed,
        SlantL = 0,
        Priority = RenderPriority.B2Low
    };
}