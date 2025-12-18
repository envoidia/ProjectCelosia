using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// A line with slanted edges. Can only be horizontal or vertical, not arbitrary angles (todo: currently only supports horizontal)
/// </summary>
public class LineActor : IActor {
    // todo public LineDir LineDir { get; set; }

    public ActorData Data { get; set; }

    public LineActor(Vector2 pos, Point size, RenderPriority renderPriority = RenderPriority.B2Med) {
        this.Data = new(this, renderPriority);

        this.Position = pos;
        this.Size = size;
        //this.LineDir = pos.Y == size.Y ? LineDir.Horiz : LineDir.Vert;
    }

    public void OnCreate() { }
    public void OnDestroy() { }

    public void Draw(GameTime gameTime) {
        Vector2 pos = new(MathHelper.SmoothStep(this.AnimFrom.X, this.X, (float) this.Prog), this.Y);

        RenderLib.DrawParallelogram(pos, this.Size, this.Origin, Settings.Theme.Fg,
            Color.Red, 0f, RenderLib.DefaultSlant, RenderLib.DefaultSlant, Progress.One);
    }
}

// public enum LineDir {
//     Horiz,
//     Vert
// }