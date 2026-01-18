using System;
using System.Reflection;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace API.Graphics;

// todo find a way to resize icons / scale font size
public sealed class TextureFragmentColored : IRenderable
{
    public Texture2D Texture { get; }
    public Rectangle Region { get; }

    public Point Size
    {
        get
        {
            return new((int) ((this.Region.Width * this._Scale.X) + 0.5f),
        (int) ((this.Region.Height * this._Scale.Y) + 0.5f));
        }
    }

    private readonly Vector2 _Scale = Vector2.One;

    public TextureFragmentColored(Texture2D texture, Rectangle region)
    {
        ArgumentNullException.ThrowIfNull(texture);

        this.Texture = texture;
        this.Region = region;
    }

    public void Draw(FSRenderContext context, Vector2 position, Color color)
    {
        //FieldInfo? f = typeof(FSRenderContext).GetField("_scale", BindingFlags.NonPublic | BindingFlags.Instance);
        //f.SetValue(context, new Vector2(1.5f));
        context.DrawImage(this.Texture, this.Region, position, Vector2.Zero, color);
    }
}