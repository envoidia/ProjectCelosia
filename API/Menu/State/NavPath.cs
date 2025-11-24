using System.Collections.Generic;

namespace API.Menu.State;

/// <summary>
/// List of IStates that have been traveled through to reach the current location
/// </summary>
public sealed class NavPath {
    internal readonly Stack<IState> stack = [];

    public IState GetState() => this.stack.Peek();

    public void Add(IState state) {
        this.stack.Push(state);
        Core.UpdateInputPrompt();
    }

    public void Remove() {
        this.stack.Pop();
        Core.UpdateInputPrompt();
    }
}
