using System;
using System.Collections.Generic;
using System.Linq;
using API.Input;
using Microsoft.Xna.Framework;

namespace API.Graphics;

/// <summary>
/// List of active <c>Actor</c>s with helper methods
/// </summary>
public static class Stage {
    private static readonly List<Actor> _Actors = [];

    /// <summary>
    /// Whether sorting is needed
    /// </summary>
    internal static bool _needsSorting = false;

    /// <summary>
    /// Whether removal is needed
    /// </summary>
    internal static bool _needsRemoval = false;

    /// <summary>
    /// Draws all visible <c>Actor</c>s and performs their <c>Routine</c>s
    /// </summary>
    public static void Act(GameTime gameTime) {
        for (int i = _Actors.Count - 1; i >= 0; i--) _Actors[i].Act(gameTime);
    }

    /// <summary>
    /// Add an <c>Actor</c>. After you're done adding, call <c>Cleanup()</c> to order by <c>RenderPriority</c>
    /// </summary>
    public static void Add(Actor actor) {
        _Actors.Add(actor);
        _needsSorting = true;
    }

    /// <summary>
    /// Add a range of <c>Actor</c>s. After you're done adding, call <c>Cleanup()</c> to order by <c>RenderPriority</c>
    /// </summary>
    public static void AddRange(params IEnumerable<Actor> actors) {
        _Actors.AddRange(actors);
        _needsSorting = true;
    }

    /// <summary>
    /// Immediately removes an <c>Actor</c>. Prefer <c>Actor.MarkForRemoval()</c> when able
    /// </summary>
    public static void ImmediateRemove(Actor actor) => _Actors.Remove(actor);

    /// <summary>
    /// Applies sorting and removal
    /// </summary>
    public static void Cleanup() {
        if (_needsRemoval) {
            _Actors.RemoveAll(a => a._marked);

            _needsRemoval = false;
        }

        if (!_needsSorting) return;

        _Actors.Sort((a, b) =>
           ((int) b.Priority).CompareTo((int) a.Priority));

        Console.WriteLine("actors sorted!");

        _needsSorting = false;
    }

    public new static string ToString() =>
        string.Join("\n", [.. _Actors.Select(a => a.ToString())]);
}