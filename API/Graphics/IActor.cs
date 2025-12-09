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
}

public static class ActorExtensions {
    extension(IActor @this) {
        /// <inheritdoc cref="ActorData.Priority" />
        public RenderPriority Priority {
            get => @this.Data.Priority;
            set => @this.Data.Priority = value;
        }

        /// <inheritdoc cref="ActorData.AddRoutine" />
        public void AddRoutine(Routine routine) => @this.Data.AddRoutine(routine);

        /// <inheritdoc cref="ActorData.MarkForRemoval" />
        public void MarkForRemoval() => @this.Data.MarkForRemoval();

    }
}