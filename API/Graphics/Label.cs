using API.Save;
using API.Util;
using FontStashSharp;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Renderable text <c>IActor</c>.
/// Expected to have static lifetime -- otherwise, make sure to manually unsubscribe from <c>Theme.Change</c>
/// </summary>
// todo color
public sealed class Label : IActor
{
    public string Text
    {
        get
        {
            return this.RichTextLayout.Text;
        }

        set
        {
            this.RichTextLayout.Text = value;
            this.Size = this.RichTextLayout.Size;
            this.Origin = this.Data.CalcOrigin();
        }
    }

    // Background
    public bool HasBackground = false;

    public Vector2 MinBackgroundSize = Vector2.Zero;

    private Color _bgC;
    public ThemeColor BackgroundColor = ThemeColor.TransBlack;

    public RichTextLayout RichTextLayout;

    /// <summary>
    /// Rotation of the text. The debug outline for rotated text does not currently take rotation into account
    /// </summary>
    public float Rotation = 0f;

    public ActorData Data { get; }

    public Label(RenderPriority priority = RenderPriority.B1Med, DynamicSpriteFont? font = null)
    {
        this.Data = new ActorData(this, priority);

        this.RichTextLayout = new()
        {
            Font = font ?? Core.Koruri60
        };

        this._bgC = Settings.Theme.Get(this.BackgroundColor);

        Theme.OnChange += () =>
        {
            this._bgC = Settings.Theme.Get(this.BackgroundColor);

            // Force text to re-render
            string t = this.Text;
            this.Text = "";
            this.Text = t;
        };
    }

    public override string ToString()
    {
        return $"{base.ToString()}: {this.RichTextLayout.Text}";
    }

    public void Draw(GameTime gt)
    {
        if (this.HasBackground)
        {
            this.Data.DrawBackground(this._bgC, this.MinBackgroundSize);
        }

        if (string.IsNullOrWhiteSpace(this.Text))
        {
            return;
        }

        this.RichTextLayout.Draw(Core.SpriteBatch, MathUtil.SmoothStep(this.AnimFrom, this.Position,
            (float) this.Prog), Settings.Theme.Fg, this.Rotation, this.Origin.ToVector2());
    }
}