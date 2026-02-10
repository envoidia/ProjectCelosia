using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Debug;

/// <summary>
/// A command to be executed by the debug console
/// </summary>
public sealed class Command
{
    public static readonly Dictionary<string, Command> Cmds = [];

    /// <summary>
    /// Function this should execute
    /// </summary>
    public readonly Func<ReadOnlySpan<string>, CommandResult> Fn;

    /// <summary>
    /// Hints for this' arguments.
    /// Eg hints of <c>[foo, bar, lorem]</c> would display <c>commandname [foo] [bar] [lorem]</c> as you type
    /// </summary>
    public readonly string[] Hints;

    /// <summary>
    /// Description of this
    /// </summary>
    public readonly string Desc;

    /// <summary>
    /// ID of the mod this is from
    /// </summary>
    public readonly string ModId;

    /// <summary>
    /// Whether this command should be listed in `man cmd`
    /// </summary>
    public readonly bool IsVisible;

    private Command(Func<ReadOnlySpan<string>, CommandResult> fn,
        string[] hints, string desc, string modId, bool isVisible)
    {
        this.Fn = fn;
        this.Hints = hints;
        this.Desc = desc;
        this.ModId = modId;
        this.IsVisible = isVisible;
    }

    /// <summary>
    /// Creates a <c>Command</c> with the specified name and adds it to the command registry,
    /// unless the name is already used or invalid
    /// </summary>
    /// <returns>The error, if any</returns>
    public static CommandRegistrationError? Register(string name, Func<ReadOnlySpan<string>,
        CommandResult> fn, string[] hints, string desc, string modId, bool isVisible = true)
    {
        if (name.Any(c => char.IsWhiteSpace(c) || c == '|'))
        {
            return CommandRegistrationError.InvalidName;
        }

        if (Cmds.ContainsKey(name))
        {
            return CommandRegistrationError.AlreadyUsed;
        }

        Cmds.Add(name, new(fn, hints, desc, modId, isVisible));

        return null;
    }
}