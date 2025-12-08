using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public sealed class Stage {
    private readonly List<Actor> _Actors = [];

    /// <summary>
    /// Draws all the <c>Stage</c>'s visible actors and performs their actions
    /// </summary>
    public void Draw(GameTime gameTime) {
        foreach (Actor actor in this._Actors) actor.Act(gameTime);
    }

    /// <summary>
    /// Add an actor to the <c>Stage</c>. After you're done adding, call <c>Stage.Sort()</c> to order by <c>RenderPriority</c>
    /// </summary>
    public void Add(Actor actor) => this._Actors.Add(actor);

    /// <summary>
    /// Remove an actor from the <c>Stage</c>
    /// </summary>
    public void Remove(Actor actor) => this._Actors.Remove(actor);

    /// <summary>
    /// Sorts the <c>Stage</c>'s actors by their <c>RenderPriority</c>. Call after a batch of additions
    /// </summary>
    public void Sort() =>
        this._Actors.Sort((a, b) =>
           ((int) a.RenderPriority).CompareTo((int) b.RenderPriority));
}

public static class Stages {
    /// <summary>
    /// <c>Stage</c> that's always drawn first
    /// </summary>
    public static readonly Stage Base = new();

    /// <summary>
    /// <c>Stage</c> that's only drawn during battle
    /// </summary>
    public static readonly Stage Battle = new();

    /// <summary>
    /// <c>Stage</c> that's only drawn in the inspect menu
    /// </summary>
    public static readonly Stage Inspect = new();

    /// <summary>
    /// <c>Stage</c> that's only drawn with a popup
    /// </summary>
    public static readonly Stage Popup = new();

    /// <summary>
    /// <c>Stage</c> that's always drawn last
    /// </summary>
    public static readonly Stage Super = new();
}