using System;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// A line
/// </summary>
// todo: support more than 2 points, cleanup
public sealed class Path : Actor {
    public Vector2 Start { get; set; }
    public Vector2 End { get; set; }
    public float Thickness { get; set; }
    public Color Color { get; set; } = Color.White;

    public AnimDirs Dir { get; set; } = AnimDirs.Collapsing;

    public float Speed { get; set; } = 2f;

    public Progress Prog { get; set; } = new();

    public Path(Stage stage, Vector2 start, Vector2 end, float thickness = 5f) {
        this.Start = start;
        this.End = end;
        this.Thickness = thickness;
        stage.Add(this);
    }

    public override void Draw(GameTime gameTime) =>
        Core.ShapeBatch.DrawLine(this.Start, this.End, this.Thickness, this.Color, this.Color, 0);
}