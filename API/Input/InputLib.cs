using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace API.Input;

public static class InputLib {

    #region Fields

    // todo private?
    private static KeyboardState _PreviousKeyboardState { get; set; }
    internal static KeyboardState _KeyboardState { get; private set; }

    private static GamePadState _GamePadState { get; set; } // todo

    private static InputDevice _PreviousInputSource { get; set; } = InputDevice.Keyboard;
    public static InputDevice LastInputSource { get; private set; } = InputDevice.Keyboard;
    public static bool InputDeviceChanged { get; private set; } = true;

    /// <summary>
    /// How long each <c>Keybind</c> has been held down for
    /// </summary>
    private static readonly TimeSpan[] _Held = new TimeSpan[Keybinds.KeybindCount];

    /// <summary>
    /// Default time between triggers when holding <c>Keybind</c> down, in seconds
    /// </summary>
    private const float _DefaultHoldDelay = 0.15f;

    /// <summary>
    /// Time from <c>Keybind</c> first becoming held to first trigger
    /// </summary>
    private static readonly TimeSpan _HoldInitDelay = TimeSpan.FromSeconds(0.3);

    /// <summary>
    /// Most recent <c>ElapsedGameTime</c>
    /// </summary>
    private static TimeSpan _elapsedTime;

    /// <summary>
    /// Distance an axis must be moved for input to register
    /// </summary>
    private const float _MinAxisDist = 0.4f;

    #endregion

    /*for (int i = 0; i < 4; i++)
        {
            GamePads[i] = new GamePadState((PlayerIndex)i);
        }*/

    public static void Update(GameTime gameTime) {
        _PreviousKeyboardState = _KeyboardState;
        _KeyboardState = Keyboard.GetState();
        _GamePadState = GamePad.GetState(PlayerIndex.One);

        _elapsedTime = gameTime.ElapsedGameTime;

        if (_PreviousInputSource != LastInputSource) {
            _PreviousInputSource = LastInputSource;
            InputDeviceChanged = true;
            return;
        }

        InputDeviceChanged = false;

    }

    /// <summary>
    /// Doesn't account for remapping. Prefer <c>Check()</c>
    /// </summary>
    /// <returns>
    /// Whether a <c>Keys</c> was pressed this frame
    /// </returns>
    public static bool IsKeyPressed(Keys key) => _KeyboardState.IsKeyDown(key);

    /// <summary>
    /// Doesn't account for remapping. Prefer <c>Check()</c>
    /// </summary>
    /// <returns>
    /// Whether a <c>Keys</c> was pressed this frame and not the previous frame
    /// </returns>
    public static bool IsKeyJustPressed(Keys key) =>
        _KeyboardState.IsKeyDown(key) && _PreviousKeyboardState.IsKeyUp(key);

    #region CheckInput

    /// <summary>
    /// Check for input from 1 <c>Keybind</c>
    /// </summary>
    public static bool Check(Keybind keybind, bool allowHold = false, float holdDelayS = _DefaultHoldDelay) =>
        _IsKeybindPressed(allowHold, holdDelayS, keybind);

    /// <summary>
    /// Check for input from either of 2 <c>Keybind</c>s
    /// </summary>
    public static bool Check(Keybind keybind1, Keybind keybind2, bool allowHold = false, float holdDelayS = _DefaultHoldDelay) =>
        _IsKeybindPressed(allowHold, holdDelayS, keybind1) ||
        _IsKeybindPressed(allowHold, holdDelayS, keybind2);

    #endregion

    #region Internals

