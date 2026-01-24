using System;
using System.Linq;

namespace API.Debug;

public static class CommandParser
{
    /// <summary>
    /// Executes the specified command string from left to right
    /// </summary>
    // todo cleanup
    public static void ExecuteCommand(string str)
    {
        Span<string[]> cmds = TokenizeCommand(str);

        // Execute first command
        Command? cmdObj = GetCommand(cmds[0][0]);

        if (cmdObj is null)
        {
            return;
        }

        Assert.NotNull(cmdObj);

        CommandResult res = cmdObj.Fn(cmds[0]);

        string source = cmds[0][0];

        if (res.ExitCode == ExitCode.Err)
        {
            if (res.Msg is not null)
            {
                DebugConsole.Log(res.Msg, source, DebugConsole.LogLevel.Error);
            }

            return;
        }

        // Only 1 command
        if (cmds.Length == 1)
        {
            if (res.Msg is not null)
            {
                DebugConsole.Log(res.Msg, source);
            }

            return;
        }

        // Execute remaining commands
        for (int i = 1; i < cmds.Length; i++)
        {
            cmdObj = GetCommand(cmds[i][0]);

            if (cmdObj is null)
            {
                return;
            }

            res = cmdObj.Fn([cmds[i][0], res.Msg ?? "", .. cmds[i][1..]]);

            source = cmds[i][0];

            if (res.ExitCode == ExitCode.Err)
            {
                if (res.Msg is not null)
                {
                    DebugConsole.Log(res.Msg, source, DebugConsole.LogLevel.Error);
                }
                return;
            }
        }

        if (res.Msg is not null)
        {
            DebugConsole.Log(res.Msg, source);
        }
    }

    /// <returns>
    /// The command associated with the given key, or null if it couldn't be found
    /// </returns>
    public static Command? GetCommand(string cmdKey)
    {
#pragma warning disable CS8600
        if (!Command.Cmds.TryGetValue(cmdKey, out Command cmd))
        {
            DebugConsole.Log($"{cmd} is not a recognized command. Use `help` for help and `ls cmd` to list all commands",
            nameof(DebugConsole), DebugConsole.LogLevel.Error);
            // todo suggest close matches

            return null;
        }
#pragma warning restore CS8600

        Assert.NotNull(cmd);

        return cmd;
    }

    /// <returns>
    /// The input string split into commands by |, then split into args by whitespace and filtered
    /// </returns>
    public static Span<string[]> TokenizeCommand(string str)
    {
        ReadOnlySpan<string> cmds = str.Split('|',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        Span<string[]> cmdArgs = new string[cmds.Length][];
        for (int i = 0; i < cmds.Length; i++)
        {
            cmdArgs[i] = cmds[i].Split((char[]) null!,
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        return cmdArgs;
    }

    /// <returns>
    /// The input string split by whitespace and filtered
    /// </returns>
    public static ReadOnlySpan<string> TokenizeWithoutPipelines(string str)
    {
        return str.Split((char[]) null!,
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    /// <returns>
    /// The closest autocomplete match for the currently typed command, or null
    /// </returns>
    public static string? GetAutocompleteMatch(string str)
    {
        string? match = Command.Cmds.Keys
                    .Where(k => k.StartsWith(str, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(k => getCommonPrefixLength(k, str))
                    .FirstOrDefault();

        if (match is null)
        {
            return null;
        }

        return match[getCommonPrefixLength(match, str)..];

        static int getCommonPrefixLength(string a, string b)
        {
            int len = Math.Min(a.Length, b.Length);
            for (int i = 0; i < len; i++)
            {
                if (a[i] != b[i]) return i;
            }

            return len;
        }

    }

    /// <returns>
    /// Hint text for the current command
    /// </returns>
    public static string? GetHintText(ReadOnlySpan<string> args, bool skipFirst)
    {
        if (args.Length != 0 && Command.Cmds.TryGetValue(args[0], out Command? cmd))
        {
            int skip = args.Length - 1;
            
            if(skipFirst)
            {
                skip++;
            }

            if (skip < cmd.Hints.Length)
            {
                return string.Join(' ', cmd.Hints.Skip(skip).Select(s => $"[{s}]"));
            }
        }

        return null;
    }
}
