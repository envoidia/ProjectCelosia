using System;
using API.Menu;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public abstract class StatBarWidgetBase : ILayoutWidget, IActor, IAnimated {
    protected const int _BarStartOffset = 90;

    public Label Title { get; } = new() { Alignment = Alignment.Controlled };
    public Label Text { get; } = new() { Alignment = Alignment.Controlled };

    public ActorData Data { get; set; }

    public Progress Prog { get; set; }

    public float Speed => IAnimated.DefaultSpeed;

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

        this.Text.Position = new Vector2(this.X + this.Width, this.Y);
        this.Text.Origin = new Point(this.Origin.X + this.Text.Width, this.Origin.Y);

        this.Height = Math.Max(this.Title.Height, this.Text.Height);
    }

    public void Create() => this.AddRoutine(IAnimated.In);
    public void Destroy() => this.AddRoutine(IAnimated.Out);

    public abstract void Draw(GameTime gameTime);
}
