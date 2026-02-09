using System;
using System.Text;
using API.Debug;
using API.Graphics;
using API.Save;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace API.Input;

public static class InputLib
{
    #region Fields

    // todo docs
    public static KeyboardState PreviousKeyboardState { get; private set; }
    public static KeyboardState KeyboardState { get; private set; }

    private static GamePadState _gamePadState; // todo

    private static InputDevice _previousInputSource = InputDevice.Keyboard;
    public static InputDevice LastInputSource { get; private set; } = InputDevice.Keyboard;

    /// <summary>
    /// Notified when the current <c>InputDevice</c> changes
    /// </summary>
    public static event Action? OnDeviceChange;

    /// <summary>
    /// How long each standard <c>Keybind</c> has been held down for
    /// </summary>
    private static readonly TimeSpan[] _Held = new TimeSpan[Keybinds.StdKeybindCount];

    /// <summary>
    /// Default time between triggers when holding <c>Keybind</c> down, in seconds
    /// </summary>
    private const float _DefaultHoldDelayS = 0.25f;

    /// <summary>
    /// Time from <c>Keybind</c> first becoming held to first trigger
    /// </summary>
    private const float _HoldInitDelayS = 0.3f;

    /// <inheritdoc cref="_HoldInitDelayS" />
    private static readonly TimeSpan _HoldInitDelay = TimeSpan.FromSeconds(_HoldInitDelayS);

    /// <summary>
    /// Tracks the status of input
    /// </summary>
    internal static readonly Routine _TrackInput = new(a => Assert.Is<Label>(a),
        static (a, gt) =>
        {
            Label l = (Label) a;

            StringBuilder sb = new();
            for (int i = 0; i < _Held.Length; i++)
            {
                TimeSpan held = _Held[i];
                double s = held.TotalSeconds;

                Color c = _CheckKeybind(Keybinds.UniqueKeybinds[i])
                    ? Settings.Theme.Cooldown : Settings.Theme.White;

                sb.Append(_CheckKeybind(Keybinds.UniqueKeybinds[i])
                    ? ThemeColor.Cooldown.Str : ThemeColor.White.Str)
                    .Append(s.ToString("0.##")).Append('\n');
            }

            bool check = _CheckKeybind(Keybinds.Hotkey1);
            sb.Append(check ? ThemeColor.Pos.Str : ThemeColor.Neg.Str).Append(check).Append('\n');

            check = _CheckKeybind(Keybinds.Hotkey2);
            sb.Append(check ? ThemeColor.Pos.Str : ThemeColor.Neg.Str).Append(check);

            l.Text = sb.ToString();

            return false;
        });

    /// <summary>
    /// Most recent <c>ElapsedGameTime</c>
    /// </summary>
    private static TimeSpan _elapsedTime;

    /// <summary>
    /// Distance an axis must be moved for input to register
    /// </summary>
    private const float _MinAxisDist = 0.4f;

    #endregion

    /*todo multi controllers for (int i = 0; i < 4; i++)
        {
            GamePads[i] = new GamePadState((PlayerIndex)i);
        }*/

    public static void Update(GameTime gt)
    {
        PreviousKeyboardState = KeyboardState;
        KeyboardState = Keyboard.GetState();
        _gamePadState = GamePad.GetState(PlayerIndex.One);

        _elapsedTime = gt.ElapsedGameTime;

        if (_previousInputSource != LastInputSource)
        {
            OnDeviceChange?.Invoke();
            _previousInputSource = LastInputSource;
            return;
        }

    }

    /// <summary>
    /// Doesn't account for remapping or buttons. Prefer <c>Check()</c>
    /// </summary>
    /// <returns>
    /// Whether a <c>Keys</c> was pressed this frame
    /// </returns>
    public static bool IsKeyPressed(Keys key)
    {
        return KeyboardState.IsKeyDown(key);
    }

    /// <summary>
    /// Doesn't account for remapping or buttons. Prefer <c>Check()</c>
    /// </summary>
    /// <returns>
    /// Whether a <c>Keys</c> was pressed this frame and not the previous frame
    /// </returns>
    // todo
    public static bool IsKeyJustPressed(Keys key)
    {
        return KeyboardState.IsKeyDown(key) && PreviousKeyboardState.IsKeyUp(key);
    }

    #region Check

    /// <summary>
    /// Check for input from 1 <c>Keybind</c>
    /// </summary>
    public static bool Check(Keybind? keybind, bool allowHold = false, float holdDelayS = _DefaultHoldDelayS)
    {
        return _IsKeybindPressed(allowHold, holdDelayS, keybind);
    }

