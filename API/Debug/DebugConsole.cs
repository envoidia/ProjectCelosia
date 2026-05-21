using System;
using System.Collections.Generic;
using System.Linq;
using API.Extensions;
using API.Graphics;
using API.Input;
using API.Menu.State;
using API.Menu.Widget;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework.Input;

namespace API.Debug;

// todo scrollbar for outhist
public static class DebugConsole
{
    #region Props/Fields

    /// <summary>
    /// Whether to mirror console messages to stdout
    /// </summary>
    public static bool Mirror = false;

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
            _Scrollbar.IsVisible = _OutHist.Count > _DisplayedOutHistLines && value;
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
                _color = ThemeColor.Emphasis;
                _colorErr = ThemeColor.Negative;
                _Input.OnChangeText!.Invoke();

                return;
            }

            if (StateMachine.State.Menus.Count > 0 && StateMachine.State.Menus[^1] == _Menu)
            {
                StateMachine.State.RemoveMenu();
                _color = ThemeColor.Midtone;
                _colorErr = ThemeColor.Midtone;
                _Input.OnChangeText!.Invoke();
            }
        }
    } = false;

    private static ThemeColor _color;
    private static ThemeColor _colorErr;

    private const int _TextX = 10;
    private const int _TextOff = 17;
    private const int _TextXWithOff = _TextX + _TextOff;
    private const int _PaddingAmt = 10;
    private static readonly Padding _Padding = new(_PaddingAmt);

    private static readonly Label _Command = new(RenderPriority.Highest, Core.Mono40)
    {
        Text = ">",
        Position = new(_TextX, World.H - 10),
        Padding = _Padding,
        Alignment = Alignment.BottomLeft,
        AnimType = AnimType.None,
        IsVisible = false
    };

    /// <summary>
    /// Autocomplete/hint portion of the command
    /// </summary>
    private static readonly Label _CommandHint = new(RenderPriority.Highest, Core.Mono40)
    {
        Position = new(_TextX, World.H - 10),
        Padding = _Padding,
        Alignment = Alignment.BottomLeft,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private static readonly ARectangle _Cursor = new(ThemeColor.Midtone, RenderPriority.Highest)
    {
        Position = new(_TextXWithOff, World.H - 50),
        IsVisible = false
    };

    private static readonly ARectangle _Scrollbar = new(ThemeColor.Midtone, RenderPriority.Highest)
    {
        X = 10,
        Alignment = Alignment.BottomLeft,
        OutlineColor = ThemeColor.Midtone
    };

    private const int _InHistLimit = 128;
    internal static readonly List<string> _InHist = new(_InHistLimit);

    private const int _DisplayedOutHistLines = 24;
    internal static readonly List<string> _OutHist = new(128);

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
            int max = _OutHist.Count - _DisplayedOutHistLines;

            if (max < 0)
            {
                return;
            }

            _outHistIndex = Math.Clamp(value, 0, max);
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

    internal static readonly Label _OutHistLabel = new(RenderPriority.Highest, Core.Mono40)
    {
        Position = new(_TextXWithOff, World.H - 35),
        MaxWidth = World.W - _TextXWithOff,
        Padding = new(_TextXWithOff, 10, 10, 40),
        BackgroundType = BackgroundType.Rectangle,
        MinBackgroundSize = new(_MinBgWidth, 0), // todo also use current command width for this
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
            // todo check all params for errors and not just the last one
            _Command.Text = $"{_color.Str}>{(match == "" ? null : _colorErr.Str)}{text} ";

            _CommandHint.X = _Command.X + _Command.Width - 18;

            _CommandHint.Text = $"{ThemeColor.Midtone.Str}{match}{(text.EndsWith(' ')
                ? null : ' ')}{hints}{(_Focused ? "" : "([Esc] to focus)")}";
        }
    };

    private static readonly Menu.Menu _Menu = new("DbgConsole")
    {
        InputWidgets = [_Input],
        GetInputPrompt = static () => null
    };

    internal static readonly ARectangle _Line = new(ThemeColor.Midtone, RenderPriority.Highest)
    {
        Position = new(0, World.H - 60),
        Size = new(_MinBgWidth, 1),
        IsVisible = false,
        OutlineColor = ThemeColor.Midtone
    };

    #endregion

    #region Functions

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
            LogLevel.Warning => ThemeColor.Emphasis.Str,
            LogLevel.Err => ThemeColor.Negative.Str,
            _ => throw new ClosedEnumsWhenException()
        };

        string color2 = logLevel == LogLevel.Info ? ThemeColor.Fg.Str : color1;

        ReadOnlySpan<string> msgLines = msg.Split('\n');

        _OutHist.Add($"{color1}[{source}]{color2} {msgLines[0]}");

        for (int i = 1; i < msgLines.Length; i++)
        {
            _OutHist.Add(msgLines[i]);
        }

        _UpdateOutHistText();

        if (!Mirror)
        {
            return;
        }

        Console.WriteLine($"{logLevel switch
        {
            LogLevel.Info => "",
            LogLevel.Warning => "\e[0;33m",
            LogLevel.Err => "\e[0;31m",
            _ => throw new ClosedEnumsWhenException()
        }}[{source}] {msg}");
    }

    public static void Log(LogMessage msg)
    {
        Log(msg.Msg, msg.Source, msg.LogLevel);
    }

    /// <summary>
    /// Clears output history
    /// </summary>
    public static void ClearOutHist()
    {
        _OutHist.Clear();
        _UpdateOutHistText();
    }

    #endregion

    #region Internals

    internal static void _Init()
    {
        _Input.SubscribeToInput();

        // todo stop codes from working ?
        _Command.RichTextLayout.SupportsCommands = true;

        Stage.Add(_Command);
        Stage.Add(_CommandHint);
        Stage.Add(_Cursor);
        Stage.Add(_OutHistLabel);
        Stage.Add(_Scrollbar);
        Stage.Add(_Line);

        _UpdateScrollbar();
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

        if (Settings.EnableMouse && (InputLib.GetMouseScroll() / InputLib.ScrollPerMouseWheelTick) is int scroll and not 0)
        {
            _OutHistIndex += scroll;
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
        }
    }

    private static void _UpdateOutHistText()
    {
        int start = Math.Max(0, _OutHist.Count - _DisplayedOutHistLines - _OutHistIndex);
        int count = Math.Min(_DisplayedOutHistLines, _OutHist.Count - start);

        _OutHistLabel.Text = $"{string.Join("\n", _OutHist.Skip(start).Take(count))}\n";

        _UpdateScrollbar();
        _Scrollbar.IsVisible = _OutHist.Count > _DisplayedOutHistLines;

        _Line.Width = Math.Max(_MinBgWidth, _OutHistLabel.Width + 20);
    }

    // todo why does it visually appear 1+ too high up sometimes when outhist updates ???
    private static void _UpdateScrollbar()
    {
        // Portion currently displayed
        float ratio = Math.Min((float) _DisplayedOutHistLines / _OutHist.Count, 1);

        // Maximum range for the bar to move. Slightly less than height so it leaves a margin on the edges
        float range = (_OutHistLabel.Height - 50) + _OutHistLabel.Padding.TB - 20;

        float barLength = range * ratio;
        range -= barLength;

        // 0 = bottom; 1 = top
        float scrollAmt = (float) _OutHistIndex / Math.Max(_OutHist.Count - _DisplayedOutHistLines, 0);

        _Scrollbar.Y = _OutHistLabel.Y - _Scrollbar.Height - 35 - (range * scrollAmt);
        _Scrollbar.Size = new(10, (int) barLength);
    }

    private static bool _ExecuteCommand()
    {
        if (_Input.Text.Length == 0)
        {
            return false;
        }

        _inHistIndex = -1;
        _outHistIndex = 0;

        if (_InHist.Count == 0 || _InHist[^1] != _Input.Text)
        {
            if (_InHist.Count > _InHistLimit)
            {
                _InHist.RemoveFirst();
            }

            _InHist.Add(_Input.Text);
        }

        LogMessage? msg = CommandParser.ExecuteCommand(_Input.Text);

        if (msg is not null)
        {
            Log(msg.Value);
        }

        return true;
    }

    #endregion
}
