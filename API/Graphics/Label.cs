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
    public BackgroundType BackgroundType = BackgroundType.None;

    public Vector2 MinBackgroundSize = Vector2.Zero;

    public RichTextLayout RichTextLayout;

    public int? MaxWidth
    {
        get
        {
            return this.RichTextLayout.Width;
        }
        set
        {
            this.RichTextLayout.Width = value;
        }
    }

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

        Theme.OnChange += () =>
        {
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
        if (string.IsNullOrWhiteSpace(this.Text))
        {
            return;
        }

        int xOff = 0;

        switch (this.BackgroundType)
        {
            case BackgroundType.Rectangle:
                this.Data.DrawBackground(Settings.Theme.TransBlack, this.MinBackgroundSize);
                break;

            case BackgroundType.Parellelogram:
                xOff = this.Height / RenderLib.DefaultSlant;

                RenderLib.DrawParallelogram(new(this.X - this.Padding.L, this.Y - this.Padding.T),
                    new(this.Width + this.Padding.LR + xOff, this.Height + this.Padding.TB),
                    this.Origin, Settings.Theme.Bg, Settings.Theme.Fg,
                    RenderLib.BgOutlineThickness, RenderLib.DefaultSlant,
                    RenderLib.DefaultSlant, Progress.One);

                break;
        }

        this.RichTextLayout.Draw(Core.SpriteBatch, MathUtil.SmoothStep(this.AnimFrom,
            new(this.X + xOff, this.Y), (float) this.Prog), Settings.Theme.Fg,
            this.Rotation, this.Origin.ToVector2());
    }
}