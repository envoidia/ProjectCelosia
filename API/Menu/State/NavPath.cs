using System.Collections.Generic;
using API.Extensions;

namespace API.Menu.State;

/// <summary>
/// List of <c>IStates</c> that have been traveled through to reach the current location
/// </summary>
public sealed class NavPath {
    /// <summary>
    /// Underlying <c>List</c> of <c>IState</c>s. Avoid accessing directly — use <c>GetState()</c>, <c>Add()</c>, and <c>Remove()</c> instead
    /// </summary>
    internal readonly List<IState> path = [];

    /// <summary>
    /// Get the last <c>IState</c> in the <c>NavPath</c>
    /// </summary>
    public IState GetState() => this.path[^1];

    /// <summary>
    /// Add an <c>IState</c> to the <c>NavPath</c>
    /// </summary>
    public void Add(IState state) {
        this.path.Add(state);
        Core.UpdateInputPrompt();
    }

    /// <summary>
    /// Remove the last <c>IState</c> from the <c>NavPath</c>
    /// </summary>
    public void Remove() {
        this.path.RemoveLast();
        Core.UpdateInputPrompt();
    }
}
