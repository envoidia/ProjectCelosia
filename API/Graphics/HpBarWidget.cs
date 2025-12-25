using System;
using API.Extensions;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Layered bars representing HP, Shield, and HP over max
/// </summary>
public sealed class HpBarWidget(Vector2 pos, int width, RenderPriority renderPriority)
    : StatBarWidgetBase(pos, width, renderPriority, ThemeColor.Stat.Str() + "StatHp".GetLang()) {
    public int Hp {
        get;
        set {
            field = value;
            this._Update();
        }
    }

    public int Shield {
        get;
        set {
            field = value;
            this._Update();
        }
    }

    public int MaxHp {
        get;
        set {
            field = value;
            this._Update();
        }
    }

    private float[] _barLens = [0, 0, 0];
    private Color[] _layers = [];

    public override void Draw(GameTime gameTime) {
        // todo animate between stages whenever it changes

        if (this._barLens[0] > 0) drawBar(this._layers[0], 0, this._barLens[0]);
        if (this._barLens[1] > 0) drawBar(this._layers[1], this._barLens[0], this._barLens[1] - this._barLens[0]);
        if (this._barLens[2] > 0) drawBar(this._layers[2], this._barLens[1], this._barLens[2] - this._barLens[1]);
        if (this._barLens[2] != 1) drawBar(Settings.Theme.Neg, this._barLens[2], 1 - this._barLens[2]);

        this.Title.Data.Act(gameTime);
        this.Text.Data.Act(gameTime);

        if (DebugUtil._drawActorOutlines) {
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

    public override void ThemeChange() => this._Update();

    private void _Update() {
        float hpLen = this.Hp / (float) this.MaxHp;
        this._barLens = [Math.Min(hpLen, 1), this.Shield / (float) this.MaxHp, Math.Max(hpLen - 1, 0)];
        this._layers = [Settings.Theme.Hp, Settings.Theme.Shield, Settings.Theme.Overheal];

        Array.Sort(this._barLens, this._layers);

        string shield = this.Shield > 0 ? $"+{this.Shield.FormatNoColor(false)}" : "";
        this.Text.Text = $"{ThemeColor.Black.Str()}{this.Hp.FormatNoColor(false)}{shield}//{this.MaxHp.FormatNoColor(false)}";

        this.CalcLayout();
    }
}
