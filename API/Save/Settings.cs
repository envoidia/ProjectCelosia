using API.Graphics;
using Microsoft.Xna.Framework;

namespace API.Save;

public static class Settings {
    #region Main Settings
    // Speed of battle animations
    // Duration of in-battle pauses relative to 100% (1 = 100%, 0.1 = 10%)
    public static float BattleSpeed { get; } = 1f;

    #endregion

    #region Hidden Settings

    public static Color ColorBg { get; } = Colors.Bg;
    public static Color ColorFg { get; } = Colors.Fg;
    public static Color ColorAccent { get; } = Colors.Accent;

    #endregion

    #region Debug

    public static bool SelectOpponentMoves { get; } = false;

    #endregion
}