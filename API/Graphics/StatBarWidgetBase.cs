using System;
using API.Menu;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Base class for StatBarWidget and HpBarWidget
/// </summary>
public abstract class StatBarWidgetBase : ILayoutWidget, IActor {
    protected const int _BarStartOffset = 90;
    protected const int _HeightOffset = 5;

    public Label Title { get; } = new() { Alignment = Alignment.Controlled };
    public Label Text { get; } = new() { Alignment = Alignment.Controlled };

    public ActorData Data { get; set; }

    /// <inheritdoc cref="ActorData.AnimFromDir" />
    public Dir AnimFromDir {
        get => this.Data.AnimFromDir;
        set {
            this.Data.AnimFromDir = value;
            this.Title.AnimFromDir = value;
            this.Text.AnimFromDir = value;
        }
    }

    public StatBarWidgetBase(Vector2 pos, int width, RenderPriority renderPriority, string text) {
        this.Data = new(this, renderPriority);
        this.Position = pos;
        this.Width = width;
        this.Title.Text = text;
        this.CalcLayout();
    }

    public void CalcLayout() {
        this.Title.Position = this.Position;
        this.Title.Origin = this.Origin;

        this.Text.Position = new(this.X + this.Width, this.Y);
        this.Text.Origin = new(this.Origin.X + this.Text.Width, this.Origin.Y);

        this.Height = Math.Max(this.Title.Height, this.Text.Height);
    }

    public void Create() {
        this.AddRoutine(IActor.In);
        this.Title.AddRoutine(IActor.In);
        this.Text.AddRoutine(IActor.In);
    }

    public void Destroy() {
        this.AddRoutine(IActor.Out);
        this.Title.AddRoutine(IActor.Out);
        this.Text.AddRoutine(IActor.Out);
    }

    public abstract void Draw(GameTime gameTime);
}
