using System;
using System.Collections.Generic;

namespace API.Debug;

/// <summary>
/// A command to be executed by the debug console
/// </summary>
public sealed class Command
{
    internal static readonly Dictionary<string, Command> _Commands = [];

    /// <summary>
    /// Function this should execute
    /// </summary>
    public readonly Action<string[]> Action;

    /// <summary>
    /// Hints for this' arguments.
    /// Eg hints of <c>[foo, bar, lorem]</c> would display <c>commandname| [foo] [bar] [lorem]</c> as you type
    /// </summary>
    public readonly string[] Hints;

    private Command(Action<string[]> action, string[] hints)
    {
        this.Action = action;
        this.Hints = hints;
    }

    /// <summary>
    /// Creates a <c>Command</c> with the specified name and adds it to the command registry, unless the name is already used
    /// </summary>
    /// <returns>Whether the Command was created and registered</returns>
    public static bool Create(string name, Action<string[]> action, string[] hints)
    {
        if (_Commands.ContainsKey(name))
        {
            return false;
        }

        _Commands.Add(name, new(action, hints));
        return true;
    }
}
