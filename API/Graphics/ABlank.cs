using System;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Blank <c>IActor</c> implementation with a draw delegate to put anything in
/// </summary>
public class ABlank : IActor
{
    public Action<GameTime>? OnDraw;

    public ActorData Data { get; }

    public ABlank(Action<GameTime>? onDraw, RenderPriority priority = RenderPriority.B1Med)
    {
        this.Data = new(this, priority);
        this.OnDraw = onDraw;
    }

    public void Draw(GameTime gt)
    {
        this.OnDraw?.Invoke(gt);
    }
}
