using System;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public class AnimatedSprite : Sprite {
    private int _currentFrame;
    private TimeSpan _elapsed;

    /// <summary>
    /// Gets or Sets the animation for this animated sprite.
    /// </summary>
    public Animation Animation {
        get;
        set {
            field = value;
            this.Region = field.Frames[0];
        }
    }

    /// <summary>
    /// Creates a new animated sprite.
    /// </summary>
    public AnimatedSprite() { }

    /// <summary>
    /// Creates a new animated sprite with the specified frames and delay.
    /// </summary>
    /// <param name="animation">The animation for this animated sprite.</param>
    public AnimatedSprite(Animation animation) {
        this.Animation = animation;
    }

    /// <summary>
    /// Updates this animated sprite.
    /// </summary>
    /// <param name="gameTime">A snapshot of the game timing values provided by the framework.</param>
    public void Update(GameTime gameTime) {
        this._elapsed += gameTime.ElapsedGameTime;

        if (this._elapsed < this.Animation.Delay) return;

        this._elapsed -= this.Animation.Delay;
        this._currentFrame++;

        if (this._currentFrame >= this.Animation.Frames.Count) {
            this._currentFrame = 0;
        }

        this.Region = this.Animation.Frames[this._currentFrame];
    }
}