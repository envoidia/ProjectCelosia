using System;
using System.Collections.Generic;
using System.Linq;
using API.Extensions;
using API.Graphics;
using API.Input;
using API.Menu.State;
using API.Menu.Widget;
using API.Util;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace API.Debug;

public static class DebugConsole
{
    #region Props/Fields

    // todo wait for compiler update (hint is incorrect)
    private static bool _Show
    {
        get;
        set
        {
            field = value;

            _Command.IsVisible = value;
            _CommandHint.IsVisible = value;
            _Cursor.IsVisible = value;
            _Log.IsVisible = value;
            _Line.IsVisible = value;

            _Focused = value;
        }
    } = false;

    private static bool _Focused
    {
        get;
        set
        {
            field = value;
            _input.CheckInput = value;

            if (value && (StateMachine.State.Menus.Count == 0 || StateMachine.State.Menus[^1] != _menu))
            {
                StateMachine.State.AddMenu(_menu);
                _color = ThemeColor.Imp;
                _input.OnChangeText!.Invoke();
            }
            else if (StateMachine.State.Menus.Count > 0 && StateMachine.State.Menus[^1] == _menu)
            {
                StateMachine.State.RemoveMenu();
                _color = ThemeColor.Gray;
                _input.OnChangeText!.Invoke();
            }
        }
    } = false;

    private static ThemeColor _color;

    private static readonly Label _Command = new(RenderPriority.B3High, Core.Koruri40)
    {
        Text = "X", // Must be init with some text or cursor never shows (why?? probably some weird FSS internals)
        Position = new(10, World.H - 5),
        Padding = new(10),
        Alignment = Alignment.BottomLeft,
        AnimType = AnimType.None,
        IsVisible = false
    };

    /// <summary>
    /// Autocomplete/hint portion of the command
    /// </summary>
    private static readonly Label _CommandHint = new(RenderPriority.B3Med, Core.Koruri40)
    {
        Position = new(10, World.H - 5),
        Padding = new(10),
        Alignment = Alignment.BottomLeft,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private static readonly ARectangle _Cursor = new(ThemeColor.Gray, RenderPriority.B3High)
    {
        IsVisible = false
    };

    // todo impl
    private const int _HistLimit = 128;
    private static readonly List<string> _Hist = new(_HistLimit);

    private const int _DisplayedCount = 24;
    internal static readonly List<string> _LogText = new(_DisplayedCount);

    private static int _histIndex = -1;

    /// <summary>
    /// History depth. -1 = not in history. 0 = _Hist[^1], etc
    /// </summary>
    private static int _HistIndex // todo same compiler update
    {
        get => _histIndex;
        set
        {
            _histIndex = Math.Clamp(value, -1, _Hist.Count - 1);

            if (_histIndex != -1)
            {
                _input.SetText(_Hist[^(_histIndex + 1)]);
                return;
            }

            _input.Clear();
        }
    }

    internal const int _MinBgWidth = 1500;

    internal static readonly Label _Log = new(RenderPriority.B3High, Core.Koruri40)
    {
        Position = new(10, World.H - 20),
        Padding = new(10, 10, 10, 20),
        HasBackground = true, // todo also use current command width for this
        MinBackgroundSize = new(_MinBgWidth, 0),
        Alignment = Alignment.BottomLeft,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private static TextInputWidget _input = null!;
    private static Menu.Menu _menu = null!;

    internal static readonly ARectangle _Line = new(ThemeColor.Gray, RenderPriority.Highest)
    {
        Position = new(0, World.H - 50),
        Size = new(_MinBgWidth, 1),
        IsVisible = false
    };

    #endregion

    static DebugConsole()
    {
        Stage.Add(_Command);
        Stage.Add(_CommandHint);
        Stage.Add(_Cursor);
        Stage.Add(_Log);
        Stage.Add(_Line);

        Core.PostCoreInit += _PostCoreInit;
    }

    #region Logging

    /// <summary>
    /// Write a message to the ingame debug log and the attached OS console
    /// </summary>
    /// <param name="msg">Message</param>
    /// <param name="source">Origin to display for the message. API uses the name of the current class, but mods should
    /// use more specific names so it's clear exactly what mod it's coming from</param>
    /// <param name="logLevel">Color to use to indicate message severity</param>
    public static void Log(string msg, string source, LogLevel logLevel = LogLevel.Info)
    {
        if (_LogText.Count == _DisplayedCount)
        {
            _LogText.RemoveFirst();
        }

        _LogText.Add($"{logLevel switch
        {
            LogLevel.Info => ThemeColor.White.Str,
            LogLevel.Warning => ThemeColor.Imp.Str,
            LogLevel.Error => ThemeColor.Neg.Str,
            _ => throw new ClosedEnumsWhenException()
        }}[{source}] {msg}");

        _Log.Text = string.Join('\n', _LogText) + '\n';
        _Line.Width = Math.Max(_MinBgWidth, _Log.Width + 20);

        Console.WriteLine($"{logLevel switch
        {
            LogLevel.Info => "",
            LogLevel.Warning => "\e[0;33m",
            LogLevel.Error => "\e[0;31m",
            _ => throw new ClosedEnumsWhenException()
        }}[{source}] {msg}");
    }

    /// <summary>
    /// Determines the color of log messages
    /// </summary>
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    #endregion

    #region Internals

    // Must be set after core instance init due to <c>TextInput</c> ctor depending on <c>Core</c> ctor
    internal static void _PostCoreInit()
    {
        _input = new(_Command, _Cursor, _ExecuteCommand, false)
        {
            OnChangeText = () =>
            {
                ReadOnlySpan<string> args = _TokenizeCommand(_input.Text);

                // Hints
                string? hints = null;

                if (args.Length != 0 && Command._Commands.TryGetValue(args[0], out Command? cmd))
                {
                    int skip = args.Length - 1;
                    if (skip < cmd.Hints.Length)
                    {
                        hints = $"{(_input.Text.EndsWith(' ') ? null : ' ')}{string.Join(' ',
                            cmd.Hints.Skip(skip).Select(s => $"[{s}]"))}";
                    }
                }

                // Autocomplete
                string? match = args.Length == 1 ? _GetAutocompleteMatch(_input.Text) : null;

                // Trailing space fixes cursor pos bug
                _Command.Text = $"{_color.Str}>{_input.Text} ";

                _CommandHint.X = _Command.X + _Command.Width - 7;
                _CommandHint.Text = $"{ThemeColor.Gray.Str}{match}{hints}{(_Focused ? "" : "   ([esc] to focus)")}";
            }
        };

        _menu = new("DbgConsole")
        {
            InputWidgets = [_input]
        };
    }

