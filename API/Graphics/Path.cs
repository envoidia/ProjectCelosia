using System;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// A line
/// </summary>
// todo: support more than 2 points, cleanup
public sealed class Path(Vector2 start, Vector2 end, Priority priority = Priority.Normal, float thickness = 5f)
    : Actor(priority), IAnimatedPrimitive {
    public Vector2 Start { get; set; } = start;
    public Vector2 End { get; set; } = end;
    public float Thickness { get; set; } = thickness;
    public Color Color { get; set; } = Color.White;

    public float Speed { get; set; } = 2f;

    public Progress Prog { get; set; } = new();

    public override void Draw(GameTime gameTime) =>
        Core.ShapeBatch.DrawLine(this.Start, this.End, this.Thickness, this.Color, this.Color, 0);
}