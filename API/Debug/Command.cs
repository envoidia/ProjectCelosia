using System;
using System.Collections.Generic;
using API.Menu.Widget;
using API.Util;

namespace API.Debug;

/// <summary>
/// A command to be executed by the debug console
/// </summary>
public sealed class Command
{
    internal static readonly Dictionary<string, Command> _Commands = [];
    internal static readonly Dictionary<string, string> _Aliases = [];

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
    public static Result<CommandRegistrationError> Create(string name, Action<string[]> action, string[] hints)
    {
        if (_Commands.ContainsKey(name))
        {
            return new(CommandRegistrationError.AlreadyExists);
        }

        if (_Aliases.ContainsKey(name))
        {
            return new(CommandRegistrationError.NameUsedByAlias);
        }

        _Commands.Add(name, new(action, hints));

        return new();
    }

    public static Result<AliasRegistrationError> AddAlias(string alias, string cmd)
    {
        if (_Aliases.ContainsKey(alias))
        {
            return new(AliasRegistrationError.AlreadyExists);
        }

        if (_Commands.ContainsKey(alias))
        {
            return new(AliasRegistrationError.NameUsedByCommand);
        }

        if (!_Commands.ContainsKey(cmd))
        {
            return new(AliasRegistrationError.CommandDoesntExist);
        }

        _Aliases.Add(alias, cmd);

        return new();
    }
}

public enum CommandRegistrationError
{
    AlreadyExists,
    NameUsedByAlias,
}

public enum AliasRegistrationError
{
    AlreadyExists,
    NameUsedByCommand,
    CommandDoesntExist
}