using System;
using System.Collections.Generic;
using API.Graphics;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Menu;

/// <summary>
/// ListWidget with a right-side element
/// </summary>
public sealed class ListRightWidget : ListWidget {
    private const int _GapBeforeRight = 400;

    public List<Label> LabelsRight { get; private set; } = null!;

    public int FixedWidth { get; set; } = 0;

    public ListRightWidget(Vector2 pos, int capacity) : base(pos, capacity) { }

    public ListRightWidget(Vector2 pos, params string[] rightText) : base(pos, rightText) { }

    protected override void _Setup(Vector2 pos, int capacity) {
        base._Setup(pos, capacity);

        this.LabelsRight = new List<Label>(capacity);
        for (int i = 0; i < capacity; i++) this.LabelsRight.Add(new Label() { Alignment = Alignment.TopRight });
    }

    /// <summary>
    /// Sets the text of the right-side element
    /// </summary>
    public void SetRightText(params string[] rightText) {
        int i = 0;
        for (; i < rightText.Length && i < this.LabelsRight.Count; i++) {
            this.LabelsRight[i].IsVisible = true;
            this.LabelsRight[i].Padding = this.ItemPadding;
            this.LabelsRight[i].Text = rightText[i];
        }

        // New list shorter, blank out remaining LabelsRight
        for (; i < this.LabelsRight.Count; i++) {
            this.LabelsRight[i].IsVisible = false;
            this.LabelsRight[i].Padding = Padding.Zero;
        }
        // New list longer, add more LabelsRight
        for (; i < rightText.Length; i++) {
            this.LabelsRight.Add(new Label() {
                Text = rightText[i],
                Alignment = Alignment.TopRight,
                Padding = this.ItemPadding
            });
        }

        this.OptCount = rightText.Length;

        this.CalcLayout();
    }

    public override void CalcLayout() {
        base.CalcLayout();

        // todo i dont think non-fixed width works correctly
        int prevW = this.FixedWidth != 0 ? this.FixedWidth : this.Width;

        for (int i = 0; i < this.Labels.Count; i++) {
            Label l = this.Labels[i];
            Label lr = this.LabelsRight[i];

            lr.Position = new(this.X + prevW - lr.Padding.R, l.Y);

            int w = l.Width + l.Padding.LR + lr.Width + lr.Padding.LR + _GapBeforeRight;
            if (w > this.Width) this.Width = w;
        }

        if (this.FixedWidth != 0) this.Width = this.FixedWidth;

        this.Origin = this.Data.CalcOrigin();
    }

    public override void OnCreate() {
        base.OnCreate();
        foreach (Label l in this.LabelsRight) l.Create();
    }

    public override void OnDestroy() {
        base.OnDestroy();
        foreach (Label l in this.LabelsRight) l.Destroy();
    }

    public override void Draw(GameTime gameTime) {
        base.Draw(gameTime);

        foreach (Label l in this.LabelsRight) {
            l.Data.Act(gameTime);
            if (_DebugMenu._drawActorOutlines) l.Data.DrawDebug(false);
        }
    }
}
