using System;
using System.Collections.Generic;
using API.Extensions;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Data holder for <c>IActor</c>
/// </summary>
public sealed class ActorData(IActor actor, RenderPriority renderPriority = RenderPriority.B1Med)
{
    public bool IsVisible = true;

    /// <summary>
    /// Priority to draw with. Changes only applied on <c>Stage.Cleanup()</c>
    /// </summary>
    public RenderPriority Priority
    {
        get;
        set
        {
            field = value;
            Stage._needsSorting = true;
        }
    } = renderPriority;

    private Vector2 _position = Vector2.Zero;
    public Vector2 Position
    {
        get => this._position;
        set
        {
            this._position = value;
            this.AnimFrom = this.CalcAnimFrom();
        }
    }
    public float X
    {
        get => this._position.X;
        set
        {
            this._position.X = value;
            this.AnimFrom = this.CalcAnimFrom();
        }
    }
    public float Y
    {
        get => this._position.Y;
        set
        {
            this._position.Y = value;
            this.AnimFrom = this.CalcAnimFrom();
        }
    }

    private Point _size = Point.Zero;
    public Point Size
    {
        get => this._size;
        set => this._size = value;
    }
    public int Width
    {
        get => this._size.X;
        set => this._size.X = value;
    }
    public int Height
    {
        get => this._size.Y;
        set => this._size.Y = value;
    }

    /// <summary>
    /// Padding to apply to this when calling <c>DrawBackground()</c> and arranging it inside of an <c>ILayoutWidget</c>
    /// </summary>
    public Padding Padding;

    public Alignment Alignment
    {
        get;
        set
        {
            field = value;
            this.Origin = this.CalcOrigin();
        }
    } = Alignment.TopLeft;

    /// <summary>
    /// Distance from Position to draw at
    /// </summary>
    public Point Origin = Point.Zero;

    /// <summary>
    /// Animation progress
    /// </summary>
    public Progress Prog;

    /// <summary>
    /// Position to interpolate to/from during create/destroy animation if <c>AnimType</c> is <c>Move</c>
    /// </summary>
    public Vector2 AnimFrom;

    /// <summary>
    /// Direction to interpolate to/from during create/destroy animation if <c>AnimType</c> is <c>Move</c>
    /// </summary>
    public Dir AnimFromDir
    {
        get;
        set
        {
            field = value;
            this.AnimFrom = this.CalcAnimFrom();
        }
    } = Dir.Left;

    /// <summary>
    /// Type of animation to use during create/destroy animation
    /// </summary>
    public AnimType AnimType = AnimType.Move;

    /// <summary>
    /// Speed multiplier. 1f = animation completes in 1s. 2f = 0.5s. Speed is doubled when closing
    /// </summary>
    public float Speed = IActor.DefaultSpeed;

    internal readonly List<Routine> _routines = [];

    /// <summary>
    /// Called when this is added to the stage. Does not add it to the stage
    /// </summary>
    public void Create()
    {
        actor.OnCreate();

        if (this.AnimType != AnimType.None)
        {
            this.AddRoutine(IActor.In);
        }
        else
        {
            this.Prog = Progress.One;
        }
    }

    /// <summary>
    /// Called when this should be removed from the stage
    /// </summary>
    public void Destroy()
    {
        actor.OnDestroy();

        if (this.AnimType != AnimType.None)
        {
            this.AddRoutine(IActor.Out);
        }
        else
        {
            this.Prog = Progress.Zero;
            Stage.Remove(actor);
        }
    }

    /// <summary>
    /// Add a <c>Routine</c> to execute when drawn
    /// </summary>
    /// <param name="routine"><c>Routine</c> to execute when drawn. When it returns true, it's removed from the list</param>
    public void AddRoutine(Routine routine)
    {
        routine.OnStart?.Invoke(actor);
        this._routines.Add(routine);
    }

    private const int _OriginDebugSize = 10;

    /// <summary>
    /// Draws this if it is visible and performs its <c>Routine</c>s
    /// </summary>
    // todo only act if not visible if possible
    public void Act(GameTime gt)
    {
        // Execute routines
        for (int i = 0; i < this._routines.Count; i++)
        {
            if (this._routines[i].OnUpdate(actor, gt))
            {
                this._routines.SwapRemove(i);
            }
        }

        if (!this.IsVisible)
        {
            return;
        }

        actor.Draw(gt);
    }

    public void DrawBackground(Color c, Vector2 minSize)
    {
        Core.ShapeBatch.FillRectangle(
            new((int) (this.Position.X - this.Padding.L - this.Origin.X),
            (int) (this.Position.Y - this.Padding.T - this.Origin.Y)),
            new(Math.Max(minSize.X, this.Width + this.Padding.LR),
            Math.Max(minSize.Y, this.Height + this.Padding.TB)), c);
    }

    public void DrawBackground(Color c)
    {
        this.DrawBackground(c, Vector2.Zero);
    }

    public void DrawDebug(bool drawOrigin = true)
    {
        Color outlineColor = this.Prog == 0
            ? Color.ActorOutlineProg0
            : this.Prog == 1
                ? Color.ActorOutlineProg1
                : Color.ActorOutline;

        (Color, Color) colors = this.IsVisible
            ? (outlineColor, Color.ActorPadding)
            : (new(outlineColor, 0.25f), new(Color.ActorPadding, 0.25f));

        // Origin
        if (drawOrigin)
        {
            Core.ShapeBatch.FillRectangle(this.Position - new Vector2(_OriginDebugSize),
                new(_OriginDebugSize * 2),
                Color.ActorOrigin);
        }

        // Padding
        if (this.Padding != Padding.Zero)
        {
            Core.ShapeBatch.DrawRectangle(this.Position - this.Origin.ToVector2() -
            new Vector2(this.Padding.L, this.Padding.T),
            new(this.Width + this.Padding.LR, this.Height + this.Padding.TB), Color.Trans,
            colors.Item2);
        }

        // Position
        Core.ShapeBatch.DrawRectangle(this.Position - this.Origin.ToVector2(),
            new(this.Width, this.Height),
            Color.Trans, colors.Item1);
    }

    public Point CalcOrigin()
    {
        return this.Alignment switch
        {
            Alignment.TopLeft => Point.Zero,
            Alignment.TopRight => new(this.Size.X, 0),
            Alignment.BottomLeft => new(0, this.Size.Y),
            Alignment.BottomRight => new(this.Size.X, this.Size.Y),
            Alignment.Center => new((int) (this.Size.X * 0.5f), (int) (this.Size.Y * 0.5f)),
            Alignment.Controlled => this.Origin,
            _ => throw new ClosedEnumsWhenException()
        };
    }

    /// <summary>
    /// Updates <c>Prog</c>
    /// </summary>
    /// <returns>Whether the animation is finished</returns>
    public bool UpdateProg(GameTime gt, AnimDirs dir)
    {
        this.Prog = RenderLib.UpdateProg(this.Prog, this.Speed, gt, dir);
        return this.Prog == 1 - Convert.ToInt32((int) dir == -1);
    }

    /// <summary>
    /// Automatically calculates the pos to anim in/out from
    /// </summary>
    public Vector2 CalcAnimFrom()
    {
        return this.AnimFromDir switch
        {
            Dir.Left => new(this.X - this.Width - World.W2 - 500, this.Y),
            Dir.Right => new(this.X + this.Width + World.W2 + 500, this.Y),
            Dir.Up => new(this.X, this.Y - this.Height - 500),
            Dir.Down => new(this.X, this.Y + this.Height + 500),
            _ => throw new ClosedEnumsWhenException()
        };
    }
}
