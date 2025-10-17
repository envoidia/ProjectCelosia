using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Base class for all renderable objects. Stores position, priority, and alignment
/// </summary>
public abstract class RenderObject {
    public Vector2 Position { get; set; }

    protected RenderPriority Priority { get; set; }

    protected Alignment alignment = Alignment.TopLeft;
    public abstract Alignment Alignment { get; set; }

    // Raw position of the origin. Not meant to be viewed or used directly
    protected Vector2 OriginRaw { get; set; }

    protected abstract void AddToRenderList();

    protected Vector2 CalcOriginRaw(Vector2 size) {
        return this.Alignment switch {
            Alignment.TopLeft => Vector2.Zero,
            Alignment.TopRight => new Vector2(size.X, 0),
            Alignment.BottomLeft => new Vector2(0, size.Y),
            Alignment.BottomRight => new Vector2(size.X, size.Y),
            Alignment.Center => new Vector2(size.X * 0.5f, size.Y * 0.5f),
            _ => throw new UnreachableException("Specified enum member does not exist")
        };
    }
}