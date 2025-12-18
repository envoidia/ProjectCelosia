using API.Save;
using API.Util;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Renderable text <c>IActor</c>.
/// Expected to have static lifetime -- otherwise, make sure to manually unsubscribe from <c>Theme.Change</c>/// </summary>
// todo color
public sealed class Label : IActor {
    public string Text {
        get => this._RichTextLayout.Text;
        set {
            this._RichTextLayout.Text = value; //$"{ThemeColor.White.Str()}{value}"; // todo idt this is needed
            this.Size = this._RichTextLayout.Size;
            this.Origin = this.Data.CalcOrigin();
        }
    }

    // Background
    public bool HasBackground { get; set; } = false;

    private Color _bgC;
    public ThemeColor BackgroundColor { get; set; } = ThemeColor.TransBlack;

    private RichTextLayout _RichTextLayout { get; set; } = new() { Font = Core.Koruri60 };

    public ActorData Data { get; }

    public Label(RenderPriority priority = RenderPriority.B1Med) {
        this.Data = new ActorData(this, priority);

        this._bgC = Settings.Theme.Get(this.BackgroundColor);

        Theme.Change += new Theme.ThemeChange((prevTheme, newTheme) => {
            this._bgC = newTheme.Get(this.BackgroundColor);

            // Force text to re-render
            string t = this.Text;
            this.Text = "";
            this.Text = t;
        });
    }

    public override string ToString() => $"Label: {this._RichTextLayout.Text}";

    public void OnCreate() { }
    public void OnDestroy() { }

    public void Draw(GameTime gameTime) {
        // todo is this return good
        if (string.IsNullOrWhiteSpace(this.Text)) return;

        if (this.HasBackground) this.Data.DrawBackground(this._bgC);

        this._RichTextLayout.Draw(Core.SpriteBatch, MathUtil.SmoothStep(this.AnimFrom, this.Position,
            (float) this.Prog), Settings.Theme.Fg, 0f, this.Origin.ToVector2());
    }
}