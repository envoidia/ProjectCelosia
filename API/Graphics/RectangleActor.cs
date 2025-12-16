using System;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public class RectangleActor : IActor {
    /// <inheritdoc cref="Label.BasePos" />
    public Vector2 BasePos { get; set; }

    public ActorData Data { get; set; }

    public RectangleActor() => this.Data = new(this);

    public void Create() => this.AddRoutine(IActor.In);
    public void Destroy() => this.AddRoutine(IActor.Out);

    public void Draw(GameTime gameTime) => Core.ShapeBatch.DrawRectangle(
            MathUtil.SmoothStep(this.BasePos, this.Position, (float) this.Prog) - this.Origin.ToVector2(),
            new Vector2(this.Width, this.Height), Colors.Trans, Color.White);
}
