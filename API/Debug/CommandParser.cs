using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                DebugConsole.Log(res.Msg, source, LogLevel.Error);
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
                    DebugConsole.Log(res.Msg, source, LogLevel.Error);
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
        if (!Command.Cmds.TryGetValue(cmdKey, out Command cmd))
        {
            DebugConsole.Log($"{cmdKey} is not a recognized command. Use `help` for help and `ls cmd` to list all commands",
            nameof(DebugConsole), LogLevel.Error);

            // todo suggest close matches

            return null;
        }

        Assert.NotNull(cmd);

        return cmd;
    }

    /// <returns>
    /// The input string split into commands by |, then split into args by unquoted whitespace and filtered
    /// </returns>
    public static Span<string[]> TokenizeCommand(string str)
    {
        ReadOnlySpan<string> cmds = str.Split('|',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        Span<string[]> cmdArgs = new string[cmds.Length][];
        for (int i = 0; i < cmds.Length; i++)
        {
            cmdArgs[i] = _SplitUnquotedWhitespace(cmds[i]);
        }

        return cmdArgs;
    }

    public static string[] _SplitUnquotedWhitespace(string input)
    {
        List<string> result = new(8);
        StringBuilder current = new(64);
        bool inQuotes = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (c == '\\' && i + 1 < input.Length)
            {
                // Preserve the escaped character
                current.Append(input[i + 1]);
                i++;
                continue;
            }

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }

            if (char.IsWhiteSpace(c) && !inQuotes)
            {
                if (current.Length > 0)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
            }
            else
            {
                current.Append(c);
            }
        }

        if (current.Length > 0)
        {
            result.Add(current.ToString());
        }

        return [.. result];
    }

    /// <returns>
    /// The closest autocomplete match for the input args, or null
    /// </returns>
    public static string? GetCurrentAutocompleteMatch(Span<string[]> args)
    {
        if (args.Length > 0 && args[^1].Length > 0 && args[^1].Length > 0)
        {
            string str = args[^1][^1];

            if (!string.IsNullOrWhiteSpace(str))
            {
                return GetAutocompleteMatch(args[^1].Length - 1, args[^1][0], str);
            }
        }

        return null;
    }

    /// <returns>
    /// The closest autocomplete match for the specified param of the currently typed command, or null.
    /// Returning an empty string makes syntax highlighting consider the input valid
    /// </returns>
    public static string? GetAutocompleteMatch(int paramNum, string param0, string str)
    {
        string[] matchAgainst;

        if (paramNum == 0)
        {
            matchAgainst = [.. Command.Cmds.Keys];
        }
        else
        {
            if (!Command.Cmds.TryGetValue(param0, out Command cmd))
            {
                return null;
            }

            if (paramNum >= cmd.Params.Length)
            {
                // Just assume it's valid since there are no rules for it
                return "";
            }

            matchAgainst = cmd.Params[paramNum - 1].GetValidInputs();

            if (matchAgainst.Length == 0)
            {
                return "";
            }
        }

        string? match = matchAgainst
            .Where(k => k.StartsWith(str, StringComparison.Ordinal))
            .OrderByDescending(k => getCommonPrefixLength(k, str))
            .FirstOrDefault();

        return match?[getCommonPrefixLength(match, str)..];

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

    public static string? GetCurrentHintText(Span<string[]> args)
    {
        if (args.Length > 0 && args[^1].Length > 0)
        {
            return GetHintText(args[^1], args.Length > 1);
        }

        return null;
    }

    /// <returns>
    /// Hint text for the current command
    /// </returns>
    public static string? GetHintText(ReadOnlySpan<string> args, bool skipFirst)
    {
        if (args.Length != 0 && Command.Cmds.TryGetValue(args[0], out Command? cmd))
        {
            int skip = args.Length - 1;

            if (skipFirst)
            {
                skip++;
            }

            if (skip < cmd.Params.Length)
            {
                return string.Join(' ', cmd.Params.Skip(skip)
                    .Select(s => $"[{s.Hint}]"));
            }
        }

        return null;
    }
}
