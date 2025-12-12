using System;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public interface IAnimatedPrimitive {
    /// <summary>
    /// Animate in
    /// </summary>
    static readonly Routine In = new(
        static actor => {
            Assert.Is<IAnimatedPrimitive>(actor);
            Assert.Zero(((IAnimatedPrimitive) actor).Prog);
        },

        static (actor, gameTime) => ((IAnimatedPrimitive) actor).Update(gameTime, AnimDirs.In));

    /// <summary>
    /// Animate out
    /// </summary>
    static readonly Routine Out = new(
        static actor => {
            Assert.Is<IAnimatedPrimitive>(actor);
            Assert.One(((IAnimatedPrimitive) actor).Prog);
        },

        static (actor, gameTime) => {
            if (((IAnimatedPrimitive) actor).Update(gameTime, AnimDirs.Out)) {
                Stage.ImmediateRemove(actor);
                return true;
            }

            return false;
        });

    /// <summary>
    /// Animation progress
    /// </summary>
    Progress Prog { get; set; }

    /// <summary>
    /// Speed multiplier. 1f = animation completes in 1s. 2f = 0.5s. Speed is doubled when closing
    /// </summary>
    float Speed { get; set; }
}

public static class AnimatedPrimitiveExtensions {
    extension(IAnimatedPrimitive @this) {
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
