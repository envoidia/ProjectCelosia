using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Base class for all renderable objects. Stores position, priority, and alignment
/// </summary>
public abstract class RenderObject : IRenderable {
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
            this._Origin = this._CalcOrigin();
        }
    } = Alignment.TopLeft;

    // Raw position of the origin. Not meant to be viewed or used directly
    internal Point _Origin { get; set; } = Point.Zero;

    public bool IsVisible { get; set; } = true;

    public RenderPriority RenderPriority { get; set; } = RenderPriority.Base;

    internal Point _CalcOrigin() => this.Alignment switch {
        Alignment.TopLeft => Point.Zero,
        Alignment.TopRight => new Point(this.Size.X, 0),
        Alignment.BottomLeft => new Point(0, this.Size.Y),
        Alignment.BottomRight => new Point(this.Size.X, this.Size.Y),
        Alignment.Center => new Point((int) (this.Size.X * 0.5f), (int) (this.Size.Y * 0.5f))
    };

    public abstract void Draw(GameTime gameTime);
}