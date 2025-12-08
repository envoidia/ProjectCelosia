using System;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// An action for an <c>Actor</c> to execute
/// </summary>
/// <param name="OnStart">Executes on start</param>
/// <param name="OnTick">Executes every frame</param>
public sealed record Routine(Routine.Start OnStart, Routine.Tick OnTick) {

    /// <summary>
    /// Executes on start
    /// </summary>
    public delegate void Start(Actor actor);

    /// <summary>
    /// Executes every frame
    /// </summary>
    /// <returns>Whether this has ended and should be removed</returns>
    public delegate bool Tick(Actor actor, GameTime gameTime);
}
