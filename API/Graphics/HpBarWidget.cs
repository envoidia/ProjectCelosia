using System;
using API.Extensions;
using API.Menu;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public class HpBarWidget : ILayoutWidget, IActor, IAnimated {
    private const int _BarStartOffset = 90;

    public Label Title { get; } = new() { Alignment = Alignment.Controlled };
    public Label Text { get; } = new() { Alignment = Alignment.Controlled };

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

    public ActorData Data { get; set; }

    public Progress Prog { get; set; }

    public float Speed => IAnimated.DefaultSpeed;

    private float[] _barLens = [0, 0, 0];
    private Color[] _layers = [Color.Lime, Color.Cyan, Colors.Pink];

    public HpBarWidget(Vector2 pos, int width, RenderPriority renderPriority) {
        this.Data = new(this, renderPriority);
        this.Position = pos;
        this.Width = width;
        this.Title.Text = "HP";

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
        // todo animate between stages whenever it changes

        if (this._barLens[0] > 0) drawBar(this._layers[0], 0, this._barLens[0]);
        if (this._barLens[1] > 0) drawBar(this._layers[1], this._barLens[0], this._barLens[1] - this._barLens[0]);
        if (this._barLens[2] > 0) drawBar(this._layers[2], this._barLens[1], this._barLens[2] - this._barLens[1]);
        if (this._barLens[2] != 1) drawBar(Colors.LightRed, this._barLens[2], 1 - this._barLens[2]);

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

    private void _Update() {
        float hpLen = this.Hp / (float) this.MaxHp;
        this._barLens = [Math.Min(hpLen, 1), this.Shield / (float) this.MaxHp, hpLen - 1];
        this._layers = [Color.Lime, Color.Cyan, Colors.Pink];

        Array.Sort(this._barLens, this._layers);

        string shield = this.Shield > 0 ? $"+{this.Shield.FormatNoColor(false)}" : "";
        this.Text.Text = $"{ColorCode.Black}{this.Hp.FormatNoColor(false)}{shield}//{this.MaxHp.FormatNoColor(false)}";

        this.CalcLayout();
    }
}
