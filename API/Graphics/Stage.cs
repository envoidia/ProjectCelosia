using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public sealed class Stage {
    private readonly List<IRenderable> _actors = [];

    /// <summary>
    /// Draws all the <c>Stage</c>'s visible actors
    /// </summary>
    public void Draw(GameTime gameTime) {
        foreach (IRenderable actor in this._actors) {
            if (actor.IsVisible) actor.Draw(gameTime);
        }
    }

    /// <summary>
    /// Add an actor to the <c>Stage</c>. After you're done adding, call <c>Stage.Sort()</c> to order by <c>RenderPriority</c>
    /// </summary>
    public void Add(IRenderable actor) => this._actors.Add(actor);

    /// <summary>
    /// Remove an actor from the <c>Stage</c>
    /// </summary>
    public void Remove(IRenderable actor) => this._actors.Remove(actor);

    /// <summary>
    /// Sorts the <c>Stage</c>'s actors by their <c>RenderPriority</c>. Call after a batch of additions
    /// </summary>
    public void Sort() =>
        this._actors.Sort((a, b) => 
           ((int) a.RenderPriority).CompareTo((int) b.RenderPriority));
}