using System;
using API.Util;
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
    /// Called when this is added to the stage.
    /// In most cases, will be blank
    /// </summary>
    void Create();

    /// <summary>
    /// Called when this should be removed from the stage.
    /// In most cases, should be implemented as <c>this.MarkForRemoval()</c>
    /// </summary>
    void Destroy();

    /// <summary>
    /// Draws this
    /// </summary>
    void Draw(GameTime gameTime);
}

public static class ActorExtensions {
    extension(IActor @this) {
        public bool IsVisible {
            get => @this.Data.IsVisible;
            set => @this.Data.IsVisible = value;
        }

        /// <inheritdoc cref="ActorData.Priority" />
        public RenderPriority Priority {
            get => @this.Data.Priority;
            set => @this.Data.Priority = value;
        }

        public Vector2 Position {
            get => @this.Data.Position;
            set => @this.Data.Position = value;
        }
        public float X {
            get => @this.Data.X;
            set => @this.Data.X = value;
        }
        public float Y {
            get => @this.Data.Y;
            set => @this.Data.Y = value;
        }

        public Point Size {
            get => @this.Data.Size;
            set => @this.Data.Size = value;
        }
        public int Width {
            get => @this.Data.Width;
            set => @this.Data.Width = value;
        }
        public int Height {
            get => @this.Data.Height;
            set => @this.Data.Height = value;
        }

        public Alignment Alignment {
            get => @this.Data.Alignment;
            set => @this.Data.Alignment = value;
        }

        public Point Origin {
            get => @this.Data.Origin;
            set => @this.Data.Origin = value;
        }

        /// <inheritdoc cref="ActorData.Padding" />
        public Padding Padding {
            get => @this.Data.Padding;
            set => @this.Data.Padding = value;
        }

        /// <inheritdoc cref="ActorData.AddRoutine" />
        public void AddRoutine(Routine routine) => @this.Data.AddRoutine(routine);

        /// <inheritdoc cref="ActorData.MarkForRemoval" />
        public void MarkForRemoval() => @this.Data.MarkForRemoval();

    }
}