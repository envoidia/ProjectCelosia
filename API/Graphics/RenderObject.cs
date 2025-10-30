using System;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Base class for all renderable objects. Stores position, priority, and alignment
/// </summary>
public abstract class RenderObject {
    public Vector2 Position { get; set; } = Vector2.Zero;

    public virtual Point Size { get; } = Point.Zero;

    public RenderPriority Priority { get; set; } = RenderPriority.Low;

    public abstract Alignment Alignment { get; set; }

    // Raw position of the origin. Not meant to be viewed or used directly
    internal Point Origin { get; set; } = Point.Zero;

    protected abstract void AddToRenderList();
    
    protected Point CalcOrigin() => this.Alignment switch {
        Alignment.TopLeft => Point.Zero,
        Alignment.TopRight => new Point(this.Size.X, 0),
        Alignment.BottomLeft => new Point(0, this.Size.Y),
        Alignment.BottomRight => new Point(this.Size.X, this.Size.Y),
        Alignment.Center => new Point((int) (this.Size.X * 0.5f), (int) (this.Size.Y * 0.5f)),
        _ => throw new ArgumentOutOfRangeException(nameof(this.Alignment))
    };
}