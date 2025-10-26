using System;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace API.Graphics;

public class Label : RenderObject {
    public string Text {
        get;
        set {
            field = value;
            this.Size = this.Font.MeasureString(this.Text);
            this.Origin = this.CalcOrigin();
        }
    }

    public DynamicSpriteFont Font { get; set; }

    public sealed override Alignment Alignment {
        get;
        set {
            field = value;
            this.Size = this.Font.MeasureString(this.Text);
            this.Origin = this.CalcOrigin();
        }
    }

    public bool Visible { get; set; }

    // Background
    public bool HasBackground { get; set; }
    public Color BackgroundColor { get; set; }
    public Vector2 BackgroundPadding { get; set; }

    private Label(Builder builder) {
        this.Font = builder.font;
        this.Text = builder.text;
        this.Position = builder.position;
        this.Alignment = builder.alignment;
        this.Visible = builder.visible;
        this.HasBackground = builder.hasBackground;
        this.BackgroundColor = builder.backgroundColor;
        this.BackgroundPadding = builder.backgroundPadding;
        this.AddToRenderList();
    }

    public class Builder(DynamicSpriteFont font) {
        internal readonly DynamicSpriteFont font = font;

        internal string text = "";
        internal Vector2 position = Vector2.Zero;
        internal Alignment alignment = Alignment.TopLeft;
        internal bool visible = true;
        internal bool hasBackground = false;
        internal Color backgroundColor = Colors.TransBlack;
        internal Vector2 backgroundPadding = Vector2.One * 10;

        public Builder SetText(string text) {
            this.text = text;
            return this;
        }

        public Builder SetPosition(Vector2 position) {
            this.position = position;
            return this;
        }

        public Builder SetAlignment(Alignment alignment) {
            this.alignment = alignment;
            return this;
        }

        public Builder Invisible() {
            this.visible = false;
            return this;
        }

        public Builder HasBackground() {
            this.hasBackground = true;
            return this;
        }

        public Label Build() {
            return new Label(this);
        }
    }

    public void Draw(SpriteBatch spriteBatch) {
        if (!this.Visible) return;

        if (this.HasBackground) {
            spriteBatch.Draw(Core.WhitePixel, new Rectangle(
                (int) (this.Position.X - this.BackgroundPadding.X), (int) (this.Position.Y - this.BackgroundPadding.Y),
                (int) (this.Size.X + (this.BackgroundPadding.X * 2)),
                (int) (this.Size.Y + (this.BackgroundPadding.Y * 2))), this.BackgroundColor);
        }

        spriteBatch.DrawString(this.Font, this.Text, this.Position, Color.White, 0f, this.Origin);
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
            default:
                throw new ArgumentOutOfRangeException("Priority");
        }
    }
}