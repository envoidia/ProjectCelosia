using API.Util;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Renderable text object
/// </summary>
public sealed class Label : IActor {
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

    public Alignment Alignment {
        get;
        set {
            field = value;
            this._CalcOrigin();
        }
    } = Alignment.TopLeft;

    public string Text {
        get => this._RichTextLayout.Text;
        set {
            this._RichTextLayout.Text = value;
            this._CalcOrigin();
        }
    }

    /*public int Width {
        get => (int) this._RichTextLayout.Width; // todo null safety
        set => this._RichTextLayout.Width = value; // todo remeasure
    }*/

    public Point Size => this._RichTextLayout.Size;

    // Background
    public bool HasBackground { get; set; } = false;
    public Color BackgroundColor { get; set; } = Colors.TransBlack;
    public Vector2 BackgroundPadding { get; set; } = new(10, 10);

    public ActorData Data { get; }

    /// <inheritdoc cref="ActorData.Priority" />
    public RenderPriority Priority {
        get => this.Data.Priority;
        set => this.Data.Priority = value;
    }

    internal Point _Origin { get; set; } = Point.Zero;

    private RichTextLayout _RichTextLayout { get; set; } = new() { Font = Core.Koruri50 };

    public Label(RenderPriority priority = RenderPriority.B1Med) {
        this.Data = new ActorData(this, priority);
    }

    public override string ToString() => $"Label: {this._RichTextLayout.Text}";

    public void Draw(GameTime gameTime) {
        if (string.IsNullOrWhiteSpace(this.Text)) return;

        if (this.HasBackground) {
            Core.SpriteBatch.Draw(Core.WhitePixel, new Rectangle(
                (int) (this.Position.X - this.BackgroundPadding.X - this._Origin.X),
                (int) (this.Position.Y - this.BackgroundPadding.Y - this._Origin.Y),
                (int) (this.Size.X + (this.BackgroundPadding.X * 2)),
                (int) (this.Size.Y + (this.BackgroundPadding.Y * 2))), this.BackgroundColor);
        }

        this._RichTextLayout.Draw(Core.SpriteBatch, this.Position, Color.White, 0f, this._Origin.ToVector2());
    }

    public void AddRoutine(Routine routine) => this.Data.AddRoutine(routine);
    public void MarkForRemoval() => this.Data.MarkForRemoval();

    internal void _CalcOrigin() => this._Origin = this.Alignment switch {
        Alignment.TopLeft => Point.Zero,
        Alignment.TopRight => new Point(this.Size.X, 0),
        Alignment.BottomLeft => new Point(0, this.Size.Y),
        Alignment.BottomRight => new Point(this.Size.X, this.Size.Y),
        Alignment.Center => new Point((int) (this.Size.X * 0.5f), (int) (this.Size.Y * 0.5f)),
        _ => throw new ClosedEnumsWhenException()
    };

}