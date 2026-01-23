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
    public readonly Action<ReadOnlySpan<string>> Action;

    /// <summary>
    /// Hints for this' arguments.
    /// Eg hints of <c>[foo, bar, lorem]</c> would display <c>commandname| [foo] [bar] [lorem]</c> as you type
    /// </summary>
    public readonly string[] Hints;

    /// <summary>
    /// Description of this
    /// </summary>
    public readonly string Desc;

    private Command(Action<ReadOnlySpan<string>> action, string[] hints, string desc)
    {
        this.Action = action;
        this.Hints = hints;
        this.Desc = desc;
    }

    /// <summary>
    /// Creates a <c>Command</c> with the specified name and adds it to the command registry, unless the name is already used
    /// </summary>
    /// <returns>Whether the command was created sucessfully</returns>
    public static bool Register(string name, Action<ReadOnlySpan<string>> action, string[] hints, string desc)
    {
        if (_Commands.ContainsKey(name))
        {
            return false;
        }

        _Commands.Add(name, new(action, hints, desc));

        return true;
    }
}