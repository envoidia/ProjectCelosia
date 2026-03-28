using System;
using System.Collections.Generic;
using System.Linq;
using API.Graphics;
using API.Input;
using API.Menu;
using API.Menu.State;
using API.Menu.Widget;
using Microsoft.Xna.Framework;

namespace API.Battle.State;

public static class LogLib
{
    private const int _BaseLines = 8;
    private const int _FullLines = 34;

    private static readonly Vector2 _LogPos = new(World.W2 - 300 + 700, 405);

    internal static readonly Label _BattleLog = new()
    {
        Position = _LogPos,
        Padding = new(10),
        BackgroundType = BackgroundType.Parellelogram,
        VerticalSpacing = -1
    };

    internal static readonly List<string> _LogText = new(1024); // todo decide capacity

    /// <summary>
    /// Amount of lines scrolled upwards
    /// </summary>
    private static int _logScroll = 0;

    private static readonly ARectangle _Scrollbar = new(ThemeColor.White, RenderPriority.B3Med)
    {
        OutlineColor = ThemeColor.White,
        Rotation = ListWidget.NormalSlant / -84.85f
    };

    /// <summary>
    /// Used to interpolate the scrollbar
    /// </summary>
    private static Vector2 _lastScrollbarPos = new(ListWidget.UninitializedScrollbarPos);

    private static readonly Menu.Menu _Menu = new("Log", Parellelograms.CoverLeft, _Scrollbar)
    {
        GetInputPrompt = static () => Menu.State.State.GetInputPromptString(
            InputPrompts.ScrollUpDown, InputPrompts.Back),

        OnCreate = static () =>
        {
            _BattleLog.Position = new(25, 10);
            _BattleLog.Priority = RenderPriority.B3Med;
            _BattleLog.BackgroundType = BackgroundType.None;
            _BattleLog.Text = _FormatLog(true);
        },

        OnDestroy = static () =>
        {
            _BattleLog.Position = _LogPos;
            _BattleLog.Priority = RenderPriority.B1Med;
            _BattleLog.BackgroundType = BackgroundType.Parellelogram;
            _BattleLog.Text = _FormatLog(false);
        },

        OnUpdate = static gt =>
        {
            // todo better solution (snap to 0/1 when opening/closing) (or at least buffer inputs)
            if (Parellelograms.CoverLeft.Prog == 1 && InputLib.Check(Keybinds.Back))
            {
                States.Battle.RemoveMenu();
                return;
            }

            int newScroll = MenuLib.CheckLogScroll(_logScroll, _LogText.Count,
                StateMachine.State.Menus[^1] == _Menu ? _FullLines : _BaseLines);

            if (newScroll != _logScroll)
            {
                _logScroll = newScroll;
                _UpdateLog();
            }

            if (InputLib.IsKeyJustPressed(Microsoft.Xna.Framework.Input.Keys.F5))
            {
                Add("a", "b", "c", "d", "e", "f", "g", "h");
            }
            if (InputLib.IsKeyJustPressed(Microsoft.Xna.Framework.Input.Keys.F6))
            {
                _LogText.Clear();
                _UpdateLog();
            }
        }
    };

    // todo limit size, try to consolidate to 1 fn? take any ienumerabel?
    /// <summary>
    /// Add to the battle log
    /// </summary>
    public static void Add(params ReadOnlySpan<string> str)
    {
        _LogText.AddRange(str);
        _logScroll = 0;
        _UpdateLog();
    }

    /// <inheritdoc cref="Add(ReadOnlySpan&lt;string&gt;)" />
    public static void Add(List<string> str)
    {
        _LogText.AddRange(str);
        _logScroll = 0;
        _UpdateLog();
    }

    internal static void _Create()
    {
        States.Battle.AddMenu(_Menu);
    }

    private static void _UpdateLog()
    {
        _UpdateScrollbar();
        _BattleLog.Text = _FormatLog(_IsLogMenu());
    }

    private static void _UpdateScrollbar()
    {
        // Portion currently displayed
        float ratio = Math.Min((float) _FullLines / _LogText.Count, 1);

        // Maximum range for the bar to move. Slightly less than height so it leaves a margin on the edges
        float range = World.H - 10;

        float barLength = range * ratio;
        range -= barLength;

        // 0 = bottom; 1 = top
        float scrollAmt = (float) _logScroll / Math.Max(_LogText.Count - _FullLines, 0);

        float x = 2035 + ((range * scrollAmt) / RenderLib.DefaultSlant);
        float y = (World.H - 5) - _Scrollbar.Height - (range * scrollAmt);

        Vector2 pos = new(x, y);

        // if (_lastScrollbarPos.X != ListWidget.UninitializedScrollbarPos)
        // {
        //     pos = Vector2.SmoothStep(_lastScrollbarPos, pos, 0.15f);
        // }

        //_lastScrollbarPos = pos;
        _Scrollbar.Position = pos;

        _Scrollbar.Size = new(10, (int) barLength);
    }

    private static string _FormatLog(bool fullLines)
    {
        int lines = _BaseLines;
        int scroll = 0;

        if (fullLines)
        {
            lines = _FullLines;
            scroll = _logScroll;
        }

        int start = Math.Max(0, _LogText.Count - lines - scroll);
        int end = Math.Min(start + lines, _LogText.Count);

        return string.Join('\n', _LogText.Skip(start).Take(end));
    }

    private static bool _IsLogMenu()
    {
        List<Menu.Menu> menus = StateMachine.State.Menus;
        return menus.Count > 0 && menus[^1] == _Menu;
    }
}
