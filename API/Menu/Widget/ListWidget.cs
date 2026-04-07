using System;
using System.Collections.Generic;
using System.Linq;
using API.Debug;
using API.Graphics;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Menu.Widget;

/* todo:
    - internal label alignment setting
    - maybe make every option an IComponent and make them hold Labels?
*/
public sealed class ListWidget : ILayoutWidget, IInputWidget, IActor
{
    public List<Label> LabelsL { get; private set; } = null!;

    private const int _GapBeforeRight = 400;

    /// <summary>
    /// Whether this has a right-side component
    /// </summary>
    public bool HasRight
    {
        get
        {
            return this.LabelsR is not null;
        }
    }

    public List<Label> LabelsR { get; private set; } = null!;

    /// <summary>
    /// Set width for this. 0 means dynamic width.
    /// Call <c>CalcLayout</c> after changing
    /// </summary>
    public int FixedWidth = 0;

    /// <summary>
    /// Amount to add to X per option.
    /// You probably want <c>NormalSlant</c> or 0. Other numbers may not function properly.
    /// Call <c>CalcLayout</c> after changing
    /// </summary>
    public int Slant = 0;

    public const int NormalSlant = -14;

    /// <summary>
    /// Background might look weird if slant isn't <c>NormalSlant</c>
    /// </summary>
    public bool HasBackground = false;

    /// <summary>
    /// Options displayed before truncating with scrollbar. If <c>NoLimit</c>, displays all
    /// Call <c>CalcLayout</c> after changing
    /// </summary>
    public int HeightLimit = NoLimit;

    public const int NoLimit = 0;

    /// <summary>
    /// Indices scrolled down from the top
    /// </summary>
    public int Scroll = 0;

    /// <summary>
    /// Used to interpolate the scrollbar
    /// </summary>
    public Vector2 LastScrollbarPos = new(UninitializedScrollbarPos);

    /// <summary>
    /// Scrollbar starting position
    /// </summary>
    public const int UninitializedScrollbarPos = -1;

    public Padding ItemPadding
    {
        get;
        init
        {
            field = value;

            foreach (Label l in this.LabelsL)
            {
                l.Padding = value;
            }
        }
    } = new(40, 20, 10, 10);

    public SelectionType PrefDir
    {
        get
        {
            return SelectionType.Vert;
        }
    }

    public SelectionType CurDir { get; set; } = SelectionType.None;

    public bool CheckInput { get; set; } = true;

    public int Index { get; set; } = 0;

    public int OptCount
    {
        get;
        private set
        {
            field = value;
            this.Index = Math.Clamp(this.Index, 0, Math.Max(value - 1, 0));
        }
    } = 0;

    public Action<int>? OnChangeIndex { get; set; }

    /// <summary>
    /// Animation progress per-item
    /// </summary>
    public List<Progress> Progs { get; private set; } = null!;

    public ActorData Data { get; private set; } = null!;

    /// <inheritdoc cref="ActorData.AnimFromDir" />
    public Dir AnimFromDir
    {
        get
        {
            return this.Data.AnimFromDir;
        }
        set
        {
            this.Data.AnimFromDir = value;

            foreach (Label l in this.LabelsL)
            {
                l.AnimFromDir = value;
            }
        }
    }

    public ListWidget(Vector2 pos, bool useRight, int capacity, RenderPriority priority = RenderPriority.B1Med)
    {
        this._Setup(pos, capacity, useRight);

        this.Data.Priority = priority;
        this.Data.OnCreate = this.OnCreate;
        this.Data.OnDestroy = this.OnDestroy;
    }

    public ListWidget(Vector2 pos, bool useRight, RenderPriority priority = RenderPriority.B1Med,
        params ReadOnlySpan<string> textL)
    {
        this._Setup(pos, textL.Length, useRight);

        this.OptCount = textL.Length;

        for (int i = 0; i < this.OptCount; i++)
        {
            this.LabelsL[i].Text = textL[i];
        }

        this.CalcLayout();

        this.Data.Priority = priority;
        this.Data.OnCreate = this.OnCreate;
        this.Data.OnDestroy = this.OnDestroy;

        this.OnChangeIndex = this._OnChangeIndex;
    }

    private void _Setup(Vector2 pos, int capacity, bool useRight)
    {
        this.Data = new ActorData(this, RenderPriority.B2Med);

        this.Position = pos;
        this.Size = Point.Zero;

        this.LabelsL = new List<Label>(capacity);

        for (int i = 0; i < capacity; i++)
        {
            this.LabelsL.Add(new Label()
            {
                Padding = this.ItemPadding
            });
        }

        if (useRight)
        {
            this.LabelsR = new List<Label>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                this.LabelsR.Add(new Label()
                {
                    Alignment = Alignment.TopRight
                });
            }
        }

