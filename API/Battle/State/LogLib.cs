using System.Collections.Generic;
using API.Graphics;
using Microsoft.Xna.Framework;

namespace API.Battle.State;

public static class LogLib {
    internal static readonly Label _BattleLog = new() { Position = new(World.W2 - 300, 405) };

    internal static readonly List<string> _LogText = new(1024); // todo decide capacity

    /// <summary>
    /// Amount of lines scrolled upwards
    /// </summary>
    private static int _logScroll = 0;

    // todo limit size, try to consolidate to 1 fn? take any ienumerabel?
    /// <summary>
    /// Add to the battle log
    /// </summary>
    public static void Add(params List<string> str) {
        _LogText.AddRange(str);
        _logScroll = 0;
        _UpdateLog();
    }

    /// <inheritdoc cref="Add(List&lt;string&gt;)" />
    public static void Add(string[] str) {
        _LogText.AddRange(str);
        _logScroll = 0;
        _UpdateLog();
    }

    private static void _UpdateLog() => _BattleLog.Text = _FormatLog(); // todo full log

    private static string _FormatLog() =>
        /* todo
int lines = 8;
int scroll = 0;

if(NavPath.Peek() == MenuType.Log) {
lines = 48;
scroll = logScroll;
}

int start = Math.Max(0, LogText.Count - lines - scroll);
int end = Math.Min(start + lines, LogText.Count);*/
        string.Join("\n", _LogText);
}
