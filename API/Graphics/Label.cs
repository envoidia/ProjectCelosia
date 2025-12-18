using System;
using API.Save;
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
            this._RichTextLayout.Text = $"{ColorCode.White}{value}";
            this.Size = this._RichTextLayout.Size;
            this.Origin = this.Data.CalcOrigin();
        }
    }

    // Background
    public bool HasBackground { get; set; } = false;
    public Color BackgroundColor { get; set; } = Colors.TransBlack;

    private RichTextLayout _RichTextLayout { get; set; } = new() { Font = Core.Koruri60 };

    public ActorData Data { get; }

    public Label(RenderPriority priority = RenderPriority.B1Med) {
        this.Data = new ActorData(this, priority);
    }

    public override string ToString() => $"Label: {this._RichTextLayout.Text}";

    public void OnCreate() { }
    public void OnDestroy() { }

    public void Draw(GameTime gameTime) {
        // todo is this return good
        if (string.IsNullOrWhiteSpace(this.Text)) return;

        if (this.HasBackground) this.Data.DrawBackground(this.BackgroundColor);

        this._RichTextLayout.Draw(Core.SpriteBatch,
            MathUtil.SmoothStep(this.AnimFrom, this.Position, (float) this.Prog),
            Settings.ColorFg, 0f, this.Origin.ToVector2());
    }
}