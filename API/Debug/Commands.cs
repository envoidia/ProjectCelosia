using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using API.Battle;
using API.Battle.State;
using API.Graphics;
using API.Lang;
using API.Modding;
using API.Save;
using API.Util;

namespace API.Debug;

/// <summary>
/// Console commands.
/// args[0] is always the current command
/// </summary>
public static class Commands
{
    #region Text

    public const string Text = "text";
    public static readonly CommandParam TextParam = new(Text);
    public static readonly CommandParam[] TextParamArr = [TextParam];

    public const string All = "all";

    public const string Show = "show";
    public const string Hide = "hide";

    public const string Give = "give";
    public const string Remove = "remove";

    private const string _BasicInfo = "Basic info";

    private const string _Help = "Enter a command followed by its arguments with a space between each.\nWrap an argument in \" to include spaces. Inside of quote pairs, \\\" will be parsed as a literal \"\n| pipes the output of a command into the next\nUse `man` with `cmd` to list commands, `kb` to list keybinds, and a command name for info about it";

    private static readonly string[] _Overlays = ["info", "console", "outline", "theme", "input", "perf"];
    private static readonly string[] _Writable = ["modlist", "registry", "lang", "stage", "battlelog", "theme"];
    private static readonly string[] _Cleanable = ["stage", "layout"];
    private static readonly string[] _Reloadable = ["lang", "settings", "themes"];

    #endregion

    internal static void _Init()
    {
        #region Basic

        Command.Register("help", static args =>
        {
            return new(_Help);
        }, [], _BasicInfo, Core.Id);

        Command.Register("clear", static _ =>
        {
            DebugConsole._OutHist.Clear();
            DebugConsole._OutHistLabel.Text = "\n";
            DebugConsole._Line.Width = DebugConsole._MinBgWidth;

            return new(null);
        }, [], "Clears the console", Core.Id);

        Command.Register("history", static _ =>
        {
            return new(string.Join('\n', DebugConsole._InHist));
        }, [], "Returns command history", Core.Id);

        Command.Register("echo", args =>
            new(string.Join(' ', args.ToArray(), 1, args.Length - 1)),
            TextParamArr, "Returns its input", Core.Id);

        const string GrepDesc = "Searches through text. `grep` and `rg` are interchangable";
        const string Search = "Search";

        Command.Register("grep", _Cmd_grep, [TextParam,
            new(Search)], GrepDesc, Core.Id);

        Command.Register("rg", _Cmd_grep, [TextParam,
            new(Search)], GrepDesc, Core.Id, false);

        Command.Register("wc", _Cmd_wc, [TextParam, new("l/w/c")],
            "Counts lines, words, and chars", Core.Id);

        Command.Register("whoami", _Cmd_whoami, [],
            "Returns the name of the loaded save", Core.Id);

        Command.Register("kill", _ =>
        {
            Core.Instance.Exit();
            return new(null);
        }, [], "Closes the game", Core.Id);

        Command.Register("export", args =>
        {
            Console.WriteLine($"[{args[0]}] {string.Join(' ', args.ToArray(),
                1, args.Length - 1).Replace('［', '[')}");

            return new(null);
        }, TextParamArr, "Writes text to stdout", Core.Id);

        Command.Register("gc", static args =>
        {
            return new($"Forced GC collect, memory usage went from {GC.GetTotalMemory(false)
                / DebugUtil._Mb} to {GC.GetTotalMemory(true) / DebugUtil._Mb}");
        }, [], "Forces GC collection and reports memory", Core.Id);

        Command.Register("cbc", _Cmd_cbc, TextParamArr, "Writes to clipboard", Core.Id);
        Command.Register("cbp", _Cmd_cbp, [], "Reads from clipboard", Core.Id);

        #endregion

        #region Domain-specific

        Command.Register("overlay", _Cmd_overlay, [new(_Overlays),
            new("show/hide/blank to toggle", ["show", "hide"])],
            "Enable/disable/toggle overlays", Core.Id);

        Command.Register("write", _Cmd_write, [new(_Writable),
            new("preserve formatting?", CommandParam.InputBools)],
            "Write various things", Core.Id,
            extendedDesc: "args[1] determines whether to preserve text formatting codes. Leave blank to preserve colors but not images");

        Command.Register("cleanup", _Cmd_cleanup, [new(_Cleanable)],
            "Sort and cleanup various things", Core.Id);

