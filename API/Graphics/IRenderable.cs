using Microsoft.Xna.Framework;

namespace API.Graphics;

public interface IRenderable {
    bool IsVisible { get; set; }
    RenderPriority RenderPriority { get; set; }

    void Draw(GameTime gameTime);
}