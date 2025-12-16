using System;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public interface IAnimated {
    /// <summary>
    /// Animate in
    /// </summary>
    static readonly Routine In = new(
        static actor => {
            Assert.Is<IAnimated>(actor);
            Assert.Zero(((IAnimated) actor).Prog);
        },

        static (actor, gameTime) => ((IAnimated) actor).Update(gameTime, AnimDirs.In));

    /// <summary>
    /// Animate out
    /// </summary>
    static readonly Routine Out = new(
        static actor => {
            Assert.Is<IAnimated>(actor);
            Assert.One(((IAnimated) actor).Prog);
        },

        static (actor, gameTime) => {
            if (((IAnimated) actor).Update(gameTime, AnimDirs.Out)) {
                Stage.ImmediateRemove(actor);
                return true;
            }

            return false;
        });

    const float DefaultSpeed = 4f;

    /// <summary>
    /// Animation progress
    /// </summary>
    Progress Prog { get; set; }

    /// <summary>
    /// Speed multiplier. 1f = animation completes in 1s. 2f = 0.5s. Speed is doubled when closing
    /// </summary>
    float Speed { get; }
}

public static class AnimatedPrimitiveExtensions {
    extension(IAnimated @this) {
        /// <summary>
        /// Updates <c>Prog</c>
        /// </summary>
        /// <returns>Whether the animation is finished</returns>
        public bool Update(GameTime gameTime, AnimDirs dir) {
            @this.Prog = RenderLib.UpdateProg(@this.Prog, @this.Speed, gameTime, dir);
            return @this.Prog == 1 - Convert.ToInt32((int) dir == -1);
        }
    }
}
