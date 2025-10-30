using System;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace API.Graphics;

public class TextureFragmentColored : IRenderable {
    public Texture2D Texture { get; }
    public Rectangle Region { get; }

    public Point Size => new((int) (this.Region.Width * this._scale.X + 0.5f), (int) (this.Region.Height * this._scale.Y + 0.5f));

    private readonly Vector2 _scale = Vector2.One;

    public TextureFragmentColored(Texture2D texture, Rectangle region) {
        ArgumentNullException.ThrowIfNull(texture);

        this.Texture = texture;
        this.Region = region;
    }

    public void Draw(FSRenderContext context, Vector2 position, Color color) {
        context.DrawImage(this.Texture, this.Region, position, this._scale, color);
    }
}