    internal static void _Update(GameTime gt)
    {
        if (InputLib.IsKeyJustPressed(Keys.F2))
        {
            _ToggleShowDebugConsole();
        }

        if (InputLib.IsKeyJustPressed(Keys.Escape))
        {
            _Focused ^= true;
        }

        if (!_Focused)
        {
            return;
        }

        if (InputLib.IsKeyJustPressed(Keys.Tab))
        {
            ReadOnlySpan<string> args = _TokenizeCommand(_input.Text);

            if (args.Length == 1)
            {
                string? match = _GetAutocompleteMatch(args[0]);
                if (match is not null)
                {
                    _input.Append(match);
                }
            }
        }

        if (InputLib.Check(Keybinds.Up, true, TextInputWidget._MoveDelay))
        {
            _HistIndex++;
        }
        else if (InputLib.Check(Keybinds.Down, true, TextInputWidget._MoveDelay))
        {
            _HistIndex--;
        }
    }

    internal static void _SetShowDebugConsole(bool show)
    {
        _Show = show;
    }

    internal static void _ToggleShowDebugConsole()
    {
        _Show ^= true;
    }

    /// <summary>
    /// Executes the specified command
    /// </summary>
    public static void ExecuteCommand(string t)
    {
        if (_Hist.Count == 0 || _Hist[^1] != t)
        {
            _Hist.Add(t);
        }

        ReadOnlySpan<string> args = _TokenizeCommand(_input.Text);

        if (!Command._Commands.TryGetValue(args[0], out Command? cmd))
        {
            Log($"{args[0]} is not a recognized command. Use `help` for help and `ls cmd` to list all commands",
                nameof(DebugConsole), LogLevel.Error);
            // todo suggest close matches
            return;
        }

        cmd.Action(args);
    }

    private static bool _ExecuteCommand()
    {
        if (_input.Text.Length == 0)
        {
            return false;
        }

        _histIndex = -1;
        ExecuteCommand(_input.Text);

        return true;
    }

    /// <summary>
    /// Splits a string by whitespace and returns a ReadOnlySpan without empty entries or whitespace
    /// </summary>
    private static ReadOnlySpan<string> _TokenizeCommand(string str)
    {
        return str.Split((char[]) null!,
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    /// <returns>The non-shared part of the closest autocomplete match for the currently typed command, or null</returns>
    private static string? _GetAutocompleteMatch(string str)
    {
        string? match = Command._Commands.Keys
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

    #endregion
}
