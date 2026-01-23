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
    public const string All = "all";

    public const string Show = "show";
    public const string Hide = "hide";

    public const string Give = "give";
    public const string Remove = "remove";

    private const string _LsDesc = "`ls cmd` to list all commands, `ls kb` to list all keybinds";
    private const string _Overlays = "info/console/outline/theme/input/perf";
    private const string _Writable = "modlist/registry/lang/stage/battlelog/theme";
    private const string _Cleanable = "stage/layout";
    private const string _Reloadable = "lang/settings/themes";

    internal static void _Init()
    {
        // Basic commands
        Command.Register("help", static args => DebugConsole.Log($"""
            This console can be used for entering commands. It is for developers, and not part of gameplay. Misuse can brick your save
            Each command takes a certain number of arguments, separated by spaces
            {_LsDesc}
            """, args[0]), [], "Basic help info");

        Command.Register("clear", static _ =>
        {
            DebugConsole._LogText.Clear();
            DebugConsole._Log.Text = "\n";
            DebugConsole._Line.Width = DebugConsole._MinBgWidth;
        }, [], "Clears the console");

        Command.Register("ls", _Cmd_ls, ["cmd/kb"], _LsDesc);

        Command.Register("echo",
            static args => DebugConsole.Log(string.Join(' ', args.ToArray(),
            1, args.Length - 1), args[0]), ["text"], "Prints text");

        // Basic utilities

        Command.Register("overlay", _Cmd_overlay, [_Overlays,
            "show/hide/blank to toggle"], "Enable/disable/toggle overlays");

        Command.Register("write", _Cmd_write, [_Writable], "Write various things to the console");

        Command.Register("cleanup", _Cmd_cleanup, [_Cleanable], "Sort and cleanup various things");

        Command.Register("reload", _Cmd_reload, [_Reloadable], "Reload various assets");

        Command.Register("gc", static args =>
            DebugConsole.Log($"Forced GC collect, memory usage went from {GC.GetTotalMemory(false)
                / DebugUtil._Mb} to {GC.GetTotalMemory(true) / DebugUtil._Mb}", args[0]),
            [], "Forces GC collection and reports memory");

        Command.Register("setting", _Cmd_setting, ["setting key/reload/reset", "value"], "Alter settings");

        Command.Register("cycletheme", _Cmd_cycletheme, [], "Cycles the current theme");

        // Battle commands

        Command.Register("buff", _Cmd_buff,
            ["unit index 0-7", "give/remove", "buff ID", "turns", "stacks"], "Give/remove buffs in battle");

        // todo passive

        // todo equip

        // todo stat statmult/affinity/stage/stageturns/mult/boolstat/statmod
    }

    #region Basic utilities

    private static void _Cmd_ls(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            DebugConsole.Log(_LsDesc, args[0]);
            return;
        }

        switch (args[1])
        {
            case "cmd":
                StringBuilder sb = new("Command list:");
                foreach (KeyValuePair<string, Command> kvp in Command._Commands)
                {
                    sb.Append($"\n{ThemeColor.Imp.Str}{kvp.Key}:{ThemeColor.White.Str} {kvp.Value.Desc}");
                }

                DebugConsole.Log(sb.ToString(), args[0]);

                return;

            case "kb":
                // todo resizable glyphs
                // DebugConsole.Log($"""
                //     {Keybinds.Left.GetCurrentGlyph()}//{Keybinds.Right.GetCurrentGlyph()} Move cursor. Hold {Keybinds.Hotkey1.GetCurrentGlyph()} to move faster
                //     /i[KBackspace]//(TODO KDel glyph) Delete to left/right
                //     {Keybinds.Hotkey2.GetCurrentGlyph()} Move and delete by word
                //     /i[KHome]///i[KEnd] Cursor to start/end
                //     {Keybinds.Left.GetCurrentGlyph()}//{Keybinds.Right.GetCurrentGlyph()} Move through command history
                //     /i[KTab] Accept autocomplete
                //     /i[KEnter] Execute command
                //     /i[KEsc] Toggle focus
                //     Non-control keys are used to type
                //     """, args[0]);
                DebugConsole.Log($"""
                    {ThemeColor.Imp.Str}[Left/Right]{ThemeColor.White.Str} Move cursor. Hold {ThemeColor.Imp.Str}[Shift]{ThemeColor.White.Str} to move faster
                    {ThemeColor.Imp.Str}[BkSp/Del]{ThemeColor.White.Str} Delete to left/right
                    {ThemeColor.Imp.Str}[Ctrl]{ThemeColor.White.Str} Move and delete by word
                    {ThemeColor.Imp.Str}[Home/End]{ThemeColor.White.Str} Cursor to start/end
                    {ThemeColor.Imp.Str}[Up/Down]{ThemeColor.White.Str} Move through command history
                    {ThemeColor.Imp.Str}[Tab]{ThemeColor.White.Str} Accept autocomplete
                    {ThemeColor.Imp.Str}[Enter]{ThemeColor.White.Str} Execute command
                    {ThemeColor.Imp.Str}[Esc]{ThemeColor.White.Str} Toggle focus
                    Non-control keys are used to type
                    """, args[0]);

                return;

            default:
                DebugConsole.Log(_LsDesc, args[0]);
                return;
        }
    }

    private static void _Cmd_overlay(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            DebugConsole.Log($"Must pass the overlay to target ({_Overlays})", args[0]);
            return;
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
                        return;

                    case _OverlayChange.Hide:
                        DebugUtil._SetShowDebugInfo(false);
                        return;

                    case _OverlayChange.Toggle:
                        DebugUtil._ToggleShowDebugInfo();
                        return;
                }

                return;

            // todo fix bugs with cursor pos and command color
            case "console":
                switch (ch)
                {
                    case _OverlayChange.Show:
                        DebugConsole._Show = true;
                        return;

                    case _OverlayChange.Hide:
                        DebugConsole._Show = false;
                        return;

                    case _OverlayChange.Toggle:
                        DebugConsole._Show ^= true;
                        return;
                }

                return;

            case "outline":
                switch (ch)
                {
                    case _OverlayChange.Show:
                        DebugUtil.DrawActorOutlines = true;
                        return;

                    case _OverlayChange.Hide:
                        DebugUtil.DrawActorOutlines = false;
                        return;

                    case _OverlayChange.Toggle:
                        DebugUtil.DrawActorOutlines ^= true;
                        return;
                }

                return;

            case "theme":
                switch (ch)
                {
                    case _OverlayChange.Show:
                        DebugUtil.DrawTheme = true;
                        return;

                    case _OverlayChange.Hide:
                        DebugUtil.DrawTheme = false;
                        return;

                    case _OverlayChange.Toggle:
                        DebugUtil.DrawTheme ^= true;
                        return;
                }

                return;

            case "input":
                switch (ch)
                {
                    case _OverlayChange.Show:
                        DebugUtil._SetShowInputView(true);
                        return;

                    case _OverlayChange.Hide:
                        DebugUtil._SetShowInputView(false);
                        return;

                    case _OverlayChange.Toggle:
                        DebugUtil._ToggleShowInputView();
                        return;
                }

                return;

            case "perf":
                switch (ch)
                {
                    case _OverlayChange.Show:
                        DebugUtil._PerfGraph.IsVisible = true;
                        return;

                    case _OverlayChange.Hide:
                        DebugUtil._PerfGraph.IsVisible = false;
                        return;

                    case _OverlayChange.Toggle:
                        DebugUtil._PerfGraph.IsVisible ^= true;
                        return;
                }

                return;

            default:
                DebugConsole.Log($"Valid overlays: {_Overlays}",
                    args[0], DebugConsole.LogLevel.Error);
                return;
        }
    }

    private enum _OverlayChange
    {
        Show,
        Hide,
        Toggle
    }

    private static void _Cmd_write(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            DebugConsole.Log($"Must pass the item to write ({_Writable})",
                args[0], DebugConsole.LogLevel.Error);

            return;
        }

        switch (args[1])
        {
            case "modlist":
                StringBuilder sb = new("Modlist: ");
                foreach (GameMod mod in ModLoader._LoadedMods)
                {
                    sb.Append($"{mod.GetName()}\n");
                }

                Console.WriteLine(sb.ToString());

                DebugConsole.Log("Wrote mod list to OS console", args[0]);

                return;

            case "registry":
                Console.WriteLine(Registry.ToString());
                DebugConsole.Log("Wrote registry to OS console", args[0]);

                return;

            case "lang":
                Console.WriteLine(Settings.Language.ToString());
                DebugConsole.Log("Wrote lang to OS console", args[0]);

                return;

            case "stage":
                Console.WriteLine(Stage.ToString());
                DebugConsole.Log("Wrote stage to OS console", args[0]);

                return;

            case "battlelog":
                Console.WriteLine($"Battle Log: {string.Join('\n', LogLib._LogText).RemoveFormattingCodes()}");
                DebugConsole.Log("Wrote battle log to OS console", args[0]);

                return;

            case "theme":
                Console.WriteLine(Settings.Theme.ToDetailedString());
                DebugConsole.Log("Wrote current theme to OS console", args[0]);

                return;

            default:
                DebugConsole.Log($"Valid items: {_Writable}",
                    args[0], DebugConsole.LogLevel.Error);

                return;
        }
    }

    private static void _Cmd_cleanup(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            DebugConsole.Log($"Must pass the item to cleanup ({_Cleanable})",
                args[0], DebugConsole.LogLevel.Error);
            return;
        }

        switch (args[1])
        {
            case "stage":
                Stage.Sort();
                DebugConsole.Log("Sorted Stage", args[0]);
                return;

            case "layout":
                Stage._RecalcLayoutWidgets();
                DebugConsole.Log("Recalculated ILayoutWidgets", args[0]);
                return;

            default:
                DebugConsole.Log($"Valid items: {_Cleanable}", args[0], DebugConsole.LogLevel.Error);
                return;
        }
    }

    private static void _Cmd_reload(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            DebugConsole.Log($"Must pass the item to reload ({_Reloadable})",
            args[0], DebugConsole.LogLevel.Error);
            return;
        }

        switch (args[1])
        {
            case "lang":
                Language.Reload();
                Console.WriteLine("Reloaded language");
                return;

            case "settings":
                Settings.Reload();
                Console.WriteLine("Reloaded settings");
                return;

            case "themes":
                // todo
                Console.WriteLine("Reloaded themes (NYI)");
                return;

            default:
                DebugConsole.Log($"Valid items: {_Reloadable}", args[0], DebugConsole.LogLevel.Error);
                return;
        }
    }

    private static void _Cmd_setting(ReadOnlySpan<string> args)
    {
        if (args.Length == 1)
        {
            DebugConsole.Log("Usage: `setting [setting] [value]`. Omit value to print the current value\n`setting reload` to reload from file, `setting reset` to reset to default\nIf a value is invalid, the default will be used instead",
            args[0], DebugConsole.LogLevel.Error);

            return;
        }

        if (args[1] == "reload")
        {
            Settings.Reload();
            return;
        }

        if (args[1] == "reset")
        {
            Settings.Create();
            Settings.Reload();
            return;
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
                DebugConsole.Log($"Setting `{args[1]}` couldn't be found",
                args[0], DebugConsole.LogLevel.Error);

                return;
            }
#pragma warning restore CS8600

            DebugConsole.Log($"{args[1]}={val}", args[0]);

            return;
        }

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

            case "Fullscreen":
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
                DebugConsole.Log($"Setting `{args[1]}` couldn't be found",
                    args[0], DebugConsole.LogLevel.Error);
                return;
        }

        DebugConsole.Log($"Changed setting `{args[1]}` to `{args[2]}`", args[0]);
        Settings.Reload();
    }

    private static void _Cmd_cycletheme(ReadOnlySpan<string> args)
    {
        ReadOnlySpan<Theme> themes = [.. Registry.Of<Theme>()];
        int i = themes.IndexOf(Settings.Theme);
        Settings.Theme = themes[i == themes.Length - 1 ? 0 : i + 1];
        DebugConsole.Log($"Theme changed to {Settings.Theme.GetName().RemoveFormattingCodes()}", args[0]);
    }

    #endregion

    #region Battle commands

    private const string _UnitIndexError = "unitindex (args[1]) must be an int 0-{0}";
    private const string _TurnsError = "turns (args[4]) must be an int > 0";
    private const string _StacksError = "stacks (args[5]) must be an int > 0";

    private static void _Cmd_buff(ReadOnlySpan<string> args)
    {
        if (args.Length < 4)
        {
            DebugConsole.Log(
                "Usage: `buff [unit index 0-7] [give/remove] [buff ID] [turns] [stacks]`. Can omit turns and stacks if removing",
                args[0], DebugConsole.LogLevel.Error);

            return;
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
            DebugConsole.Log(string.Format(_UnitIndexError, PosLib.Highest),
                args[0], DebugConsole.LogLevel.Error);
            return;
        }

        if (unitIndex is < 0 or > PosLib.Highest)
        {
            DebugConsole.Log(string.Format(_UnitIndexError, PosLib.Highest),
                args[0], DebugConsole.LogLevel.Error);
            return;
        }

        Unit unit = BattleLib.Battle.GetUnitAtPos(unitIndex);

        // Find Buff
        IRegistrable? registrable = Registry.Get(args[3]);
        if (registrable is not Buff)
        {
            DebugConsole.Log($"buff ID (args[3]) `{args[3]}` is not a valid buff",
                args[0], DebugConsole.LogLevel.Error);

            return;
        }

        Buff buff = (Buff) registrable!;

        // Determine give/remove
        switch (args[2])
        {
            case Give:
                // Find turns
                int turns;
                try
                {
                    turns = int.Parse(args[4]);
                }
                catch
                {
                    DebugConsole.Log(_TurnsError,
                        args[0], DebugConsole.LogLevel.Error);
                    return;
                }

                if (turns is < 1)
                {
                    DebugConsole.Log(_TurnsError,
                        args[0], DebugConsole.LogLevel.Error);
                    return;
                }

                // Find stacks
                int stacks;
                try
                {
                    stacks = int.Parse(args[5]);
                }
                catch
                {
                    DebugConsole.Log(_StacksError,
                        args[0], DebugConsole.LogLevel.Error);
                    return;
                }

                if (stacks is < 1)
                {
                    DebugConsole.Log(_StacksError,
                        args[0], DebugConsole.LogLevel.Error);
                    return;
                }

                unit.GiveBuffInstances(new BuffInstance(buff, turns, Math.Min(stacks, buff.MaxStacks)));

                DebugConsole.Log(
                    $"Gave {buff.GetNameWithoutIcon()}{ThemeColor.White.Str} to {unit.FormatName(false)}",
                    args[0]);

                return;

            case Remove:
                unit.RemoveBuffs(buff);

                // todo differentiate between removal and lack thereof? allow removal of x turns/stacks?
                DebugConsole.Log(
                    $"Removed {buff.GetNameWithoutIcon()}{ThemeColor.White.Str} from {unit.FormatName(false)}",
                    args[0]);

                return;

            default:
                DebugConsole.Log("args[2] must be `give` or `remove`",
                    args[0], DebugConsole.LogLevel.Error);
                return;
        }
    }

    #endregion
}
