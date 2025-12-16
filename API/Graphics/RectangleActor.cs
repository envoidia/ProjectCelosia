using Microsoft.Xna.Framework;

namespace API.Graphics;

public class RectangleActor : IActor {
    public ActorData Data { get; set; }

    public RectangleActor() => this.Data = new(this);

    public void Create() { }

    public void Destroy() => this.MarkForRemoval();

    public void Draw(GameTime gameTime) => Core.ShapeBatch.DrawRectangle(this.Position - this.Origin.ToVector2(),
            new Vector2(this.Width, this.Height), Colors.Trans, Color.White);
}
