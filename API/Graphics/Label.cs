using System;
using API.Util;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Renderable text <c>IActor</c>
/// </summary>
// todo color
public sealed class Label : IActor {
    public string Text {
        get => this._RichTextLayout.Text;
        set {
            this._RichTextLayout.Text = value;
            this.Size = this._RichTextLayout.Size;
            this.Origin = this.Data.CalcOrigin();
        }
    }

    // Background
    public bool HasBackground { get; set; } = false;
    public Color BackgroundColor { get; set; } = Colors.TransBlack;
    public Vector2 BackgroundPadding { get; set; } = new(10, 10);

    public ActorData Data { get; }

    private RichTextLayout _RichTextLayout { get; set; } = new() { Font = Core.Koruri60 };

    public Label(RenderPriority priority = RenderPriority.B1Med) {
        this.Data = new ActorData(this, priority);
    }

    public override string ToString() => $"Label: {this._RichTextLayout.Text}";

    public void Draw(GameTime gameTime) {
        if (string.IsNullOrWhiteSpace(this.Text)) return;

        if (this.HasBackground) {
            Core.SpriteBatch.Draw(Core.WhitePixel, new Rectangle(
                (int) (this.Position.X - this.BackgroundPadding.X - this.Origin.X),
                (int) (this.Position.Y - this.BackgroundPadding.Y - this.Origin.Y),
                (int) (this.Size.X + (this.BackgroundPadding.X * 2)),
                (int) (this.Size.Y + (this.BackgroundPadding.Y * 2))), this.BackgroundColor);
        }

        this._RichTextLayout.Draw(Core.SpriteBatch, this.Position, Color.White, 0f, this.Origin.ToVector2());
    }

    public void Create() { }
    public void Destroy() => this.MarkForRemoval();
}