using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using API.Battle;
using API.Battle.State;
using API.Extensions;
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
    public const string Text = "text";
    public static readonly string[] TextArr = [Text];

    public const string All = "all";

    public const string Show = "show";
    public const string Hide = "hide";

    public const string Give = "give";
    public const string Remove = "remove";

    private const string _BasicInfo = "Basic info";

    private const string _ManDesc = "Enter a command followed by its arguments with a space between each.\n| pipes the output of a command into the next\n`man cmd` to list all commands, `man kb` to list all keybinds";

    private const string _Overlays = "info/console/outline/theme/input/perf";
    private const string _Writable = "modlist/registry/lang/stage/battlelog/theme";
    private const string _Cleanable = "stage/layout";
    private const string _Reloadable = "lang/settings/themes";

    internal static void _Init()
    {
        #region Basic

        Command.Register("help", static args =>
        {
            return new($"""
            This console can be used for entering commands. It is for developers, and not part of gameplay. Misuse can brick your save
            {_ManDesc}
            """);
        }, [], _BasicInfo, Core.Id);

        Command.Register("man", _Cmd_man, ["cmd/kb"], _BasicInfo, Core.Id);

        Command.Register("clear", static _ =>
        {
            DebugConsole._LogText.Clear();
            DebugConsole._Log.Text = "\n";
            DebugConsole._Line.Width = DebugConsole._MinBgWidth;

            return new(null);
        }, [], "Clears the console", Core.Id);

        Command.Register("history", static _ =>
        {
            return new(string.Join('\n', DebugConsole._Hist));
        }, [], "Returns command history", Core.Id);

        Command.Register("echo", args =>
            new(string.Join(' ', args.ToArray(), 1, args.Length - 1)),
            TextArr, "Returns its input", Core.Id);

        Command.Register("grep", _Cmd_grep, [Text, "search"],
            "Searches through text", Core.Id);

        Command.Register("wc", _Cmd_wc, [Text, "l/w/c"],
            "Counts lines, words, and chars", Core.Id);

        Command.Register("which", _Cmd_which, ["command"],
            "Returns the ID and description of a command", Core.Id);

        Command.Register("whoami", _Cmd_whoami, [], "Loaded save name", Core.Id);

        Command.Register("kill", _ =>
        {
            Core.Instance.Exit();
            return new(null);
        }, [], "Closes the game", Core.Id);

        Command.Register("export", args =>
        {
            Console.WriteLine($"[{args[0]}] {string.Join(' ', args.ToArray(),
                1, args.Length - 1)}");

            return new(null);
        }, TextArr, "Writes text to stdout", Core.Id);

        Command.Register("gc", static args =>
        {
            return new($"Forced GC collect, memory usage went from {GC.GetTotalMemory(false)
                / DebugUtil._Mb} to {GC.GetTotalMemory(true) / DebugUtil._Mb}");
        }, [], "Forces GC collection and reports memory", Core.Id);

        Command.Register("copy", _Cmd_copy, TextArr, "Writes to clipboard", Core.Id);
        Command.Register("paste", _Cmd_paste, [], "Reads from clipboard", Core.Id);

        #endregion

        #region Domain-specific

        Command.Register("overlay", _Cmd_overlay, [_Overlays,
            "show/hide/blank to toggle"], "Enable/disable/toggle overlays", Core.Id);

        Command.Register("write", _Cmd_write, [_Writable, "preserve formatting?"],
            "Write various things", Core.Id);

        Command.Register("cleanup", _Cmd_cleanup, [_Cleanable],
            "Sort and cleanup various things", Core.Id);

        Command.Register("reload", _Cmd_reload, [_Reloadable],
            "Reload various assets", Core.Id);

        Command.Register("cycletheme", _Cmd_cycletheme, [], "Cycles the current theme", Core.Id);

        Command.Register("setting", _Cmd_setting, ["setting key/reload/reset", "value"],
            "Alter settings", Core.Id);

        #endregion

        #region Battle

        Command.Register("buff", _Cmd_buff, ["unit index 0-7", "give/remove", "buff ID", "turns", "stacks"],
            "Give/remove buffs in battle", Core.Id);

        // todo passive

        // todo equip

        // todo stat statmult/affinity/stage/stageturns/mult/boomantat/statmod

        #endregion
    }

    #region Basic

    private static CommandResult _Cmd_man(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            return new(ExitCode.Err, _ManDesc);
        }

        switch (args[1])
        {
            case "cmd":
                StringBuilder sb = new("Command list:");
                foreach (KeyValuePair<string, Command> kvp in Command.Cmds)
                {
                    sb.Append($"\n{ThemeColor.Imp.Str}{kvp.Key}:{ThemeColor.White.Str} {kvp.Value.Desc}");
                }

                return new(sb.ToString());

            case "kb":
                // todo resizable glyphs
                return new($"""
                    {ThemeColor.Imp.Str}[Left/Right]{ThemeColor.White.Str} Move cursor. Hold {ThemeColor.Imp.Str}[Shift]{ThemeColor.White.Str} to move faster
                    {ThemeColor.Imp.Str}[BkSp/Del]{ThemeColor.White.Str} Delete to left/right
                    {ThemeColor.Imp.Str}[Ctrl]{ThemeColor.White.Str} Move and delete by word
                    {ThemeColor.Imp.Str}[Home/End]{ThemeColor.White.Str} Cursor to start/end
                    {ThemeColor.Imp.Str}[Up/Down]{ThemeColor.White.Str} Move through command history
                    {ThemeColor.Imp.Str}[Tab]{ThemeColor.White.Str} Accept autocomplete
                    {ThemeColor.Imp.Str}[Ctrl+C/V/X]{ThemeColor.White.Str} Copy/paste/cut
                    {ThemeColor.Imp.Str}[Enter]{ThemeColor.White.Str} Execute command
                    {ThemeColor.Imp.Str}[Esc]{ThemeColor.White.Str} Toggle focus
                    Non-control keys are used to type
                    """);

            default:
                return new(ExitCode.Err, _ManDesc);
        }
    }

    private static CommandResult _Cmd_grep(ReadOnlySpan<string> args)
    {
        if (args.Length < 3)
        {
            return new(ExitCode.Err, "Usage: `grep [text] [search]`");
        }

        string[] lines = args[1].Split('\n');
        StringBuilder matches = new();
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
            return new(ExitCode.Err, "Usage: `wc [text] [l/w/c]`");
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

    private static CommandResult _Cmd_which(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            return new(ExitCode.Err, "Must pass the target command");
        }

#pragma warning disable CS8600
        if (!Command.Cmds.TryGetValue(args[1], out Command cmd))
        {
            return new(ExitCode.Err, $"Command {args[1]} couldn't be found");
        }
#pragma warning restore CS8600

        Assert.NotNull(cmd);

        return new($"{cmd.ModId}:{args[1]}: {cmd.Desc}");
    }

    private static CommandResult _Cmd_whoami(ReadOnlySpan<string> args)
    {
        return new(ExitCode.Err, "NYI");
    }

    private static CommandResult _Cmd_copy(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            return new(ExitCode.Err, "Must pass the text to copy");
        }

        Util.Clipboard.Text = string.Join(' ', args.ToArray(), 1, args.Length - 1);

        return new(null);
    }

    private static CommandResult _Cmd_paste(ReadOnlySpan<string> args)
    {
        return new(Util.Clipboard.Text);
    }

    #endregion

    #region Domain-specific

    private static CommandResult _Cmd_overlay(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            return new(ExitCode.Err, $"Must pass the overlay to target ({_Overlays})");
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
            return new(ExitCode.Err, $"Must pass the item to write ({_Writable}\nargs[1] determines whether to preserve text formatting codes. Leave blank to preserve colors but not images");
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
                StringBuilder sb = new("Modlist: ");
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
                return new(Settings.Theme.ToDetailedString());

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
        ReadOnlySpan<Theme> themes = [.. Registry.Of<Theme>()];
        int i = themes.IndexOf(Settings.Theme);
        Settings.Theme = themes[i == themes.Length - 1 ? 0 : i + 1];

        return new($"Theme changed to {Settings.Theme.GetName().RemoveFormattingCodes()}");
    }

    private static CommandResult _Cmd_setting(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            return new(ExitCode.Err, "Usage: `setting [setting] [value]`. Omit value to print the current value\n`setting reload` to reload from file, `setting reset` to reset to default\nIf a value is invalid, the default will be used instead");
        }

        if (args[1] == "reload")
        {
            Settings.Reload();
            return new(_ReloadedSettings);
        }

        if (args[1] == "reset")
        {
            Settings.Create();
            Settings.Reload();
            return new("Reset settings");
        }

        if (!File.Exists(Settings.FilePath))
        {
            Settings.Create();
        }

        Dictionary<string, string> settings = Properties.Parse(Settings.FilePath);

        if (args.Length == 2)
        {
#pragma warning disable CS8600
            if (!settings.TryGetValue(args[1], out string val))
            {
                return new(ExitCode.Err, $"Setting `{args[1]}` couldn't be found");
            }
#pragma warning restore CS8600

            Assert.NotNull(val);

            return new($"{args[1]}={val}");
        }

        // todo dont default all others when setting one
        switch (args[1])
        {
            case "Language":
                Settings.Create(language: args[2]);
                break;

            case "BattleSpeed":
                Settings.Create(battleSpeed: float.ParseOrDefault(args[2], 1f));
                break;

            case "ShowInvalidMoveWarning":
                Settings.Create(showInvalidMoveWarning: bool.ParseOrDefault(args[2], true));
                break;

            case "Resolution":
                Settings.Create(resolution: int.ParseOrDefault(args[2], -1));
                break;

            case "Fulmancreen":
                Settings.Create(fullscreen: bool.ParseOrDefault(args[2], true));
                break;

            case "EnableVsync":
                Settings.Create(enableVsync: bool.ParseOrDefault(args[2], true));
                break;

            case "TargetFps":
                Settings.Create(targetFps: int.ParseOrDefault(args[2], -1));
                break;

            case "Theme":
                Settings.Create(theme: args[2]);
                break;

            case "MusicVolume":
                Settings.Create(musicVolume: float.ParseOrDefault(args[2], 0.75f));
                break;

            case "SfxVolume":
                Settings.Create(sfxVolume: float.ParseOrDefault(args[2], 0.75f));
                break;

            case "ShowInputGuide":
                Settings.Create(showInputGuide: bool.ParseOrDefault(args[2], true));
                break;

            case "DetectNintendoController":
                Settings.Create(detectNintendoController: bool.ParseOrDefault(args[2], true));
                break;

            case "EnableDebugFeatures":
                Settings.Create(enableDebugFeatures: bool.ParseOrDefault(args[2], false));
                break;

            case "SelectOpponentMoves":
                Settings.Create(selectOpponentMoves: bool.ParseOrDefault(args[2], false));
                break;

            default:
                return new(ExitCode.Err, $"Setting `{args[1]}` couldn't be found");
        }

        Settings.Reload();
        return new($"Changed setting `{args[1]}` to `{args[2]}`");
    }

    #endregion

    #region Battle

    private const string _UnitIndexError = "unitindex (args[1]) must be an int 0-{0}";
    private const string _TurnsError = "turns (args[4]) must be an int > 0";
    private const string _StacksError = "stacks (args[5]) must be an int > 0";

    private static CommandResult _Cmd_buff(ReadOnlySpan<string> args)
    {
        if (args.Length < 4)
        {
            return new(ExitCode.Err,
                "Usage: `buff [unit index 0-7] [give/remove] [buff ID] [turns] [stacks]`. Can omit turns and stacks if removing");
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
                    $"Gave {buff.GetNameWithoutIcon()}{ThemeColor.White.Str} to {unit.FormatName(false)}");

            case Remove:
                unit.RemoveBuffs(buff);

                // todo differentiate between removal and lack thereof? allow removal of x turns/stacks?
                return new(
                    $"Removed {buff.GetNameWithoutIcon()}{ThemeColor.White.Str} from {unit.FormatName(false)}");

            default:
                return new(ExitCode.Err, "args[2] must be `give` or `remove`");
        }
    }

    #endregion
}
