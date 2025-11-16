using FontStashSharp;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public class Label : RenderObject {
    private RichTextLayout RichTextLayout { get; set; } = new();

    public string Text {
        get => this.RichTextLayout.Text;
        set {
            this.RichTextLayout.Text = value;
            this.Origin = this.CalcOrigin();
        }
    }

    public int Width {
        get => (int) this.RichTextLayout.Width!;
        set => this.RichTextLayout.Width = value; // todo remeasure
    }

    public override Point Size => this.RichTextLayout.Size;

    public override Alignment Alignment {
        get;
        set {
            field = value;
            this.Origin = this.CalcOrigin();
        }
    } = Alignment.TopLeft;

    // Background
    public bool HasBackground { get; set; } = false;
    public Color BackgroundColor { get; set; } = Colors.TransBlack;
    public Vector2 BackgroundPadding { get; set; } = new(10, 10);

    public Label(Stage stage, string text, bool isVisible, DynamicSpriteFont font) {
        this.Text = text;
        this.IsVisible = isVisible;
        this.RichTextLayout.Font = font;
        stage.Add(this);
    }

    public Label(Stage stage, string text, bool isVisible = true) : this(stage, text, isVisible, Core.Koruri50) { }

    public Label(Stage stage, bool isVisible = true) : this(stage, "", isVisible, Core.Koruri50) { }

    public override void Draw(GameTime gameTime) {
        if (string.IsNullOrWhiteSpace(this.Text)) return;

        if (this.HasBackground) {
            Core.SpriteBatch.Draw(Core.WhitePixel, new Rectangle(
                (int) (this.Position.X - this.BackgroundPadding.X - this.Origin.X),
                (int) (this.Position.Y - this.BackgroundPadding.Y - this.Origin.Y),
                (int) (this.Size.X + (this.BackgroundPadding.X * 2)),
                (int) (this.Size.Y + (this.BackgroundPadding.Y * 2))), this.BackgroundColor);
        }

        this.RichTextLayout.Draw(Core.SpriteBatch, this.Position, Color.White, 0f, this.Origin.ToVector2());
    }
}