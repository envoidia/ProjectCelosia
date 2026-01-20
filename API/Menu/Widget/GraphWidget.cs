using System;
using System.Collections.Generic;
using API.Debug;
using API.Graphics;
using API.Input;
using Microsoft.Xna.Framework;

namespace API.Menu.Widget;

/// <summary>
/// A line graph. Only intended for the FPS debug graph, so missing functionality
/// (doesn't support origin changes or varying point amounts/colors)
/// </summary>
/// todo draw background
public sealed class GraphWidget : ILayoutWidget, IActor
{
    public readonly Label LabelX = new();
    public readonly Label LabelY = new();

    private const int _UnitCount = 10;
    public readonly Label[] LabelsUnit = new Label[_UnitCount]; // todo fix

    private const int _MaxPoints = 256;
    private const int _PxPerMs = 50;

    private const int _MetaPadding = 10;
    private const int _MetaPaddingHalf = 5;
    private const int _MetaPadding2 = 20;

    /// <summary>
    /// Total frame time
    /// </summary>
    private readonly Queue<float> _Points0 = new(_MaxPoints);

    /// <summary>
    /// Update time
    /// </summary>
    private readonly Queue<float> _Points1 = new(_MaxPoints);

    /// <summary>
    /// Draw time
    /// </summary>
    private readonly Queue<float> _Points2 = new(_MaxPoints);

    public ActorData Data { get; }

    public GraphWidget(Vector2 pos, Point size, string textX, string textY, RenderPriority priority)
    {
        this.Data = new(this, priority);

        this.Position = pos;
        this.Size = size;

        this.LabelX.Text = textX;
        this.LabelY.Text = textY;

        this.LabelX.Priority = priority;
        this.LabelY.Priority = priority;

        this.LabelY.Rotation = -MathHelper.PiOver2;

        this.CalcLayout();
    }

    public void AddPoint(int index, float point)
    {
        Queue<float> ps = index switch
        {
            0 => this._Points0,
            1 => this._Points1,
            2 => this._Points2,
            _ => throw new ArgumentOutOfRangeException(nameof(index), $"Index must be 0-2; was {index}")
        };

        if (ps.Count == _MaxPoints)
        {
            ps.Dequeue();
        }

        ps.Enqueue(point);
    }

    public void CalcLayout()
    {
        this.LabelX.Position = new(this.X + (this.Width / 2) - this.LabelX.Width / 2,
            this.Y + this.Height + this.Padding.B + this.LabelX.Padding.T);

        this.LabelY.Position = new(this.X - this.LabelY.Height - this.LabelY.Padding.B - this.Padding.L - 35,
            this.Y + (this.LabelY.Size.X / 2) + (this.Height / 2));

        for (int i = 0; i < _UnitCount; i++)
        {
            this.LabelsUnit[i] = new(this.Priority)
            {
                Position = new(this.X - this.Padding.L - _MetaPadding - _MetaPaddingHalf,
                    this.Y - _MetaPadding2 + (i * _PxPerMs)),
                Text = (_UnitCount - i).ToString(),
                Alignment = Alignment.TopRight
            };
        }
    }

    public void Draw(GameTime gt)
    {
        Vector2 bLeft = new(this.X, this.Y + this.Height);

        // Left side line
        Core.ShapeBatch.DrawLine(this.Position, bLeft, 1, Color.White, Color.White);

        // Bottom line
        Core.ShapeBatch.DrawLine(bLeft, new(this.X + this.Width, bLeft.Y),
            1, Color.White, Color.White);

        // Mid-graph lines
        for (int i = 0; i < _UnitCount; i++)
        {
            float y = this.Y - _MetaPadding2 + (i * _PxPerMs) + (this.LabelsUnit[i].Height / 2);
            Core.ShapeBatch.DrawLine(new(this.X + _MetaPaddingHalf, y),
                new(this.X + this.Width, y), 1, Color.Gray, Color.Gray);
        }

        // Points
        drawPoints(this._Points0, Color.Red);
        drawPoints(this._Points1, Color.Blue);
        drawPoints(this._Points2, Color.Green);

        void drawPoints(Queue<float> points, Color c)
        {
            float xOff = _MetaPadding / 2;
            foreach (float point in points)
            {
                float y = this.Y + this.Height - _MetaPaddingHalf - (point * _PxPerMs);
                Core.ShapeBatch.FillCircle(new(this.X + xOff, y),
                    2.5f, c);
                xOff += (this.Width - _MetaPaddingHalf) / (float) _MaxPoints;
            }
        }

        if (InputLib.IsKeyJustPressed(Microsoft.Xna.Framework.Input.Keys.E))
        {
            Console.WriteLine("[{0}]", string.Join(", ", this._Points0));
        }

        // Labels
        this.LabelX.Data.Act(gt);
        this.LabelY.Data.Act(gt);

        foreach (Label l in this.LabelsUnit)
        {
            l.Data.Act(gt);
        }

        if (DebugUtil.DrawActorOutlines)
        {
            this.LabelX.Data.DrawDebug(false);
            this.LabelY.Data.DrawDebug(false);

            foreach (Label l in this.LabelsUnit)
            {
                l.Data.DrawDebug(false);
            }
        }
    }

    public void OnCreate()
    {
        this.LabelX.Create();
        this.LabelY.Create();

        foreach (Label l in this.LabelsUnit)
        {
            l.Data.Create();
        }
    }

    public void OnDestroy()
    {
        this.LabelX.Destroy();
        this.LabelY.Destroy();

        foreach (Label l in this.LabelsUnit)
        {
            l.Destroy();
        }
    }
}
