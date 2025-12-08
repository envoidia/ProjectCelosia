using System;
using API.Util;
using Microsoft.Xna.Framework;
using static API.Graphics.Actor;

namespace API.Graphics;

public interface IAnimatedPrimitive {
    /// <summary>
    /// <c>Routine</c> to animate unfolding
    /// </summary>
    static readonly Routine Unfold = static (actor, gameTime) =>
        ((IAnimatedPrimitive) actor).Update(gameTime, AnimDirs.Unfolding);

    /// <summary>
    /// <c>Routine</c> to animate collapsing
    /// </summary>
    static readonly Routine Collapse = static (actor, gameTime) =>
        ((IAnimatedPrimitive) actor).Update(gameTime, AnimDirs.Collapsing);


    /// <summary>
    /// Animation progress
    /// </summary>
    Progress Prog { get; set; }

    /// <summary>
    /// Speed multiplier. 1f = animation completes in 1s. 2f = 0.5s. Speed is doubled when closing
    /// </summary>
    float Speed { get; set; }

    /// <summary>
    /// Updates <c>Prog</c>
    /// </summary>
    /// <returns>Whether the animation is finished</returns>
    bool Update(GameTime gameTime, AnimDirs dir) {
        int isNeg = Convert.ToInt32((int) dir == -1);
        this.Prog += (float) (gameTime.ElapsedGameTime.TotalSeconds * (int) dir * this.Speed * (1 + isNeg));
        return this.Prog == 1 - isNeg;
    }
}
