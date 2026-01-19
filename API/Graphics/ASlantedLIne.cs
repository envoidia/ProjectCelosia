using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// A line with slanted edges. Can only be horizontal or vertical, not arbitrary angles 
/// todo: currently only supports horizontal
/// </summary>
public class ASlantedLine : IActor
{
    // todo public LineDir LineDir;

    public ActorData Data { get; }

    /// <param name="pos">Start pos</param>
    /// <param name="size">End pos</param>
    public ASlantedLine(Vector2 pos, Point size, RenderPriority renderPriority = RenderPriority.B2Med)
    {
        this.Data = new(this, renderPriority);

        this.Position = pos;
        this.Size = size;
        //this.LineDir = pos.Y == size.Y ? LineDir.Horiz : LineDir.Vert;
    }

    public void OnCreate() { }
    public void OnDestroy() { }

    public void Draw(GameTime gt)
    {
        RenderLib.DrawParallelogram(new Vector2(MathHelper.SmoothStep(this.AnimFrom.X, this.X,
            (float) this.Prog), this.Y), this.Size, this.Origin, Settings.Theme.Fg,
            Color.Red, 0f, RenderLib.DefaultSlant, RenderLib.DefaultSlant, Progress.One);
    }
}

// public enum LineDir {
//     Horiz,
//     Vert
// }