using System.Collections.Generic;
using API.Extensions;
using API.Graphics;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

/// <summary>
/// List of <c>States</c> that have been traveled through to reach the current location
/// </summary>
public static class NavPath {
    /// <summary>
    /// Underlying <c>List</c> of <c>State</c>s. Avoid accessing directly — use <c>GetState()</c>, <c>Add()</c>, and <c>Remove()</c> instead
    /// </summary>
    public static readonly List<State> Path = []; // todo private

    private static readonly Label _InputPrompt = new(Core.StageBase) {
        Position = World.Vec - new Vector2(10, 10),
        Alignment = Alignment.BottomRight,
        HasBackground = true,
        RenderPriority = RenderPriority.Super
    };

    /// <returns>
    /// The last <c>State</c> in the <c>NavPath</c>
    /// </returns>
    public static State GetState() => Path[^1];

    /// <summary>
    /// Add an <c>State</c> to the <c>NavPath</c>
    /// </summary>
    public static void Add(State state) {
        state.Create();
        Path.Add(state);
        UpdateInputPrompt();
    }

    /// <summary>
    /// Remove the last <c>State</c> from the <c>NavPath</c>
    /// </summary>
    public static void Remove() {
        Path[^1].Destroy();
        Path.RemoveLast();
        UpdateInputPrompt();
    }

    /// <summary>
    /// Update the input prompt <c>Label</c> in the bottom-right corner
    /// </summary>
    public static void UpdateInputPrompt() => _InputPrompt.Text = GetState().GetInputPrompt();
}
