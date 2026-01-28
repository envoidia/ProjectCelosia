using API.Save;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// A line
/// </summary>
// todo: support more than 2 points
public sealed class Path : IActor
{
    public Vector2 Start;
    public Vector2 End;
    public float Thickness;

    public ActorData Data { get; }

    public RenderPriority Priority
    {
        get
        {
            return this.Data.Priority;
        }

        set
        {
            this.Data.Priority = value;
        }
    }

    public Path(Vector2 start, Vector2 end, RenderPriority renderPriority = RenderPriority.B1Med, float thickness = 5f)
    {
        this.Start = start;
        this.End = end;
        this.Thickness = thickness;
        this.Data = new(this, renderPriority);
    }

    public void Draw(GameTime gt)
    {
        Core.ShapeBatch.DrawLine(this.Start, this.End, this.Thickness, Settings.Theme.Fg, Color.Red, 0);
    }
}