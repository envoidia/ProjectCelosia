using System;
using API.Menu;
using API.Util;
using Cyotek.Drawing.BitmapFont;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public sealed class StatBarWidget : ILayoutWidget, IActor, IAnimated {
    public Label Title { get; }
    public Label Text { get; }

    /// <summary>
    /// The value being tracked
    /// </summary>
    public int Val { get; set; }

    /// <summary>
    /// The amount to be considered 100%
    /// </summary>
    public int MaxVal { get; set; }

    public ActorData Data { get; set; }

    public Progress Prog { get; set; }

    public float Speed => IAnimated.DefaultSpeed;

    public StatBarWidget(string title, string text) {
        this.Title = new Label() {
            Text = title,
            Alignment = Alignment.Controlled
        };

        this.Text = new Label() {
            Text = text,
            Alignment = Alignment.Controlled
        };

        this.Data = new(this, RenderPriority.B2Med);
    }

    public void CalcLayout() {

    }

    public void Create() => this.AddRoutine(IAnimated.In);
    public void Destroy() => this.AddRoutine(IAnimated.Out);

    public void Draw(GameTime gameTime) {
        this.Title.Data.Act(gameTime);
        this.Text.Data.Act(gameTime);
    }
}