        Command.Register("reload", _Cmd_reload, [new(_Reloadable)],
            "Reload various assets", Core.Id);

        Command.Register("cycletheme", _Cmd_cycletheme, [],
            "Cycles the current theme", Core.Id);

        Command.Register("setting", _Cmd_setting,
            [new("setting key/reload/reset",
            [.. Settings.AllSettings.Keys, "reload", "reset"]),
            new("value")], "Alter settings", Core.Id,
            extendedDesc: "Omit value to print the current value\n`reload` to reload from file, `reset` to reset to default\nIf a value is invalid, the default will be used instead");

        #endregion

        #region Battle

        Command.Register("buff", _Cmd_buff, [new("unit index 0-7",
            CommandParam.InputNumbers0To7), new(["give", "remove"]),
            new("buff ID", [], () => Registry.IdsOf<Buff>()),
            new("turns", CommandParam.InputNumbers1To9),
            new("stacks", CommandParam.InputNumbers1To9)],
            "Give/remove buffs in battle", Core.Id,
            extendedDesc: "Can omit turns and stacks if removing");

        // todo passive

        // todo equip

        // todo stat statmult/affinity/stage/stageturns/mult/boomantat/statmod

        #endregion

        // Must be last
        string[] manArgs = ["cmd", "kb"];
        const string Man = "man";
        Command.Register(Man, _Cmd_man, [new("cmd/kb/command name",
            ["cmd", "kb", Man, .. Command.Cmds.Keys])], _BasicInfo, Core.Id);
    }

    #region Basic

    private static CommandResult _Cmd_man(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            return new(ExitCode.Err, Command.Cmds["man"].GetUsageText());
        }

        switch (args[1])
        {
            case "cmd":
                const int Cap = 1500;
                StringBuilder sb = new("Command list:", Cap);
                foreach (KeyValuePair<string, Command> kvp in
                    Command.Cmds.Where(kvp => kvp.Value.IsVisible))
                {
                    sb.Append($"\n{ThemeColor.Imp.Str}[{kvp.Key}]{ThemeColor.White.Str} {kvp.Value.Desc}");
                }

                Assert.CapIs(sb, Cap); // todo remove before final release
                return new(sb.ToString());

            case "kb":
                return new($"""
                    {ThemeColor.Imp.Str}[Left/Right]{ThemeColor.White.Str} Move cursor
                    {ThemeColor.Imp.Str}[BkSp/Del]{ThemeColor.White.Str} Delete to left/right
                    {ThemeColor.Imp.Str}[Ctrl]{ThemeColor.White.Str} Move and delete by word (space-separated)
                    {ThemeColor.Imp.Str}[Alt]{ThemeColor.White.Str} Move and delete by word part (case- or space-separated)
                    {ThemeColor.Imp.Str}[Home/End]{ThemeColor.White.Str} Cursor to start/end

                    {ThemeColor.Imp.Str}[Ctrl+Shift+BkSp/Del]{ThemeColor.White.Str} Delete all to left/right
                    {ThemeColor.Imp.Str}[Ctrl+Shift+K]{ThemeColor.White.Str} Delete all

                    {ThemeColor.Imp.Str}[Up/Down]{ThemeColor.White.Str} Move through command history
                    {ThemeColor.Imp.Str}[Ctrl+Up/Down]{ThemeColor.White.Str} Move through output history
                    {ThemeColor.Imp.Str}[Ctrl+Home/End]{ThemeColor.White.Str} To top/bottom of output history
                    
                    {ThemeColor.Imp.Str}[Tab]{ThemeColor.White.Str} Accept autocomplete
                    {ThemeColor.Imp.Str}[Ctrl+C/V/X]{ThemeColor.White.Str} Copy/paste/cut
                    {ThemeColor.Imp.Str}[Enter]{ThemeColor.White.Str} Execute command
                    {ThemeColor.Imp.Str}[Esc]{ThemeColor.White.Str} Toggle focus

                    Hold {ThemeColor.Imp.Str}[Shift]{ThemeColor.White.Str} to move faster in all cases

                    Non-control keys are used to type
                    """);
        }

        if (!Command.Cmds.TryGetValue(args[1], out Command cmd))
        {
            return new(ExitCode.Err, $"Command {args[1]} couldn't be found");
        }

        Assert.NotNull(cmd);

        string? extDesc = cmd.ExtendedDesc is null ? null : $"\n{cmd.ExtendedDesc}";

        return new($"{cmd.ModId}:{args[1]}: {cmd.Desc}\n{cmd.GetUsageText()}{extDesc}");
    }

    private static CommandResult _Cmd_grep(ReadOnlySpan<string> args)
    {
        if (args.Length < 3)
        {
            return new(ExitCode.Err, Command.Cmds["grep"].GetUsageText());
        }

        string[] lines = args[1].Split('\n');
        StringBuilder matches = new(256);
        foreach (string line in lines)
        {
            if (line.Contains(args[2]))
            {
                if (matches.Length != 0)
                {
                    matches.Append("\n/c[white]");
                }

                matches.Append(line);
            }
        }

        string s = matches.ToString();

        return new(s);
    }

    private const string _WcRes = "{0} lines, {1} words, {2} chars";
    private static CommandResult _Cmd_wc(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            return new(ExitCode.Err, Command.Cmds["wc"].GetUsageText());
        }

        bool usingFormat = args[^1] is "l" or "w" or "c";

        if (string.IsNullOrEmpty(args[1]))
        {
            if (args.Length >= 3 && usingFormat)
            {
                return new("0");
            }

            return new(string.Format(_WcRes, 0, 0, 0));
        }

        string str = string.Join(' ', args.ToArray(), 1, args.Length - (usingFormat ? 2 : 1));

        int chars = str.Length;

        if (args.Length >= 3 && args[^1] == "c")
        {
            return new(chars.ToString());
        }

        int lines = 1;
        int words = 1;

        foreach (char c in str)
        {
            if (c == '\n')
            {
                lines++;
                continue;
            }

            if (c == ' ')
            {
                words++;
                continue;
            }
        }

        if (args.Length >= 3 && args[^1] == "l")
        {
            return new(lines.ToString());
        }

        if (args.Length >= 3 && args[^1] == "w")
        {
            return new(words.ToString());
        }

        return new(string.Format(_WcRes, lines, words, chars));
    }

    private static CommandResult _Cmd_whoami(ReadOnlySpan<string> args)
    {
        return new(ExitCode.Err, "NYI");
    }

    private static CommandResult _Cmd_cbc(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            return new(ExitCode.Err, "Must pass the text to copy");
        }

        Clipboard.Text = string.Join(' ', args.ToArray(), 1, args.Length - 1)
            .Replace('［', '[');

        return new(null);
    }

    private static CommandResult _Cmd_cbp(ReadOnlySpan<string> args)
    {
        return new(Clipboard.Text);
    }

    #endregion

    #region Domain-specific

    private static CommandResult _Cmd_overlay(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            return new(ExitCode.Err, $"Must pass the overlay to target ({string.Join(
                '/', _Overlays)})");
        }

        _OverlayChange ch = _OverlayChange.Toggle;

        if (args.Length > 2)
        {
            if (args[2] == Show)
            {
                ch = _OverlayChange.Show;
            }
            else if (args[2] == Hide)
            {
                ch = _OverlayChange.Hide;
            }
        }

        switch (args[1])
        {
            case "info":
                switch (ch)
                {
                    case _OverlayChange.Show:
                        DebugUtil._SetShowDebugInfo(true);
                        return new(null);

                    case _OverlayChange.Hide:
                        DebugUtil._SetShowDebugInfo(false);
                        return new(null);

                    case _OverlayChange.Toggle:
                        DebugUtil._ToggleShowDebugInfo();
                        return new(null);

                    default:
                        throw new ClosedEnumsWhenException();
                }

            case "console":
                switch (ch)
                {
                    case _OverlayChange.Show:
                        DebugConsole._Show = true;
                        return new(null);

                    case _OverlayChange.Hide:
                        DebugConsole._Show = false;
                        return new(null);

                    case _OverlayChange.Toggle:
                        DebugConsole._Show ^= true;
                        return new(null);

                    default:
                        throw new ClosedEnumsWhenException();
                }

            case "outline":
                switch (ch)
                {
                    case _OverlayChange.Show:
                        DebugUtil.DrawActorOutlines = true;
                        return new(null);

                    case _OverlayChange.Hide:
                        DebugUtil.DrawActorOutlines = false;
                        return new(null);

                    case _OverlayChange.Toggle:
                        DebugUtil.DrawActorOutlines ^= true;
                        return new(null);

                    default:
                        throw new ClosedEnumsWhenException();
                }

            case "theme":
                switch (ch)
                {
                    case _OverlayChange.Show:
                        DebugUtil.DrawTheme = true;
                        return new(null);

                    case _OverlayChange.Hide:
                        DebugUtil.DrawTheme = false;
                        return new(null);

                    case _OverlayChange.Toggle:
                        DebugUtil.DrawTheme ^= true;
                        return new(null);

                    default:
                        throw new ClosedEnumsWhenException();
                }

            case "input":
                switch (ch)
                {
                    case _OverlayChange.Show:
                        DebugUtil._SetShowInputView(true);
                        return new(null);

                    case _OverlayChange.Hide:
                        DebugUtil._SetShowInputView(false);
                        return new(null);

                    case _OverlayChange.Toggle:
                        DebugUtil._ToggleShowInputView();
                        return new(null);

                    default:
                        throw new ClosedEnumsWhenException();
                }

            case "perf":
                switch (ch)
                {
                    case _OverlayChange.Show:
                        DebugUtil._PerfGraph.IsVisible = true;
                        return new(null);

                    case _OverlayChange.Hide:
                        DebugUtil._PerfGraph.IsVisible = false;
                        return new(null);

                    case _OverlayChange.Toggle:
                        DebugUtil._PerfGraph.IsVisible ^= true;
                        return new(null);

                    default:
                        throw new ClosedEnumsWhenException();
                }

            default:
                return new(ExitCode.Err, $"Valid overlays: {_Overlays}");
        }
    }

    private enum _OverlayChange
    {
        Show,
        Hide,
        Toggle
    }

    private static CommandResult _Cmd_write(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            return new(ExitCode.Err, $"Must pass the item to write ({_Writable}\n{Command.Cmds["write"].ExtendedDesc}");
        }

        _Preserve fmt = _Preserve.NonImages;

        if (args.Length >= 3)
        {
            if (!bool.TryParse(args[2], out bool b))
            {
                return new(ExitCode.Err, $"args[1] ({args[1]}) must be a bool (true or false)");
            }

            fmt = b ? _Preserve.True : _Preserve.False;
        }

        switch (args[1])
        {
            case "modlist":
                StringBuilder sb = new("Modlist: ", 256);
                int iMax = ModLoader._LoadedMods.Count - 1;
                for (int i = 0; i <= iMax; i++)
                {
                    sb.Append(ModLoader._LoadedMods[i].GetName());

                    if (i != iMax)
                    {
                        sb.Append('\n');
                    }
                }

                return new(sb.ToString());

            case "registry":
                return new(format(Registry.ToString(), fmt));

            case "lang":
                return new(format(Settings.Language.ToString(), fmt));

            case "stage":
                return new(format(Stage.ToString(), fmt));

            case "battlelog":
                return new(format($"Battle Log: {string.Join('\n', LogLib._LogText)}", fmt));

            case "theme":
                return new(Settings.Theme.ToDetailedString(fmt != _Preserve.False));

            default:
                return new(ExitCode.Err, $"Valid items: {_Writable}");
        }

        static string format(string str, _Preserve fmt)
        {
            return fmt switch
            {
                _Preserve.NonImages => str.RemoveImageCodes(),
                _Preserve.True => str,
                _Preserve.False => str.RemoveFormattingCodes(),
                _ => throw new ClosedEnumsWhenException()
            };
        }
    }

    private enum _Preserve
    {
        NonImages,
        True,
        False
    }

    private static CommandResult _Cmd_cleanup(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            return new(ExitCode.Err, $"Must pass the item to cleanup ({_Cleanable})");
        }

        switch (args[1])
        {
            case "stage":
                Stage.Sort();
                return new("Sorted Stage");

            case "layout":
                Stage._RecalcLayoutWidgets();
                return new("Recalculated ILayoutWidgets");

            default:
                return new(ExitCode.Err, $"Valid items: {_Cleanable}");
        }
    }

    private const string _ReloadedSettings = "Reloaded settings";
    private static CommandResult _Cmd_reload(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            return new(ExitCode.Err, $"Must pass the item to reload ({_Reloadable})");
        }

        switch (args[1])
        {
            case "lang":
                Language.Reload();
                return new("Reloaded lang files");

            case "settings":
                Settings.Reload();
                return new(_ReloadedSettings);

            case "themes":
                // todo
                return new("Reloaded themes (NYI)");

            default:
                return new(ExitCode.Err, $"Valid items: {_Reloadable}");
        }
    }

    private static CommandResult _Cmd_cycletheme(ReadOnlySpan<string> args)
    {
        Theme[] themes = Registry.Of<Theme>();
        int i = themes.IndexOf(Settings.Theme);
        Settings.Theme = themes[i == themes.Length - 1 ? 0 : i + 1];

        return new($"Theme changed to {Settings.Theme.GetName().RemoveFormattingCodes()}");
    }

    private static CommandResult _Cmd_setting(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            Command cmd = Command.Cmds["setting"];
            return new(ExitCode.Err, $"{cmd.GetUsageText()}\n{cmd.ExtendedDesc}");
        }

        if (args[1] == "reload")
        {
            Settings.Reload();
            return new(_ReloadedSettings);
        }

        if (args[1] == "reset")
        {
            Settings.Reset();
            Settings.Write();
            return new("Reset settings");
        }

        if (!File.Exists(Settings.FilePath))
        {
            Settings.Reset();
            Settings.Write();
        }

        Dictionary<string, string> settings = Properties.Parse(Settings.FilePath);

        if (args.Length == 2)
        {
            if (!settings.TryGetValue(args[1], out string val))
            {
                return new(ExitCode.Err, $"Setting `{args[1]}` couldn't be found");
            }

            Assert.NotNull(val);

            return new($"{args[1]}={val}");
        }

        if (!Settings.AllSettings.ContainsKey(args[1]))
        {
            return new(ExitCode.Err, $"Setting `{args[1]}` couldn't be found");
        }

        Settings.AllSettings[args[1]] = args[2];
        Settings.Write();
        Settings.Reload();

        return new($"Changed setting `{args[1]}` to `{args[2]}`");
    }

    #endregion

    #region Battle

    private static CommandResult _Cmd_buff(ReadOnlySpan<string> args)
    {
        const string _UnitIndexError = "unitindex (args[1]) must be an int 0-{0}";
        const string _TurnsError = "turns (args[4]) must be an int > 0";
        const string _StacksError = "stacks (args[5]) must be an int > 0";

        if (args.Length < 4)
        {
            return new(ExitCode.Err, getHelpText());
        }

        // todo fail if not in battle

        // Find Unit
        int unitIndex;
        try
        {
            unitIndex = int.Parse(args[1]);
        }
        catch
        {
            return new(ExitCode.Err, string.Format(_UnitIndexError, PosLib.Highest));
        }

        if (unitIndex is < 0 or > PosLib.Highest)
        {
            return new(ExitCode.Err, string.Format(_UnitIndexError, PosLib.Highest));
        }

        Unit unit = BattleLib.Battle.GetUnitAtPos(unitIndex);

        // Find Buff
        IRegistrable? registrable = Registry.Get(args[3]);
        if (registrable is not Buff)
        {
            return new(ExitCode.Err, $"buff ID (args[3]) `{args[3]}` is not a valid buff");
        }

        Buff buff = (Buff) registrable;

        // Determine give/remove
        switch (args[2])
        {
            case Give:
                if (args.Length < 6)
                {
                    return new(ExitCode.Err, getHelpText());
                }

                if (!int.TryParse(args[4], out int turns))
                {
                    return new(ExitCode.Err, _TurnsError);
                }

                if (turns is < 1)
                {
                    return new(ExitCode.Err, _TurnsError);
                }

                // Find stacks
                if (!int.TryParse(args[5], out int stacks))
                {
                    return new(ExitCode.Err, _StacksError);
                }

                if (stacks is < 1)
                {
                    return new(ExitCode.Err, _StacksError);
                }

                unit.GiveBuffInstances(new BuffInstance(buff, turns, Math.Min(stacks, buff.MaxStacks)));

                return new(
                    $"Gave {ThemeColor.Imp.Str}{stacks}x {buff.GetNameWithoutIcon()}{ThemeColor.White.Str} to {unit
                        .FormatName(false)} for {ThemeColor.Imp.Str}{turns}{ThemeColor.White.Str} turns");

            case Remove:
                unit.RemoveBuffs(buff);

                // todo differentiate between removal and lack thereof? allow removal of x turns/stacks?
                return new(
                    $"Removed {buff.GetNameWithoutIcon()}{ThemeColor.White.Str} from {unit.FormatName(false)}");

            default:
                return new(ExitCode.Err, "args[2] must be `give` or `remove`");
        }

        static string getHelpText()
        {
            Command cmd = Command.Cmds["buff"];
            return $"{cmd.GetUsageText()}\n{cmd.ExtendedDesc}";
        }
    }

    #endregion
}
