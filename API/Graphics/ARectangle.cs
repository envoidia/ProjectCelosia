using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public sealed class ARectangle : IActor
{
    public ActorData Data { get; }

    public ARectangle(ThemeColor color = ThemeColor.White, RenderPriority priority = RenderPriority.B1Med)
    {
        this.Data = new(this, priority);
        this.Color = color;
        Theme.OnChange += this._ThemeChange;
    }

    public ThemeColor Color
    {
        get;
        set
        {
            field = value;
            this._ThemeChange();
        }
    }

    private Color _color;

    private void _ThemeChange()
    {
        this._color = Settings.Theme.Get(this.Color);
    }

    public void OnCreate() { }
    public void OnDestroy() { }

    public void Draw(GameTime gt)
    {
        Core.ShapeBatch.DrawRectangle(
            MathUtil.SmoothStep(this.AnimFrom, this.Position, (float) this.Prog) - this.Origin.ToVector2(),
            new(this.Width, this.Height), Microsoft.Xna.Framework.Color.Trans, this._color);
    }
}
