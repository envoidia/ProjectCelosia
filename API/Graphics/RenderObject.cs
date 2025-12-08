using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Base class for more complex renderable objects. Stores position, priority, and alignment
/// </summary>
// todo will this be used for anything other than Label? remove if not
public abstract class RenderObject(RenderPriority priority = RenderPriority.B1Med) : Actor(priority) {
    private Vector2 _position = Vector2.Zero;

    public Vector2 Position {
        get => this._position;
        set => this._position = value;
    }

    public float X {
        get => this._position.X;
        set => this._position.X = value;
    }

    public float Y {
        get => this._position.Y;
        set => this._position.Y = value;
    }

    public abstract Point Size { get; }

    public Alignment Alignment {
        get;
        set {
            field = value;
            this._CalcOrigin();
        }
    } = Alignment.TopLeft;

    /// <summary>
    /// Raw position of the origin
    /// </summary>
    internal Point _Origin { get; set; } = Point.Zero;

    internal void _CalcOrigin() => this._Origin = this.Alignment switch {
        Alignment.TopLeft => Point.Zero,
        Alignment.TopRight => new Point(this.Size.X, 0),
        Alignment.BottomLeft => new Point(0, this.Size.Y),
        Alignment.BottomRight => new Point(this.Size.X, this.Size.Y),
        Alignment.Center => new Point((int) (this.Size.X * 0.5f), (int) (this.Size.Y * 0.5f)),
        _ => throw new ClosedEnumsWhenException()
    };
}