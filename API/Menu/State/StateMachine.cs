using System.Collections.Generic;
using System.Linq;
using API.Extensions;
using API.Graphics;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

/// <summary>
/// List of <c>States</c> that have been traveled through to reach the current location
/// </summary>
public static class StateMachine {
    private static readonly List<State> _Path = [];

    private static readonly Label _InputPrompt = new() {
        Position = World.Vec - new Vector2(10, 10),
        Alignment = Alignment.BottomRight,
        HasBackground = true,
        Priority = Priority.Highest
    };

    /// <returns>
    /// The last <c>State</c> in the <c>NavPath</c>
    /// </returns>
    public static State GetState() => _Path[^1];

    /// <summary>
    /// Add an <c>State</c> to the <c>NavPath</c>
    /// </summary>
    public static void Add(State state) {
        state.Create();
        _Path.Add(state);
        UpdateInputPrompt();
    }

    /// <summary>
    /// Remove the last <c>State</c> from the <c>NavPath</c>
    /// </summary>
    public static void Remove() {
        _Path[^1].Destroy();
        _Path.RemoveLast();
        UpdateInputPrompt();
    }

    /// <summary>
    /// Update the input prompt <c>Label</c> in the bottom-right corner
    /// </summary>
    public static void UpdateInputPrompt() => _InputPrompt.Text = GetState().GetInputPrompt();

    public new static string ToString() => string.Join(", ", [.. _Path.Select(s => s.Name)]);
}
