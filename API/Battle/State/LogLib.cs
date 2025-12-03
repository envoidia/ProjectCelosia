using System.Collections.Generic;
using API.Graphics;
using API.Input;
using API.Menu.State;
using Microsoft.Xna.Framework;
using static API.Input.InputPrompts;
using static API.Battle.State.BattleLib;
using System;
using API.Extensions;

namespace API.Battle.State;

public static class LogLib {
    private static readonly Label BattleLog = new(Core.StageBattle) { Position = new Vector2(World.W2 - 300, 405) };

    internal static readonly List<string> LogText = new(1024); // todo decide capacity

    /// <summary>
    /// Amount of lines scrolled upwards
    /// </summary>
    private static int logScroll = 0;

    // todo limit size, try to consolidate to 1 fn?
    public static void Add(params List<string> str) {
        LogText.AddRange(str);
        logScroll = 0;
        UpdateLog();
    }

    public static void Add(string[] str) {
        LogText.AddRange(str);
        logScroll = 0;
        UpdateLog();
    }

    private static void UpdateLog() => BattleLog.Text = FormatLog();// todo full log

    private static string FormatLog() =>
        /* todo
int lines = 8;
int scroll = 0;

if(NavPath.Peek() == MenuType.Log) {
lines = 48;
scroll = logScroll;
}

int start = Math.Max(0, LogText.Count - lines - scroll);
int end = Math.Min(start + lines, LogText.Count);*/
        string.Join("\n", LogText);
}
