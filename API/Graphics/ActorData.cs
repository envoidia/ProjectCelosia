using System.Collections.Generic;
using API.Extensions;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Data holder for <c>IActor</c>
/// </summary>
public sealed class ActorData(IActor actor, RenderPriority renderPriority = RenderPriority.B1Med) {
    /// <summary>
    /// Whether to draw this
    /// </summary>
    // todo is this really needed
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

    /// <summary>
    /// <c>Routine</c> to execute when drawn
    /// </summary>
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
        routine.OnStart(actor);
        this._routines.Add(routine);
    }

    /// <summary>
    /// Mark this to be removed from the <c>Stage</c> on next <c>Stage.Cleanup()</c>
    /// </summary>
    public void MarkForRemoval() {
        this._marked = true;
        Stage._needsRemoval = true;
    }

    /// <summary>
    /// Draws this if it is visible, and performs its <c>Routine</c>s
    /// </summary>
    public void Act(GameTime gameTime) {
        if (!this.IsVisible) return;

        actor.Draw(gameTime);

        // Execute routines
        for (int i = 0; i < this._routines.Count; i++) {
            if (this._routines[i].OnTick(actor, gameTime)) this._routines.SwapRemove(i);
        }
    }
}
