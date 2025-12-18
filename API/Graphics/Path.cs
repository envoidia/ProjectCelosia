using System;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// A line
/// </summary>
// todo: support more than 2 points
public sealed class Path : IActor {
    public Vector2 Start { get; set; }
    public Vector2 End { get; set; }
    public float Thickness { get; set; }
    public Color Color { get; set; } = Settings.ColorFg;

    public ActorData Data { get; }

    public RenderPriority Priority {
        get => this.Data.Priority;
        set => this.Data.Priority = value;
    }

    public Path(Vector2 start, Vector2 end, RenderPriority renderPriority = RenderPriority.B1Med, float thickness = 5f) {
        this.Start = start;
        this.End = end;
        this.Thickness = thickness;
        this.Data = new(this, renderPriority);
    }

    public void Draw(GameTime gameTime) =>
        Core.ShapeBatch.DrawLine(this.Start, this.End, this.Thickness, this.Color, this.Color, 0);

    public void OnCreate() => this.AddRoutine(IActor.In);
    public void OnDestroy() => this.AddRoutine(IActor.Out);
}