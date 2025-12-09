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

    /// <inheritdoc cref="ActorData.Priority" />
    /// Implement as get => this.Data.Priority; set => this.Data.Priority = value;
    RenderPriority Priority { get; set; }

    /// <summary>
    /// Draws this
    /// </summary>
    void Draw(GameTime gameTime);

    /// <inheritdoc cref="ActorData.AddRoutine" />
    /// Implement as this.Data.AddRoutine(routine)
    void AddRoutine(Routine routine);

    /// <inheritdoc cref="ActorData.MarkForRemoval" />
    /// Implement as this.Data.MarkForRemoval()
    void MarkForRemoval();
}
