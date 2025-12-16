using System;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public class RectangleActor : IActor {
    public ActorData Data { get; set; }

    public RectangleActor() => this.Data = new(this);

    public void Create() => this.AddRoutine(IActor.In);
    public void Destroy() => this.AddRoutine(IActor.Out);

    public void Draw(GameTime gameTime) => Core.ShapeBatch.DrawRectangle(
            MathUtil.SmoothStep(this.AnimFrom, this.Position, (float) this.Prog) - this.Origin.ToVector2(),
            new(this.Width, this.Height), Colors.Trans, Color.White);
}
