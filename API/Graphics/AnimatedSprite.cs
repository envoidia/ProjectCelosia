using System;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public class AnimatedSprite : Sprite {
    private int _currentFrame;
    private TimeSpan _elapsed;

    /// <summary>
    /// Gets or Sets the animation for this animated sprite.
    /// </summary>
    private Animation _animation;

    public Animation Animation {
        get => this._animation;
        set {
            this._animation = value;
            this.Region = this._animation.Frames[0];
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

        if (this._elapsed < this._animation.Delay) return;

        this._elapsed -= this._animation.Delay;
        this._currentFrame++;

        if (this._currentFrame >= this._animation.Frames.Count) {
            this._currentFrame = 0;
        }

        this.Region = this._animation.Frames[this._currentFrame];
    }
}