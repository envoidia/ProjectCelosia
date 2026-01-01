using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// Type that can be rendered and can hold actions to be executed
/// </summary>
public interface IActor
{
    /// <summary>
    /// Data holder for this
    /// </summary>
    ActorData Data { get; }

    /// <summary>
    /// Called when this is added to the stage. Should only be called from <c>ActorData.Create()</c>
    /// In most cases, will be blank
    /// </summary>
    void OnCreate();

    /// <summary>
    /// Called when this should be removed from the stage. Should only be called from <c>ActorData.Create()</c>
    /// In most cases, will be blank
    /// </summary>
    void OnDestroy();

    /// <summary>
    /// Draws this
    /// </summary>
    void Draw(GameTime gt);

    /// <summary>
    /// Animate in
    /// </summary>
    static readonly Routine In = new(
        static actor => Assert.Zero(actor.Prog),
        static (actor, gt) => actor.UpdateProg(gt, AnimDirs.In));

    /// <summary>
    /// Animate out
    /// </summary>
    static readonly Routine Out = new(
        static actor => Assert.One(actor.Prog),

        static (actor, gt) =>
        {
            if (actor.UpdateProg(gt, AnimDirs.Out))
            {
                Stage.Remove(actor);
                return true;
            }

            return false;
        });

    const float DefaultSpeed = 4f;
}

public static class ActorExtensions
{
    extension(IActor @this)
    {
        public bool IsVisible
        {
            get => @this.Data.IsVisible;
            set => @this.Data.IsVisible = value;
        }

        /// <inheritdoc cref="ActorData.Priority" />
        public RenderPriority Priority
        {
            get => @this.Data.Priority;
            set => @this.Data.Priority = value;
        }

        public Vector2 Position
        {
            get => @this.Data.Position;
            set => @this.Data.Position = value;
        }
        public float X
        {
            get => @this.Data.X;
            set => @this.Data.X = value;
        }
        public float Y
        {
            get => @this.Data.Y;
            set => @this.Data.Y = value;
        }

        public Point Size
        {
            get => @this.Data.Size;
            set => @this.Data.Size = value;
        }
        public int Width
        {
            get => @this.Data.Width;
            set => @this.Data.Width = value;
        }
        public int Height
        {
            get => @this.Data.Height;
            set => @this.Data.Height = value;
        }

        public Alignment Alignment
        {
            get => @this.Data.Alignment;
            set => @this.Data.Alignment = value;
        }

        public Point Origin
        {
            get => @this.Data.Origin;
            set => @this.Data.Origin = value;
        }

        /// <inheritdoc cref="ActorData.Padding" />
        public Padding Padding
        {
            get => @this.Data.Padding;
            set => @this.Data.Padding = value;
        }

        /// <inheritdoc cref="ActorData.Prog" />
        public Progress Prog
        {
            get => @this.Data.Prog;
            set => @this.Data.Prog = value;
        }

        /// <inheritdoc cref="ActorData.AnimFrom" />
        public Vector2 AnimFrom
        {
            get => @this.Data.AnimFrom;
            set => @this.Data.AnimFrom = value;
        }

        /// <inheritdoc cref="ActorData.AnimFromDir" />
        public Dir AnimFromDir
        {
            get => @this.Data.AnimFromDir;
            set => @this.Data.AnimFromDir = value;
        }

        /// <inheritdoc cref="ActorData.AnimType" />
        public AnimType AnimType
        {
            get => @this.Data.AnimType;
            set => @this.Data.AnimType = value;
        }

        /// <inheritdoc cref="ActorData.Speed" />
        public float Speed
        {
            get => @this.Data.Speed;
            set => @this.Data.Speed = value;
        }

        /// <inheritdoc cref="ActorData.Create" />
        public void Create()
        {
            @this.Data.Create();
        }

        /// <inheritdoc cref="ActorData.Destroy" />
        public void Destroy()
        {
            @this.Data.Destroy();
        }

        /// <inheritdoc cref="ActorData.AddRoutine" />
        public void AddRoutine(Routine routine)
        {
            @this.Data.AddRoutine(routine);
        }

        public bool UpdateProg(GameTime gt, AnimDirs dir)
        {
            return @this.Data.UpdateProg(gt, dir);
        }
    }
}