    /// <summary>
    /// Check for input from either of 2 <c>Keybind</c>s
    /// </summary>
    public static bool Check(Keybind keybind1, Keybind keybind2, bool allowHold = false, float holdDelayS = _DefaultHoldDelayS)
    {
        return _IsKeybindPressed(allowHold, holdDelayS, keybind1) ||
        _IsKeybindPressed(allowHold, holdDelayS, keybind2);
    }

    #endregion

    #region Internals

    private static bool _IsKeybindPressed(bool allowHold, float holdDelayS, Keybind? keybind)
    {
        if (keybind is null)
        {
            return false;
        }

        // Bypasses held time checks
        if (keybind.Id is KeybindId.Hotkey1 or KeybindId.Hotkey2)
        {
            return _CheckKeybind(keybind);
        }

        // Merged keybinds
        if (keybind == Keybinds.LeftUp)
        {
            return _IsKeybindPressed(allowHold, holdDelayS, Keybinds.Left) ||
                _IsKeybindPressed(allowHold, holdDelayS, Keybinds.Up);
        }

        if (keybind == Keybinds.RightDown)
        {
            return _IsKeybindPressed(allowHold, holdDelayS, Keybinds.Right) ||
                _IsKeybindPressed(allowHold, holdDelayS, Keybinds.Down);
        }

        // Normal keybinds
        // Not held
        if (!_CheckKeybind(keybind))
        {
            _Held[(int) keybind.Id] = TimeSpan.Zero;
            return false;
        }

        // Held for 0t
        if (_Held[(int) keybind.Id] == TimeSpan.Zero && _CheckKeybind(keybind))
        {
            _Held[(int) keybind.Id] += _elapsedTime;
            return true;
        }

        // Held for long enough to re-tick
        if (allowHold && _Held[(int) keybind.Id] >= _HoldInitDelay && _CheckKeybind(keybind))
        {
            _Held[(int) keybind.Id] = _HoldInitDelay - (_CheckKeybind(Keybinds.Hotkey1)
                ? TimeSpan.FromSeconds(holdDelayS / 2)
                : TimeSpan.FromSeconds(holdDelayS));
            return true;
        }

        // Not long enough yet
        _Held[(int) keybind.Id] += _elapsedTime;
        return false;
    }

    private static bool _CheckKeybind(Keybind keybind)
    {
        if (_IsKeyDown(keybind.Key))
        {
            LastInputSource = InputDevice.Keyboard;
            return true;
        }

        if (_IsButtonDown(keybind.Button))
        {
            LastInputSource = InputDevice.XboxController; // todo controller types
            return true;
        }

        return false;
    }

    private static bool _IsKeyDown(Keys key)
    {
        return key switch
        {
            Keys.LeftShift or Keys.RightShift => KeyboardState.IsKeyDown(Keys.LeftShift) ||
                KeyboardState.IsKeyDown(Keys.RightShift),
            Keys.LeftControl or Keys.RightControl => KeyboardState.IsKeyDown(Keys.LeftControl) ||
                KeyboardState.IsKeyDown(Keys.RightControl),
            Keys.LeftAlt or Keys.RightAlt => KeyboardState.IsKeyDown(Keys.LeftAlt) ||
                KeyboardState.IsKeyDown(Keys.RightAlt),
            Keys.LeftWindows or Keys.RightWindows => KeyboardState.IsKeyDown(Keys.LeftWindows) ||
                KeyboardState.IsKeyDown(Keys.RightWindows),
            _ => KeyboardState.IsKeyDown(key),
        };
    }

    private static bool _IsButtonDown(Buttons button)
    {
        return button switch
        {
            Buttons.DPadLeft => _gamePadState.ThumbSticks.Left.X < -_MinAxisDist,
            Buttons.DPadRight => _gamePadState.ThumbSticks.Left.X > _MinAxisDist,
            Buttons.DPadUp => _gamePadState.ThumbSticks.Left.Y < -_MinAxisDist,
            Buttons.DPadDown => _gamePadState.ThumbSticks.Left.Y > _MinAxisDist,
            Buttons.LeftTrigger => _gamePadState.Triggers.Left > _MinAxisDist,
            Buttons.RightTrigger => _gamePadState.Triggers.Right > _MinAxisDist,
            _ => _gamePadState.IsButtonDown(button)
        };
    }

    #endregion
}