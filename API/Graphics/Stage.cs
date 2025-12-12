using System;
using System.Collections.Generic;
using System.Linq;
using API.Menu;
using API.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ResolutionBuddy;

namespace API.Graphics;

/// <summary>
/// List of active <c>Actor</c>s with helper methods
/// </summary>
public static class Stage {
    // todo decide starting size
    private static readonly List<IActor> _Actors = new(30);

    internal static bool _needsSorting = false;
    internal static bool _needsRemoval = false;

    /// <summary>
    /// Draws all visible <c>Actor</c>s and performs their <c>Routine</c>s
    /// </summary>
    public static void Act(GameTime gameTime) {
        int i = _Actors.Count - 1;

        // The only reason this gross stuff has to happen is bc SpriteBatch and ShapeBatch are separate
        begin();
        for (; i >= 0 && _Actors[i].Data.Priority < RenderPriority.B2Low; i--) _Actors[i].Data.Act(gameTime);
        end();
        begin();
        for (; i >= 0 && _Actors[i].Data.Priority < RenderPriority.B3Low; i--) _Actors[i].Data.Act(gameTime);
        end();
        begin();
        for (; i >= 0; i--) _Actors[i].Data.Act(gameTime);
        end();

        // Debug overlay (F3)
        if (DebugMenu._drawActorOutlines) {
            begin();
            foreach (IActor a in _Actors) a.Data.DrawDebug();
            end();
        }

        static void begin() {
            Core.ShapeBatch.Begin(Resolution.TransformationMatrix());
            Core.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
            SamplerState.PointClamp, null, null, null,
            Resolution.TransformationMatrix());
        }

        static void end() {
            Core.ShapeBatch.End();
            Core.SpriteBatch.End();
        }
    }

    /// <summary>
    /// Add an <c>Actor</c>. After you're done adding, call <c>Cleanup()</c> to order by <c>RenderPriority</c>
    /// </summary>
    public static void Add(IActor actor) {
        Assert.DoesntContain(_Actors, actor);

        _Actors.Add(actor);
    actor.Data._marked = false;
        actor.Create();

        _needsSorting = true;
    }

    /// <summary>
    /// Add a range of <c>Actor</c>s. After you're done adding, call <c>Cleanup()</c> to order by <c>RenderPriority</c>
    /// </summary>
    public static void AddRange(params IEnumerable<IActor> actors) {
        foreach (IActor actor in actors) {
            Assert.DoesntContain(_Actors, actor);

            _Actors.Add(actor);
            actor.Data._marked = false;
            actor.Create();
        }

        _needsSorting = true;
    }

    /// <summary>
    /// Immediately removes an <c>Actor</c>. Prefer <c>Actor.MarkForRemoval()</c> when able
    /// </summary>
    public static void ImmediateRemove(IActor actor) => _Actors.Remove(actor);

    /// <summary>
    /// Applies sorting and removal
    /// </summary>
    public static void Cleanup() {
        if (_needsRemoval) {
            _Actors.RemoveAll(a => a.Data._marked);

            _needsRemoval = false;
        }

        if (!_needsSorting) return;

        _Actors.Sort((a, b) =>
           ((int) b.Data.Priority).CompareTo((int) a.Data.Priority));

        _needsSorting = false;
    }

    public static int ActorCount() => _Actors.Count;

    public new static string ToString() =>
        string.Join("\n", [.. _Actors.Select(a => a.ToString())]);
}