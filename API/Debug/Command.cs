using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Debug;

/// <summary>
/// A command to be executed by the debug console
/// </summary>
public sealed class Command
{
    /// <summary>
    /// All registered commands
    /// </summary>
    public static readonly Dictionary<string, Command> Cmds = [];

    /// <summary>
    /// All saved variables
    /// </summary>
    public static readonly Dictionary<string, string> Env = [];

    /// <summary>
    /// Function this should execute
    /// </summary>
    public readonly Func<ReadOnlySpan<string>, CommandResult> Fn;

    /// <summary>
    /// The parameters for this
    /// </summary>
    public readonly CommandParam[] Params;

    public readonly string Name;
    public readonly string Desc;
    public readonly string? ExtendedDesc = null;

    /// <summary>
    /// ID of the mod this is from
    /// </summary>
    public readonly string ModId;

    /// <summary>
    /// Whether this command should be listed in `man cmd`
    /// </summary>
    public readonly bool IsVisible;

    private Command(Func<ReadOnlySpan<string>, CommandResult> fn, CommandParam[] @params,
        string name, string desc, string modId, bool isVisible, string? extendedDesc)
    {
        this.Fn = fn;
        this.Params = @params;
        this.Name = name;
        this.Desc = desc;
        this.ModId = modId;
        this.IsVisible = isVisible;
        this.ExtendedDesc = extendedDesc;
    }

    /// <summary>
    /// Creates a <c>Command</c> with the specified name and adds it to the command registry,
    /// unless the name is already used or invalid
    /// </summary>
    /// <returns>The error, if any</returns>
    public static CommandRegistrationError? Register(string name, Func<ReadOnlySpan<string>,
        CommandResult> fn, CommandParam[] @params, string desc, string modId,
        bool isVisible = true, string? extendedDesc = null)
    {
        if (!CommandParser.IsNameValid(name))
        {
            return CommandRegistrationError.InvalidName;
        }

        if (Cmds.ContainsKey(name))
        {
            return CommandRegistrationError.AlreadyUsed;
        }

        Cmds.Add(name, new(fn, @params, name, desc, modId, isVisible, extendedDesc));

        return null;
    }

    public string GetHintText(int skip)
    {
        return string.Join(' ', this.Params.Skip(skip)
            .Select(s => $"[{s.Hint}]"));
    }

    public string GetUsageText()
    {
        return $"Usage: {this.Name} {this.GetHintText(0)}";
    }
}