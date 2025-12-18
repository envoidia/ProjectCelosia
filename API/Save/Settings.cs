using API.Graphics;

namespace API.Save;

public static class Settings {
    #region Visual

    public static Theme Theme {
        get;
        set {
            Theme._Change(field, value);
            field = value;
        }
    } = Theme.Apollo;

    static Settings() {
        // Set up default theme
        Theme._ChangeFSSColors(Theme);
    }

    #endregion

    #region Battle
    // Speed of battle animations
    // Duration of in-battle pauses relative to 100% (1 = 100%, 0.1 = 10%)
    public static float BattleSpeed { get; } = 1f;

    #endregion

    #region Debug

    public static bool EnableDebugFeatures { get; set; } =
#if DEBUG
        true
#else 
        false
#endif
        ;

    public static bool SelectOpponentMoves { get; } = false;

    #endregion
}