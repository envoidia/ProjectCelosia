using API.Graphics;

namespace API.Battle;

public static class Parellelograms
{
    /// <summary>
    /// <c>Parellelogram</c> that covers most of the left half of the screen
    /// </summary>
    // todo how far offscreen is needed
    public static readonly Parellelogram CoverLeft = new(10, 2000, 0, World.H)
    {
        Speed = IActor.DefaultSpeed,
        SlantL = 0,
        Priority = RenderPriority.B2Low
    };
}
