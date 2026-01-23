using System;
using API.Debug;
using API.Extensions;
using API.Graphics;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Menu.Widget;

/// <summary>
/// Layered bars representing a numerical amount, with text
/// </summary>
public sealed class StatBarWidget : StatBarWidgetBase
{
    public ThemeColor ColorLayer0
    {
        get;
        init
        {
            field = value;
            this.ThemeChange();
        }
    } = ThemeColor.Neg;

    public ThemeColor ColorLayer1
    {
        get;
        init
        {
            field = value;
            this.ThemeChange();
        }
    } = ThemeColor.Stat;

    /// <summary>
    /// Colors for layers after the first 2
    /// </summary>
    private static Color[] _layers = [];

    /// <summary>
    /// The value being tracked
    /// </summary>
    public int Val
    {
        get;
        set
        {
            field = value;
            this._UpdateText();
        }
    }

    /// <summary>
    /// The amount to be considered 100%
    /// </summary>
    public int MaxVal
    {
        get;
        set
        {
            field = value;
            this._UpdateText();
        }
    }

    private Color _c0;
    private Color _c1;

    internal static void _Init()
    {
        ThemeChangeStatic();
        Theme.OnChange += ThemeChangeStatic;
    }

    public StatBarWidget(Vector2 pos, int width, RenderPriority renderPriority, string text = "")
        : base(pos, width, renderPriority, text)
    {
        this.ThemeChange();
    }

    public override void Draw(GameTime gt)
    {
        // todo animate between stages whenever it changes

        // Draw bars
        float barCount = this.Val / (float) this.MaxVal;

        // Length 0-1 of upper bar
        float upperBarLen = (float) (barCount - Math.Floor(barCount));

        // Lower bar
        drawBar(this._GetLayerColor((int) Math.Floor(barCount)), upperBarLen, 1 - upperBarLen);

        // Upper bar
        if (barCount != Math.Floor(barCount))
        {
            drawBar(this._GetLayerColor((int) Math.Ceiling(barCount)), 0, upperBarLen);
        }

        this.Title.Data.Act(gt);
        this.Text.Data.Act(gt);

        if (DebugUtil.DrawActorOutlines)
        {
            this.Title.Data.DrawDebug(false);
            this.Text.Data.DrawDebug(false);
        }

        void drawBar(Color c, float start, float len)
        {
            Vector2 pos = new(MathHelper.SmoothStep(this.AnimFrom.X,
                this.X + ((this.Width - _BarStartOffset) * start) + _BarStartOffset, (float) this.Prog),
                this.Y + _HeightOffset);

            RenderLib.DrawParallelogram(
                pos, new((int) ((this.Width - _BarStartOffset) * len), this.Height - (_HeightOffset * 2)),
                this.Origin, c, Color.Red, 0f,
                RenderLib.DefaultSlant, RenderLib.DefaultSlant, Progress.One);
        }
    }

    public override void ThemeChange()
    {
        this._c0 = Settings.Theme.Get(this.ColorLayer0);
        this._c1 = Settings.Theme.Get(this.ColorLayer1);
    }

    private static void ThemeChangeStatic()
    {
        _layers = [Settings.Theme.Pos, Settings.Theme.StatBarLayer4, Settings.Theme.StatBarLayer5, Settings.Theme.White];
    }

    private void _UpdateText()
    {
        this.Text.Text = $"{ThemeColor.Black.Str}{this.Val.FormatNoColor(false)}//{this.MaxVal.FormatNoColor(false)}";
        this.CalcLayout();
    }

    private Color _GetLayerColor(int layer)
    {
        return layer switch
        {
            0 => this._c0,
            1 => this._c1,
            _ => _layers[Math.Min(layer - 2, _layers.Length - 1)]
        };
    }
}
