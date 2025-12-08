using System;
using System.Collections.Generic;
using API.Extensions;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Type that can be rendered and can hold actions to be executed
/// </summary>
public abstract class Actor {
    /// <summary>
    /// Whether to draw
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Priority to draw with. Only applied on <c>Stage.Sort()</c>, and only within the <c>Stage</c>
    /// </summary>
    public RenderPriority RenderPriority { get; set; } = RenderPriority.Low;

    /// <summary>
    /// An action for an <c>Actor</c> to execute every frame. 
    /// </summary>
    /// <returns>Whether this <c>Routine</c> has ended and should be removed</returns>
    public delegate bool Routine(Actor actor, GameTime gameTime);

    /// <summary>
    /// <c>Routine</c> to execute when drawn
    /// </summary>
    private readonly List<Routine> _routines = [];

    /// <summary>
    /// Add a <c>Routine</c> to execute when drawn
    /// </summary>
    /// <param name="routine"><c>Routine</c> to execute when drawn. When it returns true, it's removed from the list</param>
    public void AddRoutine(Routine routine) => this._routines.Add(routine);

    /// <summary>
    /// Draws this <c>Actor</c> if it is visible, and performs its <c>Routine</c>s
    /// </summary>
    public void Act(GameTime gameTime) {
        if (!this.IsVisible) return;

        this.Draw(gameTime);

        // Execute routines
        if (this._routines.Count == 0) return;

        for (int i = 0; i < this._routines.Count; i++) {
            if (this._routines[i](this, gameTime)) this._routines.SwapRemove(i);
        }
    }

    /// <summary>
    /// Draws this
    /// </summary>
    public abstract void Draw(GameTime gameTime);
}