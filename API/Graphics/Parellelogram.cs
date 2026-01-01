using API.Save;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// todo
/// </summary>
// todo use Position + deprecate
public class Parellelogram : IActor
{
    private const float _DefaultOutlineThickness = 20f;

    public int L;
    public int R;
    public int T;
    public int B;

    public float OutlineThickness;

    /// <summary>
    /// Move X by 1 for every slant Y
    /// </summary>
    public int SlantL = RenderLib.DefaultSlant;

    /// <inheritdoc cref="SlantL" />
    public int SlantR = RenderLib.DefaultSlant;

    public ActorData Data { get; }

    /// <inheritdoc cref="ActorData.Priority" />
    public RenderPriority Priority
    {
        get => this.Data.Priority;
        set => this.Data.Priority = value;
    }

    public Parellelogram(int l, int r, int t, int b, float outlineThickness = _DefaultOutlineThickness,
        RenderPriority renderPriority = RenderPriority.B1Med)
    {
        this.L = l;
        this.R = r;
        this.T = t;
        this.B = b;
        this.OutlineThickness = outlineThickness;
        this.Data = new ActorData(this, renderPriority);
    }

    public virtual void Draw(GameTime gt)
    {
        if (this.Prog == 0)
        {
            return;
        }

        RenderLib.DrawParallelogram(this.L, this.R, this.T, this.B, Settings.Theme.Bg, Settings.Theme.Fg,
            this.OutlineThickness, this.SlantL, this.SlantR, this.Prog);
    }

    public void OnCreate() { }
    public void OnDestroy() { }
}

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