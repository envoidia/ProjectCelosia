using System;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Base class for all renderable objects. Stores position, priority, and alignment
/// </summary>
public abstract class RenderObject {
    public Vector2 Position { get; set; }

    public Vector2 Size { get; set; }

    protected RenderPriority Priority { get; set; }

    public abstract Alignment Alignment { get; set; }

    // Raw position of the origin. Not meant to be viewed or used directly
    protected Vector2 Origin { get; set; }

    protected abstract void AddToRenderList();

    protected Vector2 CalcOrigin() {
        return this.Alignment switch {
            Alignment.TopLeft => Vector2.Zero,
            Alignment.TopRight => new Vector2(this.Size.X * 2, 0), // todo why does this need to be * 2?????????
            Alignment.BottomLeft => new Vector2(0, this.Size.Y),
            Alignment.BottomRight => new Vector2(this.Size.X, this.Size.Y),
            Alignment.Center => new Vector2(this.Size.X * 0.5f, this.Size.Y * 0.5f),
            _ => throw new ArgumentOutOfRangeException(nameof(this.Alignment))
        };
    }
}