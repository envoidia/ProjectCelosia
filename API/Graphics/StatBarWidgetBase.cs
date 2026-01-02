using System;
using API.Menu;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Base class for StatBarWidget and HpBarWidget.
/// Expected to have static lifetime -- otherwise, make sure to manually unsubscribe from <c>Theme.Change</c>
/// </summary>
public abstract class StatBarWidgetBase : ILayoutWidget, IActor
{
    protected const int _BarStartOffset = 100;
    protected const int _HeightOffset = 5;

    public Label Title { get; } = new() { Alignment = Alignment.Controlled };
    public Label Text { get; } = new() { Alignment = Alignment.Controlled };

    public ActorData Data { get; }

    /// <inheritdoc cref="ActorData.AnimFromDir" />
    public Dir AnimFromDir
    {
        get
        {
            return this.Data.AnimFromDir;
        }

        set
        {
            this.Data.AnimFromDir = value;
            this.Title.AnimFromDir = value;
            this.Text.AnimFromDir = value;
        }
    }

    public StatBarWidgetBase(Vector2 pos, int width, RenderPriority renderPriority, string text)
    {
        this.Data = new(this, renderPriority);
        this.Position = pos;
        this.Width = width;
        this.Title.Text = text;

        Theme.OnChange += this.ThemeChange;

        this.CalcLayout();
    }

    public void CalcLayout()
    {
        this.Title.Position = this.Position;
        this.Title.Origin = this.Origin;

        this.Text.Position = new(this.X + this.Width, this.Y);
        this.Text.Origin = new(this.Origin.X + this.Text.Width, this.Origin.Y);

        this.Height = Math.Max(this.Title.Height, this.Text.Height);
    }

    // todo respect anim type
    public void OnCreate()
    {
        this.Title.Create();
        this.Text.Create();
    }

    public void OnDestroy()
    {
        this.Title.Destroy();
        this.Text.Destroy();
    }

    public abstract void Draw(GameTime gt);

    public abstract void ThemeChange();
}
