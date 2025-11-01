using FontStashSharp;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

    public sealed override Alignment Alignment {
        get;
        set {
            field = value;
            this.Origin = this.CalcOrigin();
        }
    } = Alignment.TopLeft;

    public bool Visible { get; set; } = true;

    // Background
    public bool HasBackground { get; set; } = false;
    public Color BackgroundColor { get; set; } = Colors.TransBlack;
    public Vector2 BackgroundPadding { get; set; } = new(10, 10);

    public Label() {
        this.RichTextLayout.Font = Core.Koruri50;
        this.AddToRenderList();
    }

    public Label(DynamicSpriteFont font) {
        this.RichTextLayout.Font = font;
        //this.RichTextLayout.Width = int.MaxValue;
        this.AddToRenderList();
    }

    public void Draw(SpriteBatch spriteBatch) {
        if (!this.Visible) return;

        if (this.HasBackground) {
            spriteBatch.Draw(Core.WhitePixel, new Rectangle(
                (int) (this.Position.X - this.BackgroundPadding.X - this.Origin.X),
                (int) (this.Position.Y - this.BackgroundPadding.Y - this.Origin.Y),
                (int) (this.Size.X + (this.BackgroundPadding.X * 2)),
                (int) (this.Size.Y + (this.BackgroundPadding.Y * 2))), this.BackgroundColor);
        }

        this.RichTextLayout.Draw(spriteBatch, this.Position, Color.White, 0f, this.Origin.ToVector2());
    }

    protected sealed override void AddToRenderList() {
        switch (this.Priority) {
            case RenderPriority.Low:
                Core.LabelsLow.Add(this);
                break;
            case RenderPriority.Med:
                Core.LabelsMed.Add(this);
                break;
            case RenderPriority.High:
                Core.LabelsHigh.Add(this);
                break;
        }
    }
}