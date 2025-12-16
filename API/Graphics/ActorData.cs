using System.Collections.Generic;
using API.Extensions;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Data holder for <c>IActor</c>
/// </summary>
public sealed class ActorData(IActor actor, RenderPriority renderPriority = RenderPriority.B1Med) {
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Priority to draw with. Changes only applied on <c>Stage.Cleanup()</c>
    /// </summary>
    public RenderPriority Priority {
        get;
        set {
            field = value;
            Stage._needsSorting = true;
        }
    } = renderPriority;

    private Vector2 _position = Vector2.Zero;
    public Vector2 Position {
        get => this._position;
        set => this._position = value;
    }
    public float X {
        get => this._position.X;
        set => this._position.X = value;
    }
    public float Y {
        get => this._position.Y;
        set => this._position.Y = value;
    }

    private Point _size = Point.Zero;
    public Point Size {
        get => this._size;
        set => this._size = value;
    }
    public int Width {
        get => this._size.X;
        set => this._size.X = value;
    }
    public int Height {
        get => this._size.Y;
        set => this._size.Y = value;
    }

    /// <summary>
    /// Padding to apply to this when calling <c>DrawBackground()</c> and arranging it inside of an <c>ILayoutWidget</c>
    /// </summary>
    public Padding Padding { get; set; }

    public Alignment Alignment {
        get;
        set {
            field = value;
            this.Origin = this.CalcOrigin();
        }
    } = Alignment.TopLeft;

    /// <summary>
    /// Distance from Position to draw at
    /// </summary>
    public Point Origin { get; set; } = Point.Zero;

    private readonly List<Routine> _routines = [];

    /// <summary>
    /// Whether this is marked to be removed from the <c>Stage</c> on next <c>Stage.Cleanup()</c>
    /// </summary>
    internal bool _marked = false;

    /// <summary>
    /// Add a <c>Routine</c> to execute when drawn
    /// </summary>
    /// <param name="routine"><c>Routine</c> to execute when drawn. When it returns true, it's removed from the list</param>
    public void AddRoutine(Routine routine) {
        routine.OnStart?.Invoke(actor);
        this._routines.Add(routine);
    }

    /// <summary>
    /// Mark this to be removed from the <c>Stage</c> on next <c>Stage.Cleanup()</c>
    /// </summary>
    public void MarkForRemoval() {
        this._marked = true;
        Stage._needsRemoval = true;
    }

    private const int _OriginDebugSize = 10;

    /// <summary>
    /// Draws this if it is visible and performs its <c>Routine</c>s
    /// </summary>
    public void Act(GameTime gameTime) {
        if (!this.IsVisible) return;

        // Execute routines
        for (int i = 0; i < this._routines.Count; i++) {
            if (this._routines[i].OnUpdate(actor, gameTime)) this._routines.SwapRemove(i);
        }

        actor.Draw(gameTime);
    }

    public void DrawBackground(Color c) => Core.ShapeBatch.FillRectangle(
            new Vector2((int) (this.Position.X - this.Padding.L - this.Origin.X),
            (int) (this.Position.Y - this.Padding.T - this.Origin.Y)),
            new Vector2(this.Width + this.Padding.LR, this.Height + this.Padding.TB), c);

    public void DrawDebug(bool drawOrigin = true) {
        (Color, Color) colors = this.IsVisible
            ? (Colors.ActorOutline, Colors.ActorPadding)
            : (Colors.ActorOutlineInvis, Colors.ActorPaddingInvis);

        // Position
        Core.ShapeBatch.DrawRectangle(this.Position - this.Origin.ToVector2(),
            new Vector2(this.Width, this.Height),
            Colors.Trans, colors.Item1);

        // Padding
        if (this.Padding != Padding.Zero) {
            Core.ShapeBatch.DrawRectangle(this.Position - this.Origin.ToVector2() -
            new Vector2(this.Padding.L, this.Padding.T),
            new Vector2(this.Width + this.Padding.LR, this.Height + this.Padding.TB), Colors.Trans,
            colors.Item2);
        }

        // Marked
        if (this._marked) this.DrawBackground(Colors.ActorMarked);

        // Origin
        if (drawOrigin) {
            Core.ShapeBatch.FillRectangle(this.Position - new Vector2(_OriginDebugSize),
                new Vector2(_OriginDebugSize * 2),
                Colors.ActorOrigin);
        }
    }

    public Point CalcOrigin() => this.Alignment switch {
        Alignment.TopLeft => Point.Zero,
        Alignment.TopRight => new Point(this.Size.X, 0),
        Alignment.BottomLeft => new Point(0, this.Size.Y),
        Alignment.BottomRight => new Point(this.Size.X, this.Size.Y),
        Alignment.Center => new Point((int) (this.Size.X * 0.5f), (int) (this.Size.Y * 0.5f)),
        Alignment.Controlled => this.Origin,
        _ => throw new ClosedEnumsWhenException()
    };
}
