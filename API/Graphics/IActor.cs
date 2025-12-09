using System;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Type that can be rendered and can hold actions to be executed
/// </summary>
public interface IActor {
    /// <summary>
    /// Data holder for this
    /// </summary>
    ActorData Data { get; }

    /// <summary>
    /// Draws this
    /// </summary>
    void Draw(GameTime gameTime);

    /// <summary>
    /// <inheritdoc cref="ActorData.AddRoutine" />
    /// <para>Implement as <c>this.Data.AddRoutine(routine)</c></para>
    /// </summary>
    void AddRoutine(Routine routine);

    /// <summary>
    /// <inheritdoc cref="ActorData.MarkForRemoval" />
    /// <para>Implement as <c>this.Data.MarkForRemoval()</c></para>
    /// </summary>
    void MarkForRemoval();
}
