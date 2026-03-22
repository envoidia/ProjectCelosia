using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public sealed class ARectangle : IActor
{
    public ActorData Data { get; }

    public ARectangle(ThemeColor fillColor, RenderPriority priority = RenderPriority.B1Med)
    {
        this.Data = new(this, priority);
        this.FillColor = fillColor;
        Theme.OnChange += this._ThemeChange;
    }

    private Color _fillColor;
    public ThemeColor FillColor
    {
        get;
        set
        {
            field = value;
            this._ThemeChange();
        }
    }

    private Color _outlineColor;
    public ThemeColor OutlineColor
    {
        get;
        set
        {
            field = value;
            this._ThemeChange();
        }
    }


    private void _ThemeChange()
    {
        this._outlineColor = Settings.Theme.Get(this.OutlineColor);
        this._fillColor = Settings.Theme.Get(this.FillColor);
    }

    public void Draw(GameTime gt)
    {
        Core.ShapeBatch.DrawRectangle(
            MathUtil.SmoothStep(this.AnimFrom, this.Position, (float) this.Prog) - this.Origin.ToVector2(),
            new(this.Width, this.Height), this._fillColor, this._outlineColor);
    }
}
