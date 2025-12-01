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

public sealed class MenuLog : IState {

    #region Fields

    private static readonly Label BattleLog = new(Core.StageBattle) { Position = new Vector2(World.W2 - 300, 405) };

    internal static readonly List<string> LogText = new(1024); // todo decide capacity

    /// <summary>
    /// Amount of lines scrolled upwards
    /// </summary>
    private static int logScroll = 0;

    #endregion

    #region Impl

    public MenuLog() {
        if (Core.MenuLog is not null) {
            throw new InvalidOperationException(string.Format(Lang.MultipleInstance, nameof(MenuLog)));
        }
    }

    public void Update(GameTime gameTime) {
        HandleDebug();

        if (Core.Input.CheckInput(Keybinds.Back, Keybinds.Menu)) {
            Core.NavPath.Remove();
        }

        // todo
    }

    public void Draw(GameTime gameTime) => Core.StageBattle.Draw(gameTime);

    public string GetInputPrompt() => IState.GetInputPromptString(ScrollUpDown, Top, Bottom, BackLog);

    #endregion

    #region Static

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

    private static void CreateLog() => Core.NavPath.Add(Core.MenuLog);

    private static void UpdateLog() => BattleLog.Text = FormatLog();// todo full log

    private static string FormatLog() =>
        /* todo
int lines = 8;
int scroll = 0;

if(Core.NavPath.Peek() == MenuType.Log) {
lines = 48;
scroll = logScroll;
}

int start = Math.Max(0, LogText.Count - lines - scroll);
int end = Math.Min(start + lines, LogText.Count);*/
        string.Join("\n", LogText);

    #endregion

}
