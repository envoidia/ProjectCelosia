using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using API.Battle;
using API.Battle.State;
using API.Graphics;
using API.Input;
using API.Modding;
using API.Save;

namespace API.Debug;

/// <summary>
/// Console commands.
/// args[0] is always the current command
/// </summary>
public static class Commands
{
    public const string Show = "show";
    public const string Hide = "hide";

    public const string Give = "give";
    public const string Remove = "remove";

    private const string _LsDesc = "`ls cmd` to list all commands, `ls kb` to list all keybinds";
    private const string _Overlays = "info/console/outline/theme/input/perf";
    private const string _Writable = "modlist/registry/lang/stage/battlelog/theme";

    internal static void _Init()
    {
        const string Help = "help";
        Command.Register(Help, static _ => DebugConsole.Log($"""
            This console can be used for entering commands. It is for developers, and not part of gameplay. Misuse can brick your save
            Each command takes a certain number of arguments, separated by spaces
            {_LsDesc}
            """, Help), [], "Basic help info");

        Command.Register("clear", _ =>
        {
            DebugConsole._LogText.Clear();
            DebugConsole._Log.Text = "\n";
            DebugConsole._Line.Width = DebugConsole._MinBgWidth;
        }, [], "Clears the console");

        Command.Register("ls", _Cmd_ls, ["cmd/kb"], _LsDesc);

        Command.Register("echo",
            static args => DebugConsole.Log(string.Join(' ', args.ToArray(), 1, args.Length - 1),
            args[0]), ["text"], "Prints text");

        Command.Register("overlay", _Cmd_overlay, [_Overlays,
            "show/hide/blank to toggle"], "Enable/disable/toggle overlays");

        Command.Register("write", _Cmd_write, [_Writable], "Write various things to the console");

        Command.Register("buff", _Cmd_buff,
            ["unit index 0-7", "buff ID", "give/remove", "turns", "stacks"], "Give/remove buffs in battle");
    }

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
                        DebugConsole._SetShowDebugConsole(true);
                        return;

                    case _OverlayChange.Hide:
                        DebugConsole._SetShowDebugConsole(false);
                        return;

                    case _OverlayChange.Toggle:
                        DebugConsole._ToggleShowDebugConsole();
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
            DebugConsole.Log($"Must pass the item to write ({_Writable})", args[0], DebugConsole.LogLevel.Error);
        }

        switch (args[1])
        {
            case "modlist":
                Console.WriteLine(string.Join(", ", ModLoader._LoadedMods));
                return;

            case "registry":
                Console.WriteLine(Registry.ToString());
                return;

            case "lang":
                Console.WriteLine(Settings.Language.ToString());
                return;

            case "stage":
                Console.WriteLine(Stage.ToString());
                return;

            case "battlelog":
                Console.WriteLine(string.Join('\n', LogLib._LogText).RemoveFormattingCodes());
                return;

            case "theme":
                Console.WriteLine(Settings.Theme.ToDetailedString());
                return;

            default:
                DebugConsole.Log($"Valid items: {_Writable}",
                    args[0], DebugConsole.LogLevel.Error);
                return;
        }
    }

    private const string _UnitIndexError = "unitindex (args[1]) must be an int 0-{0}";
    private const string _TurnsError = "turns (args[4]) must be an int > 0";
    private const string _StacksError = "stacks (args[5]) must be an int > 0";

    private static void _Cmd_buff(ReadOnlySpan<string> args)
    {
        if (args.Length < 4)
        {
            DebugConsole.Log("Usage: `buff [unit index 0-7] [buff ID] [give/remove] [turns] [stacks]`\nCan omit turns and stacks if removing", args[0], DebugConsole.LogLevel.Error);
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
        IRegistrable? registrable = Registry.Get(args[2]);
        if (registrable is not Buff)
        {
            DebugConsole.Log($"buff ID (args[2]) `{args[2]}` is not a valid buff",
                args[0], DebugConsole.LogLevel.Error);
            return;
        }

        Buff buff = (Buff) registrable!;

        // Determine give/remove
        switch (args[3])
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

                DebugConsole.Log($"Gave {buff.GetNameWithoutIcon()}{ThemeColor.White.Str} to {unit.FormatName(false)}", args[0]);

                return;

            case Remove:
                unit.RemoveBuffs(buff);

                // todo differentiate between removal and lack thereof?
                DebugConsole.Log($"Removed {buff.GetNameWithoutIcon()}{ThemeColor.White.Str} from {unit.FormatName(false)}", args[0]);

                return;

            default:
                DebugConsole.Log("args[3] must be `give` or `remove`",
                    args[0], DebugConsole.LogLevel.Error);
                return;
        }
    }
}