        this.Progs = [.. Enumerable.Repeat(Progress.Zero, capacity)];
    }

    private void _OnChangeIndex(int newIndex)
    {
        if (this.HeightLimit == NoLimit)
        {
            return;
        }

        // Check margin
        if (newIndex == 0)
        {
            this.Scroll = 0;
        }
        else if (newIndex == this.OptCount - 1)
        {
            this.Scroll = this.OptCount - this.HeightLimit;
        }
        else if (newIndex <= this.Scroll)
        {
            this.Scroll--;
        }
        else if (newIndex >= this.Scroll + this.HeightLimit - 1)
        {
            this.Scroll++;
        }

        // Make other labels invis, reposition ones in range
        int h = 0;
        int prevW = this.FixedWidth != 0 ? this.FixedWidth : this.Width;
        int shownCount = 0;

        for (int i = 0; i < this.OptCount; i++)
        {
            Label l = this.LabelsL[i];

            if (!this._IndexIsShown(i))
            {
                l.IsVisible = false;

                if (this.HasRight)
                {
                    this.LabelsR[i].IsVisible = false;
                }

                continue;
            }

            shownCount++;

            l.IsVisible = true;
            l.Prog = Progress.One;

            h += l.Padding.T;
            l.Position = this.Position + new Vector2(l.Padding.L + (this.Slant * shownCount), h);

            if (this.HasRight)
            {
                Label lr = this.LabelsR[i];

                lr.IsVisible = true;
                lr.Prog = Progress.One;

                lr.Position = new(this.X + prevW - lr.Padding.R + (this.Slant * shownCount), l.Y);
            }

            h += l.Height + l.Padding.B;
        }

        this.Height = h;
    }

    private bool _IndexIsShown(int index)
    {
        return this.HeightLimit == NoLimit || (index >= this.Scroll && index <= this.Scroll + this.HeightLimit - 1);
    }

    /// <summary>
    /// Call <c>CalcLayout</c> after changing
    /// </summary>
    public void SetTextL(params ReadOnlySpan<string> text)
    {
        this._SetText(this.LabelsL, text, Alignment.TopLeft);
    }

    /// <inheritdoc cref="SetTextL" />
    public void SetTextR(params ReadOnlySpan<string> text)
    {
        Assert.NotNull(this.LabelsR);
        this._SetText(this.LabelsR, text, Alignment.TopRight);
    }

    private void _SetText(List<Label> labels, ReadOnlySpan<string> text, Alignment align)
    {
        this.OptCount = text.Length;

        int i = 0;
        for (; i < text.Length && i < this.LabelsL.Count; i++)
        {
            labels[i].IsVisible = true;
            labels[i].Padding = this.ItemPadding;
            labels[i].Text = text[i];
            labels[i].Prog = this.Prog;
        }

        for (; i < this.LabelsL.Count; i++)
        {
            labels[i].IsVisible = false;
            labels[i].Padding = Padding.Zero;
            this.Progs[i] = Progress.Zero;
        }

        for (; i < text.Length; i++)
        {
            labels.Add(new Label()
            {
                Text = text[i],
                Alignment = align,
                Padding = this.ItemPadding,
                Prog = this.Prog
            });

            this.Progs.Add(Progress.Zero);
        }
    }

    public void CalcLayout()
    {
        this.Size = Point.Zero;

        // todo i dont think non-fixed width works correctly (this must be set after all L calcs)
        int prevW = this.FixedWidth != 0 ? this.FixedWidth : this.Width;
        int shownCount = 0;

        for (int i = 0; i < this.OptCount; i++)
        {
            if (!this._IndexIsShown(i))
            {
                continue;
            }

            shownCount++;

            Label l = this.LabelsL[i];

            this.Height += l.Padding.T;
            l.Position = this.Position + new Vector2(l.Padding.L + (this.Slant * shownCount), this.Height);
            this.Height += l.Height + l.Padding.B;

            int w = l.Width + l.Padding.LR;

            if (this.HasRight)
            {
                Label lr = this.LabelsR[i];

                lr.Position = new(this.X + prevW - lr.Padding.R + (this.Slant * shownCount), l.Y);

                w += lr.Width + lr.Padding.LR + _GapBeforeRight;
            }

            if (w > this.Width)
            {
                this.Width = w;
            }
        }

        if (this.FixedWidth != 0)
        {
            this.Width = this.FixedWidth;
        }

        this.Origin = this.Data.CalcOrigin();

        if (this.HasRight)
        {
            Assert.Eq(this.LabelsL.Count, this.LabelsR.Count);
        }
    }

    public void Input(GameTime gt)
    {
        this.Index = this.CheckInput();
    }

    public void OnCreate()
    {
        for (int i = 0; i < this.OptCount; i++)
        {
            this.LabelsL[i].Create();

            if (this.HasRight)
            {
                this.LabelsR[i].Create();
            }
        }
    }

    public void OnDestroy()
    {
        for (int i = 0; i < this.OptCount; i++)
        {
            this.LabelsL[i].Destroy();

            if (this.HasRight)
            {
                this.LabelsR[i].Destroy();
            }
        }
    }

    public void Draw(GameTime gt)
    {
        // todo cleanup + move input out of draw
        // + dont have the cursor immediately vanish when optcount goes from n>0 to =0

        float x = MathHelper.SmoothStep(this.AnimFrom.X,
            this.Position.X - this.Padding.L, (float) this.Prog);

        bool drawScrollbar = this.HeightLimit != NoLimit && this.OptCount > this.HeightLimit;

        int extraWidth = drawScrollbar ? 23 : 0;

        if (this.HasBackground)
        {
            int maxScroll = this.HeightLimit == NoLimit ? 0 : this.OptCount - this.HeightLimit;

            RenderLib.DrawParallelogram(
                new(x + (this.Slant * (this.OptCount - maxScroll - 1)),
                this.Position.Y - this.Padding.T),
                new(this.Width + this.Padding.LR + extraWidth, this.Height),
                this.Origin, Settings.Theme.Bg, Settings.Theme.Fg,
                RenderLib.BgOutlineThickness, RenderLib.DefaultSlant, RenderLib.DefaultSlant,
                Progress.One);
        }

        int h = 0;

        for (int i = 0; i < this.OptCount; i++)
        {
            this.Progs[i] = RenderLib.UpdateProg(this.Progs[i], IActor.DefaultSpeed, gt,
                i == this.Index ? AnimDirs.In : AnimDirs.Out);

            if (!this._IndexIsShown(i))
            {
                continue;
            }

            Label l = this.LabelsL[i];

            // Cursor
            if (this.Progs[i] != 0)
            {
                RenderLib.DrawParallelogram(
                    new(x + (this.Slant * (i - this.Scroll)), this.Position.Y + h - this.Padding.T),
                    new(this.Width + this.Padding.LR + extraWidth, l.Height + l.Padding.TB + this.Padding.TB),
                    this.Origin, Settings.Theme.Accent, Color.Red,
                    0, RenderLib.DefaultSlant, RenderLib.DefaultSlant,
                    Progress.Min(this.Prog, this.Progs[i]));
            }

            h += l.Height + l.Padding.TB;

            l.Data.Act(gt);

            if (DebugUtil.DrawActorOutlines)
            {
                l.Data.DrawDebug(false);
            }

            if (this.HasRight)
            {
                Label lr = this.LabelsR[i];
                lr.Data.Act(gt);

                if (DebugUtil.DrawActorOutlines)
                {
                    lr.Data.DrawDebug(false);
                }
            }
        }

        // Scrollbar
        // The magic numbers throughout this code are just whatever values made the spacing look right to me
        if (drawScrollbar)
        {
            // Portion currently displayed
            float ratio = Math.Min((float) this.HeightLimit / this.OptCount, 1);

            // Maximum range for the bar to move. Slightly less than height so it leaves a margin on the edges
            float range = this.Height - 10;

            float barLength = range * ratio;
            range -= barLength;

            // 0 = bottom; 1 = top
            float scrollAmt = Math.Abs((float) this.Scroll / Math.Max(this.OptCount - this.HeightLimit, 0) - 1);

            float y1 = this.Y - 5 + this.Height - barLength - (range * scrollAmt);

            float centerY = y1 + (barLength / 2f);
            float x1 = this.X + this.Width + this.Padding.LR
               + (this.Slant == 0 ? -25 : (((this.Y - 5) - centerY) / RenderLib.DefaultSlant))
               + (this.HasRight ? -18 : 15);

            Vector2 pos = new(x1, y1);

            if (this.LastScrollbarPos.X != UninitializedScrollbarPos)
            {
                pos = Vector2.SmoothStep(this.LastScrollbarPos, pos, RenderLib.GetInterpolationAmount(gt));
            }

            this.LastScrollbarPos = pos;

            Core.ShapeBatch.DrawRectangle(pos, new(10, barLength),
                Settings.Theme.Fg, Color.Red, 0f, rotation: this.Slant / -84.85f);
        }

        // Disabled input overlay
        if (DebugUtil.DrawActorOutlines && !this.CheckInput)
        {
            this.Data.DrawBackground(Color.ActorDisabledInput);
        }
    }
}