    private static bool _IsKeybindPressed(bool allowHold, float holdDelayS, Keybind keybind) {
        // Bypasses held time checks
        if (keybind.Id == KeybindId.Hotkey) return _CheckKeybind(keybind);

        if (!_CheckKeybind(keybind)) {
            _Held[(int) keybind.Id] = TimeSpan.Zero;
            return false;
        }

        if (_Held[(int) keybind.Id] == TimeSpan.Zero && _CheckKeybind(keybind)) {
            _Held[(int) keybind.Id] += _elapsedTime;
            return true;
        }

        if (allowHold && _Held[(int) keybind.Id] >= _HoldInitDelay && _CheckKeybind(keybind)) {
            _Held[(int) keybind.Id] = _HoldInitDelay -
                TimeSpan.FromSeconds(holdDelayS * Convert.ToInt32(_CheckKeybind(Keybinds.Hotkey)) + 1);
            return true;
        }

        _Held[(int) keybind.Id] += _elapsedTime;
        return false;
    }

    private static bool _CheckKeybind(Keybind keybind) {
        if (_IsKeyDown(keybind)) {
            LastInputSource = InputDevice.Keyboard;
            return true;
        }

        if (_IsButtonDown(keybind)) {
            LastInputSource = InputDevice.XboxController; // todo
            return true;
        }

        return false;
    }

    private static bool _IsKeyDown(Keybind keybind) => keybind.Id switch {
        KeybindId.LeftRight => _IsKeyDown(Keybinds.Left.Key) || _IsKeyDown(Keybinds.Right.Key),
        KeybindId.UpDown => _IsKeyDown(Keybinds.Up.Key) || _IsKeyDown(Keybinds.Down.Key),
        KeybindId.LeftRightUpDown => _IsKeyDown(Keybinds.Left.Key) || _IsKeyDown(Keybinds.Right.Key) ||
            _IsKeyDown(Keybinds.Up.Key) || _IsKeyDown(Keybinds.Down.Key),
        _ => _IsKeyDown(keybind.Key)
    };

    private static bool _IsKeyDown(Keys key) => key switch {
        Keys.LeftShift or Keys.RightShift => _KeyboardState.IsKeyDown(Keys.LeftShift) ||
            _KeyboardState.IsKeyDown(Keys.RightShift),
        Keys.LeftControl or Keys.RightControl => _KeyboardState.IsKeyDown(Keys.LeftControl) ||
            _KeyboardState.IsKeyDown(Keys.RightControl),
        Keys.LeftAlt or Keys.RightAlt => _KeyboardState.IsKeyDown(Keys.LeftAlt) ||
            _KeyboardState.IsKeyDown(Keys.RightAlt),
        Keys.LeftWindows or Keys.RightWindows => _KeyboardState.IsKeyDown(Keys.LeftWindows) ||
            _KeyboardState.IsKeyDown(Keys.RightWindows),
        _ => _KeyboardState.IsKeyDown(key),
    };

    private static bool _IsButtonDown(Keybind keybind) => keybind.Id switch {
        KeybindId.LeftRight => _IsButtonDown(Keybinds.Left.Button) || _IsButtonDown(Keybinds.Right.Button),
        KeybindId.UpDown => _IsButtonDown(Keybinds.Up.Button) || _IsButtonDown(Keybinds.Down.Button),
        KeybindId.LeftRightUpDown => _IsButtonDown(Keybinds.Left.Button) || _IsButtonDown(Keybinds.Right.Button) ||
            _IsButtonDown(Keybinds.Up.Button) || _IsButtonDown(Keybinds.Down.Button),
        _ => _IsButtonDown(keybind.Button)
    };

    private static bool _IsButtonDown(Buttons button) => button switch {
        Buttons.DPadLeft => _GamePadState.ThumbSticks.Left.X < -_MinAxisDist,
        Buttons.DPadRight => _GamePadState.ThumbSticks.Left.X > _MinAxisDist,
        Buttons.DPadUp => _GamePadState.ThumbSticks.Left.Y < -_MinAxisDist,
        Buttons.DPadDown => _GamePadState.ThumbSticks.Left.Y > _MinAxisDist,
        Buttons.LeftTrigger => _GamePadState.Triggers.Left > _MinAxisDist,
        Buttons.RightTrigger => _GamePadState.Triggers.Right > _MinAxisDist,
        _ => _GamePadState.IsButtonDown(button)
    };

    #endregion
}