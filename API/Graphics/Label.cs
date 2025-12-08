using FontStashSharp;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Renderable text object
/// </summary>
public sealed class Label : RenderObject {
    private RichTextLayout _RichTextLayout { get; set; } = new();

    public string Text {
        get => this._RichTextLayout.Text;
        set {
            this._RichTextLayout.Text = value;
            this._CalcOrigin();
        }
    }

    public int Width {
        get => (int) this._RichTextLayout.Width; // todo null safety
        set => this._RichTextLayout.Width = value; // todo remeasure
    }

    public override Point Size => this._RichTextLayout.Size;

    // Background
    public bool HasBackground { get; set; } = false;
    public Color BackgroundColor { get; set; } = Colors.TransBlack;
    public Vector2 BackgroundPadding { get; set; } = new(10, 10);

    public Label(Stage? stage, string text, bool isVisible, DynamicSpriteFont font) {
        this.Text = text;
        this.IsVisible = isVisible;
        this._RichTextLayout.Font = font;
        stage?.Add(this);
    }

    public Label(Stage stage, string text, bool isVisible = true) : this(stage, text, isVisible, Core.Koruri50) { }

    public Label(Stage stage, bool isVisible = true) : this(stage, "", isVisible, Core.Koruri50) { }

    public override void Draw(GameTime gameTime) {
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
}