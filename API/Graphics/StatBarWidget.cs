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
public sealed class StatBarWidget(Vector2 pos, int width, RenderPriority renderPriority, string text = "")
    : StatBarWidgetBase(pos, width, renderPriority, text) {
    public Color ColorLayer0 { get; init; } = Colors.Neg;
    public Color ColorLayer1 { get; init; } = Colors.Stat;

    /// <summary>
    /// Colors for layers after the first 2
    /// </summary>
    private static readonly Color[] _Layers = [Colors.Pos, Colors.Blues[5], Colors.Pinks[5], Colors.White];

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

    public override void Draw(GameTime gameTime) {
        // todo animate between stages whenever it changes

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

        if (_DebugMenu._drawActorOutlines) {
            this.Title.Data.DrawDebug(false);
            this.Text.Data.DrawDebug(false);
        }

        void drawBar(Color c, float start, float len) {
            Vector2 pos = new(MathHelper.SmoothStep(this.AnimFrom.X,
                this.X + ((this.Width - _BarStartOffset) * start) + _BarStartOffset, (float) this.Prog),
                this.Y + _HeightOffset);

            RenderLib.DrawParallelogram(
                pos, new((int) ((this.Width - _BarStartOffset) * len), this.Height - (_HeightOffset * 2)),
                this.Origin, c, Color.Red, 0f,
                RenderLib.DefaultSlant, RenderLib.DefaultSlant, Progress.One);
        }
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
