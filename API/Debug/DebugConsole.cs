using System;
using System.Collections.Generic;
using System.Linq;
using API.Extensions;
using API.Graphics;
using API.Input;
using API.Menu.State;
using API.Menu.Widget;
using API.Util;
using Microsoft.Xna.Framework.Input;

namespace API.Debug;

// todo scrollbar for outhist
public static class DebugConsole
{
    #region Props/Fields

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
            _OutHistLabel.IsVisible = value;
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
                _colorErr = ThemeColor.Neg;
                _Input.OnChangeText!.Invoke();
            }
            else if (StateMachine.State.Menus.Count > 0 && StateMachine.State.Menus[^1] == _Menu)
            {
                StateMachine.State.RemoveMenu();
                _color = ThemeColor.Gray;
                _colorErr = ThemeColor.Gray;
                _Input.OnChangeText!.Invoke();
            }
        }
    } = false;

    private static ThemeColor _color;
    private static ThemeColor _colorErr;

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

    private const int _InHistLimit = 128;
    internal static readonly List<string> _InHist = new(_InHistLimit);

    // todo scroll output
    private const int _DisplayedOutHistLines = 24;
    internal static readonly List<string> _OutHist = new(_DisplayedOutHistLines);

    /// <summary>
    /// Non-auto prop to avoid unneccessary update in <c>_ExecuteCommand</c>
    /// </summary>
    private static int _outHistIndex = 0;

    /// <summary>
    /// Lines scrolled up from the bottom of the output history
    /// </summary>
    private static int _OutHistIndex
    {
        get => _outHistIndex;
        set
        {
            _outHistIndex = Math.Clamp(value, 0, _OutHist.Count - _DisplayedOutHistLines);
            _UpdateOutHistText();
        }
    }

    /// <inheritdoc cref="_outHistIndex" />
    private static int _inHistIndex = -1;

    /// <summary>
    /// History depth. -1 = not in history. 0 = _Hist[^1], etc
    /// </summary>
    private static int _InHistIndex // todo compiler update (hint is wrong)
    {
        get => _inHistIndex;
        set
        {
            _inHistIndex = Math.Clamp(value, -1, _InHist.Count - 1);

            if (_inHistIndex != -1)
            {
                _Input.SetText(_InHist[^(_inHistIndex + 1)]);
                return;
            }

            _Input.Clear();
        }
    }

    internal const int _MinBgWidth = 1500;

    internal static readonly Label _OutHistLabel = new(RenderPriority.B3High, Core.Mono40)
    {
        Position = new(10, World.H - 35),
        Padding = new(10, 10, 10, 30),
        BackgroundType = BackgroundType.Rectangle, // todo also use current command width for this
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
            string? hints = CommandParser.GetCurrentHintText(args);
            string? match = CommandParser.GetCurrentAutocompleteMatch(args);

            // Trailing space fixes cursor pos bug
            _Command.Text = $"{_color.Str}>{(match == "" ? null : _colorErr.Str)}{text} ";

            _CommandHint.X = _Command.X + _Command.Width - 18;

            _CommandHint.Text = $"{ThemeColor.Gray.Str}{match}{(text.EndsWith(' ')
                ? null : ' ')}{hints}{(_Focused ? "" : "([Esc] to focus)")}";
        }
    };

    private static readonly Menu.Menu _Menu = new("DbgConsole")
    {
        InputWidgets = [_Input],
        GetInputPrompt = static () => null
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
        if (_OutHist.Count == _DisplayedOutHistLines)
        {
            _OutHist.RemoveFirst();
        }

        string color1 = logLevel switch
        {
            LogLevel.Info => ThemeColor.Accent.Str,
            LogLevel.Warning => ThemeColor.Imp.Str,
            LogLevel.Error => ThemeColor.Neg.Str,
            _ => throw new ClosedEnumsWhenException()
        };

        string color2 = logLevel == LogLevel.Info ? ThemeColor.White.Str : color1;

        ReadOnlySpan<string> msgLines = msg.Split('\n');

        _OutHist.Add($"{color1}[{source}]{color2} {msgLines[0]}");

        for (int i = 1; i < msgLines.Length; i++)
        {
            _OutHist.Add(msgLines[i]);
        }

        _UpdateOutHistText();

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

    internal static void _Init()
    {
        _Input.SubscribeToInput();

        Stage.Add(_Command);
        Stage.Add(_CommandHint);
        Stage.Add(_Cursor);
        Stage.Add(_OutHistLabel);
        Stage.Add(_Line);

        // todo
        _OutHist.Add("foo0");
        _OutHist.Add("foo1");
        _OutHist.Add("foo2");
        _OutHist.Add("foo3");
        _OutHist.Add("foo4");
        _OutHist.Add("foo5");
        _OutHist.Add("foo6");
        _OutHist.Add("foo7");
        _OutHist.Add("foo8");
        _OutHist.Add("foo9");
        _OutHist.Add("foo10");
        _OutHist.Add("foo11");
        _OutHist.Add("foo12");
        _OutHist.Add("foo13");
        _OutHist.Add("foo14");
        _OutHist.Add("foo15");
        _OutHist.Add("foo16");
        _OutHist.Add("foo17");
        _OutHist.Add("foo18");
        _OutHist.Add("foo19");
        _OutHist.Add("foo20");
        _OutHist.Add("foo21");
        _OutHist.Add("foo22");
        _OutHist.Add("foo23");
        _OutHist.Add("foo24");
        _OutHist.Add("foo25");
        _OutHist.Add("foo26");
        _OutHist.Add("foo27");
        _OutHist.Add("foo28");
    }

    internal static void _Update()
    {
        if (_Show && InputLib.IsKeyJustPressed(Keys.Escape))
        {
            _Focused ^= true;
            return;
        }

        if (!_Focused)
        {
            return;
        }

        if (InputLib.IsKeyJustPressed(Keys.Tab))
        {
            Span<string[]> args = CommandParser.TokenizeCommand(_Input.Text);

            string? match = CommandParser.GetCurrentAutocompleteMatch(args);
            if (match is not null)
            {
                _Input.Append(match);
            }

            return;
        }

        if (InputLib.Check(Keybinds.Up, true, TextInputWidget._MoveDelay))
        {
            if (InputLib.IsCtrlPressed())
            {
                _OutHistIndex++;
                return;
            }

            _InHistIndex++;
            return;
        }

        if (InputLib.Check(Keybinds.Down, true, TextInputWidget._MoveDelay))
        {
            if (InputLib.IsCtrlPressed())
            {
                _OutHistIndex--;
                return;
            }

            _InHistIndex--;
            return;
        }

        if (!InputLib.IsCtrlPressed())
        {
            return;
        }

        if (InputLib.IsKeyJustPressed(Keys.Home))
        {
            _OutHistIndex = _OutHist.Count - _DisplayedOutHistLines;
            return;
        }

        if (InputLib.IsKeyJustPressed(Keys.End))
        {
            _OutHistIndex = 0;
            return;
        }
    }

    private static void _UpdateOutHistText()
    {
        int start = Math.Max(0, _OutHist.Count - _DisplayedOutHistLines - _OutHistIndex);
        int take = Math.Min(_DisplayedOutHistLines, _OutHist.Count - start);

        _OutHistLabel.Text = string.Join("\n", _OutHist.Skip(start).Take(take)) + "\n";

        _Line.Width = Math.Max(_MinBgWidth, _OutHistLabel.Width + 20);
    }

    private static bool _ExecuteCommand()
    {
        string text = _Input.Text;
        if (text.Length == 0)
        {
            return false;
        }

        _inHistIndex = -1;
        _outHistIndex = 0;

        if (_InHist.Count == 0 || _InHist[^1] != text)
        {
            if (_InHist.Count > _InHistLimit)
            {
                _InHist.RemoveFirst();
            }

            _InHist.Add(text);
        }

        CommandParser.ExecuteCommand(text);

        return true;
    }

    #endregion
}
