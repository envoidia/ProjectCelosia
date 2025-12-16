using System;
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

    public void Create() => this.AddRoutine(IActor.In);
    public void Destroy() => this.AddRoutine(IActor.Out);

    public void Draw(GameTime gameTime) {
        RenderLib.DrawParallelogram(this.Position, this.Size, this.Origin, Settings.ColorFg,
            Color.Red, 0f, RenderLib.DefaultSlant, RenderLib.DefaultSlant, this.Prog);
    }
}

// public enum LineDir {
//     Horiz,
//     Vert
// }