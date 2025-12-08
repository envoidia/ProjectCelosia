using System;
using API.Util;
using Microsoft.Xna.Framework;
using static API.Graphics.Actor;

namespace API.Graphics;

public interface IAnimatedPrimitive {
    /// <summary>
    /// <c>Routine</c> to animate in
    /// </summary>
    static readonly Routine In = static (actor, gameTime) =>
        ((IAnimatedPrimitive) actor).Update(gameTime, AnimDirs.In);

    /// <summary>
    /// <c>Routine</c> to animate out
    /// </summary>
    static readonly Routine Out = static (actor, gameTime) => {
        if (((IAnimatedPrimitive) actor).Update(gameTime, AnimDirs.Out)) {
            Stage.ImmediateRemove(actor);
            return true;
        }

        return false;
    };

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
