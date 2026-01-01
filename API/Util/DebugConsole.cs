using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using API.Extensions;
using API.Graphics;
using API.Input;
using API.Menu;
using API.Menu.State;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace API.Util;

public static class DebugConsole
{
    private static bool _Show
    { // todo
        get;
        set
        {
            field = value;

            _Command.IsVisible = value;
            _History.IsVisible = value;
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

    private static readonly Label _Command = new(RenderPriority.B3High, Core.Koruri40)
    {
        Text = ">",
        Position = new(10, World.H - 5),
        Padding = new(10),
        Alignment = Alignment.BottomLeft,
        AnimType = AnimType.None,
        IsVisible = false
    };

    // todo impl
    private const int _HistLimit = 128;
    private static readonly List<string> _Hist = new(_HistLimit);

    private const int _DisplayedCount = 24;
    private static readonly List<string> _LogText = new(_DisplayedCount);

    private const int _MinBgWidth = 500;

    internal static readonly Label _History = new(RenderPriority.B3High, Core.Koruri40)
    {
        Position = new(10, World.H - 20),
        Padding = new(10, 10, 10, 20),
        HasBackground = true,
        MinBackgroundSize = new(_MinBgWidth, 0),
        Alignment = Alignment.BottomLeft,
        AnimType = AnimType.None,
        IsVisible = false
    };

    private static readonly TextInput _Input = new(_Command,
        ExecuteCommand)
    {
        OnChangeText = () => _Command.Text = $"{_color.Str}>{_Input!.Text}   {(_Focused ? "" : "([esc] to focus)")}"
    };

    private static readonly Menu.Menu _Menu = new("DbgConsole")
    {
        InputWidgets = [_Input]
    };

    private static readonly RectangleActor _Line = new(ThemeColor.Gray, RenderPriority.Highest)
    {
        Position = new(0, World.H - 50),
        Size = new(_MinBgWidth, 1),
        IsVisible = false
    };

    static DebugConsole()
    {
        Stage.Add(_Command);
        Stage.Add(_History);
        Stage.Add(_Line);
    }

    internal static void Update(GameTime gt)
    {
        if (InputLib.IsKeyJustPressed(Keys.F2))
        {
            _Show ^= true;
        }

        if (InputLib.IsKeyJustPressed(Keys.Escape))
        {
            _Focused ^= true;
        }

        //Console.WriteLine(RenderPriority.B3High.ToString() + " " + RenderPriority.Highest.ToString() + " " + _Command.Priority.ToString());
        //_Command.Priority = RenderPriority.B3High;
    }

    private static bool ExecuteCommand()
    {
        string t = _Input.Text;

        if (t.Length == 0)
        {
            return false;
        }

        _Hist.Add(t);
        Log(t, nameof(DebugConsole));

        return true;
    }

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

        _History.Text = string.Join('\n', _LogText) + '\n';
        _Line.Width = Math.Max(_MinBgWidth, _History.Width + 20);

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
}
