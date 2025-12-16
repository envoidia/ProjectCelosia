using System;
using API.Extensions;
using API.Menu;
using API.Util;
using Cyotek.Drawing.BitmapFont;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Layered bars representing a numerical amount, with text
/// </summary>
public sealed class StatBarWidget : ILayoutWidget, IActor, IAnimated {
    public Color ColorLayer0 { get; init; } = Colors.LightRed;
    public Color ColorLayer1 { get; init; } = Color.Yellow;

    /// <summary>
    /// Colors for layers after the first 2
    /// </summary>
    private static readonly Color[] _Layers = [Color.Lime, Color.Cyan, Colors.LightPurple, Color.White];

    private const int _BarStartOffset = 90;

    public Label Title { get; } = new() { Alignment = Alignment.Controlled };
    public Label Text { get; } = new() { Alignment = Alignment.Controlled };

    /// <summary>
    /// The value being tracked
    /// </summary>
    public int Val {
        get;
        set {
            field = value;
            this._UpdateText();
        }
    }

    /// <summary>
    /// The amount to be considered 100%
    /// </summary>
    public int MaxVal {
        get;
        set {
            field = value;
            this._UpdateText();
        }
    }

    public ActorData Data { get; set; }

    public Progress Prog { get; set; }

    public float Speed => IAnimated.DefaultSpeed;

    public StatBarWidget(Vector2 pos, int width, RenderPriority renderPriority, string title = "") {
        this.Data = new(this, renderPriority);
        this.Position = pos;
        this.Width = width;
        this.Title.Text = title;

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

    public void Draw(GameTime gameTime) {
        // todo prog

        // Draw bars
        float barCount = this.Val / (float) this.MaxVal;

        // Length 0-1 of upper bar
        float upperBarLen = (float) (barCount - Math.Floor(barCount));

        // Lower bar
        drawBar(this._GetLayerColor((int) Math.Floor(barCount)), upperBarLen, 1 - upperBarLen);

        // Upper bar
        if (barCount != Math.Floor(barCount)) {
            drawBar(this._GetLayerColor((int) Math.Ceiling(barCount)), 0, upperBarLen);
        }

        this.Title.Data.Act(gameTime);
        this.Text.Data.Act(gameTime);

        if (DebugMenu._drawActorOutlines) {
            this.Title.Data.DrawDebug(false);
            this.Text.Data.DrawDebug(false);
        }

        void drawBar(Color c, float start, float len) =>
            RenderLib.DrawParallelogram(
                new Vector2(this.X + ((this.Width - _BarStartOffset) * start) + _BarStartOffset, this.Y + 5),
                new Point((int) ((this.Width - _BarStartOffset) * len), this.Height - 10), this.Origin,
                c, Color.Red, 0f,
                RenderLib.DefaultSlant, RenderLib.DefaultSlant, this.Prog);
    }

    private void _UpdateText() {
        this.Text.Text = $"{ColorCode.Black}{this.Val.FormatNoColor(false)}//{this.MaxVal.FormatNoColor(false)}";
        this.CalcLayout();
    }

    private Color _GetLayerColor(int layer) => layer switch {
        0 => this.ColorLayer0,
        1 => this.ColorLayer1,
        _ => _Layers[Math.Min(layer - 2, _Layers.Length - 1)]
    };
}
