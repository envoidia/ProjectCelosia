using System;
using System.Collections.Generic;
using API.Extensions;
using API.Graphics;
using API.Input;
using API.Menu.State;
using API.Menu.Widget;
using API.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace API.Debug;

public static class DebugConsole
{
    #region Props/Fields

    // todo wait for compiler update (hint is incorrect)
    internal static bool _Show
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

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
            _Input.CheckInput = value;

            if (value && (StateMachine.State.Menus.Count == 0 || StateMachine.State.Menus[^1] != _Menu))
            {
                StateMachine.State.AddMenu(_Menu);
                _color = ThemeColor.Imp;
                _Input.OnChangeText!.Invoke();
            }
            else if (StateMachine.State.Menus.Count > 0 && StateMachine.State.Menus[^1] == _Menu)
            {
                StateMachine.State.RemoveMenu();
                _color = ThemeColor.Gray;
                _Input.OnChangeText!.Invoke();
            }
        }
    } = false;

    private static ThemeColor _color;

    private static readonly Label _Command = new(RenderPriority.B3High, Core.Mono40)
    {
        Text = ">",
        Position = new(10, World.H - 10),
        Padding = new(10),
        Alignment = Alignment.BottomLeft,
        AnimType = AnimType.None,
        IsVisible = false
    };

    /// <summary>
    /// Autocomplete/hint portion of the command
    /// </summary>
    private static readonly Label _CommandHint = new(RenderPriority.B3Med, Core.Mono40)
    {
        Position = new(10, World.H - 10),
        Padding = new(10),
        Alignment = Alignment.BottomLeft,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private static readonly ARectangle _Cursor = new(ThemeColor.Gray, RenderPriority.B3High)
    {
        Position = new(27, World.H - 50),
        IsVisible = false
    };

    private const int _HistLimit = 128;
    internal static readonly List<string> _Hist = new(_HistLimit);

    // todo scroll output
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
                _Input.SetText(_Hist[^(_histIndex + 1)]);
                return;
            }

            _Input.Clear();
        }
    }

    internal const int _MinBgWidth = 1500;

    internal static readonly Label _Log = new(RenderPriority.B3High, Core.Mono40)
    {
        Position = new(10, World.H - 35),
        Padding = new(10, 10, 10, 30),
        HasBackground = true, // todo also use current command width for this
        MinBackgroundSize = new(_MinBgWidth, 0),
        Alignment = Alignment.BottomLeft,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private static readonly TextInputWidget _Input = new(_Command, _Cursor, _ExecuteCommand, false)
    {
        OnChangeText = () =>
        {
            string text = _Input!.Text;
            Span<string[]> args = CommandParser.TokenizeCommand(text);

            // Hints and autocomplete
            string? hints = null;
            string? match = null;

            if (args.Length > 0 && args[^1].Length > 0)
            {
                hints = CommandParser.GetHintText(args[^1], args.Length > 1);

                if (args[^1].Length == 1)
                {
                    string str = args[^1][^1];

                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        match = CommandParser.GetAutocompleteMatch(str);
                    }
                }
            }

            // Trailing space fixes cursor pos bug
            _Command.Text = $"{_color.Str}>{text} ";

            _CommandHint.X = _Command.X + _Command.Width - 18;

            _CommandHint.Text = $"{ThemeColor.Gray.Str}{match}{(text.EndsWith(' ')
                ? null : ' ')}{hints}{(_Focused ? "" : "([Esc] to focus)")}";
        }
    };

    private static readonly Menu.Menu _Menu = new("DbgConsole")
    {
        InputWidgets = [_Input]
    };

    internal static readonly ARectangle _Line = new(ThemeColor.Gray, RenderPriority.Highest)
    {
        Position = new(0, World.H - 60),
        Size = new(_MinBgWidth, 1),
        IsVisible = false
    };

    #endregion

    #region Logging

    /// <summary>
    /// Write a message to the ingame console
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

        string color1 = logLevel switch
        {
            LogLevel.Info => ThemeColor.Accent.Str,
            LogLevel.Warning => ThemeColor.Imp.Str,
            LogLevel.Error => ThemeColor.Neg.Str,
            _ => throw new ClosedEnumsWhenException()
        };

        string color2 = logLevel == LogLevel.Info ? ThemeColor.White.Str : color1;

        _LogText.Add($"{color1}[{source}]{color2} {msg}");

        _Log.Text = string.Join('\n', _LogText) + '\n';
        _Line.Width = Math.Max(_MinBgWidth, _Log.Width + 20);

        // If mirroring to external log/console, must sanitize '［', '['
        // Console.WriteLine($"{logLevel switch
        // {
        //     LogLevel.Info => "",
        //     LogLevel.Warning => "\e[0;33m",
        //     LogLevel.Error => "\e[0;31m",
        //     _ => throw new ClosedEnumsWhenException()
        // }}[{source}] {msg}");
    }

    #endregion

    #region Internals

    // Must be set after core instance init due to <c>TextInput</c> ctor depending on <c>Core</c> ctor
    internal static void _Init()
    {
        _Input.SubscribeToInput();

        Stage.Add(_Command);
        Stage.Add(_CommandHint);
        Stage.Add(_Cursor);
        Stage.Add(_Log);
        Stage.Add(_Line);
    }

    internal static void _Update(GameTime gt)
    {
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
            string text = _Input.Text.Split('|', StringSplitOptions.TrimEntries)[^1];

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            string? match = CommandParser.GetAutocompleteMatch(text);
            if (match is not null)
            {
                _Input.Append(match);
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

    private static bool _ExecuteCommand()
    {
        string text = _Input.Text;
        if (text.Length == 0)
        {
            return false;
        }

        _histIndex = -1;

        if (_Hist.Count == 0 || _Hist[^1] != text)
        {
            if (_Hist.Count > _HistLimit)
            {
                _Hist.RemoveFirst();
            }

            _Hist.Add(text);
        }

        CommandParser.ExecuteCommand(text);

        return true;
    }

    #endregion
}
