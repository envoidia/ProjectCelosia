using System;
using System.Runtime.CompilerServices;
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

    public static KeyboardState PreviousKeyboardState { get; private set; }
    public static KeyboardState KeyboardState { get; private set; }

    private static GamePadState _gamePadState;

    private static MouseState _previousMouseState;
    private static MouseState _mouseState;

    /// <summary>
    /// Amount of reported scroll per tick on the mouse wheel
    /// </summary>
    public const int ScrollPerMouseWheelTick = 120;

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

    private static readonly StringBuilder _InputSb = new(150);
    /// <summary>
    /// Tracks the status of input
    /// </summary>
    internal static readonly Routine _TrackInput = new(a => Assert.Is<Label>(a),
        static (a, gt) =>
        {
            Label l = (Label) a;

            _InputSb.Clear();
            for (int i = 0; i < _Held.Length; i++)
            {
                TimeSpan held = _Held[i];
                double s = held.TotalSeconds;

                Color c = CheckRaw(Keybinds.NonMergedKeybinds[i])
                    ? Settings.Theme.Cooldown : Settings.Theme.Fg;

                if ((KeybindId) i is KeybindId.Hotkey1 or KeybindId.Hotkey2)
                {
                    bool isHeld = CheckRaw(Keybinds.NonMergedKeybinds[i]);
                    appendBoolKey(isHeld);

                    continue;
                }

                _InputSb.Append($"{(CheckRaw(Keybinds.NonMergedKeybinds[i])
                    ? ThemeColor.Cooldown.Str : ThemeColor.Fg.Str)}{s.ToString("0.##")}\n");
            }

            Point pos = GetMousePos().ToPoint();
            _InputSb.Append($"{pos.X}, {pos.Y}\n");

            appendBoolKey(IsMouseLeftPressed());
            appendBoolKey(IsMouseRightPressed());

            int scroll = GetMouseScroll();
            _InputSb.Append($"{(scroll > 0 ? ThemeColor.Positive.Str : ThemeColor.Negative.Str)}{scroll}\n");

            appendBoolKey(IsMouseMiddlePressed());
            appendBoolKey(IsMouseX1Pressed());
            appendBoolKey(IsMouseX2Pressed(), null);

            l.Text = _InputSb.ToString();
            return false;

            static void appendBoolKey(bool isHeld, char? ending = '\n')
            {
                _InputSb.Append($"{(isHeld ? ThemeColor.Positive.Str : ThemeColor.Negative.Str)}{(isHeld
                    ? "true" : "false")}{ending}");
            }
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

        _previousMouseState = _mouseState;
        _mouseState = Mouse.GetState();

        // todo check all
        // _gamePadState = GamePad.GetState(PlayerIndex.One);
        _gamePadState = _GetMergedGamePadState();

        _elapsedTime = gt.ElapsedGameTime;

        if (_previousInputSource != LastInputSource)
        {
            OnDeviceChange?.Invoke();
            _previousInputSource = LastInputSource;
            return;
        }
    }

    #region Input Checks

    /// <summary>
    /// Doesn't account for remapping or buttons. Prefer <c>Check</c>
    /// </summary>
    /// <returns>
    /// Whether a <c>Keys</c> was pressed this frame
    /// </returns>
    public static bool IsKeyPressed(Keys key)
    {
        return KeyboardState.IsKeyDown(key);
    }

    /// <summary>
    /// Doesn't account for remapping or buttons. Prefer <c>Check</c>
    /// </summary>
    /// <returns>
    /// Whether a <c>Keys</c> was pressed this frame and not the previous frame
    /// </returns>
    public static bool IsKeyJustPressed(Keys key)
    {
        return KeyboardState.IsKeyDown(key) && PreviousKeyboardState.IsKeyUp(key);
    }

    /// <returns>
    /// Whether either shift key is pressed
    /// </returns>
    public static bool IsShiftPressed()
    {
        return IsKeyPressed(Keys.LeftShift) || IsKeyPressed(Keys.RightShift);
    }

    /// <returns>
    /// Whether either ctrl key is pressed
    /// </returns>
    public static bool IsCtrlPressed()
    {
        return IsKeyPressed(Keys.LeftControl) || IsKeyPressed(Keys.RightControl);
    }

    /// <returns>
    /// Whether either alt key is pressed
    /// </returns>
    public static bool IsAltPressed()
    {
        return IsKeyPressed(Keys.LeftAlt) || IsKeyPressed(Keys.RightAlt);
    }

    /// <returns>
    /// Whether the mouse left button is pressed
    /// </returns>
    public static bool IsMouseLeftPressed()
    {
        return _mouseState.LeftButton == ButtonState.Pressed;
    }

    /// <returns>
    /// Whether the mouse left button is pressed this frame and wasn't the previous frame
    /// </returns>
    public static bool IsMouseLeftJustPressed()
    {
        return _mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;
    }

    /// <returns>
    /// Whether the mouse right button is pressed
    /// </returns>
    public static bool IsMouseRightPressed()
    {
        return _mouseState.RightButton == ButtonState.Pressed;
    }

    /// <returns>
    /// Whether the mouse right button is pressed this frame and wasn't the previous frame
    /// </returns>
    public static bool IsMouseRightJustPressed()
    {
        return _mouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released;
    }

    /// <returns>
    /// Whether the mouse middle button is pressed
    /// </returns>
    public static bool IsMouseMiddlePressed()
    {
        return _mouseState.MiddleButton == ButtonState.Pressed;
    }

    /// <returns>
    /// Whether the mouse middle button is pressed this frame and wasn't the previous frame
    /// </returns>
    public static bool IsMouseMiddleJustPressed()
    {
        return _mouseState.MiddleButton == ButtonState.Pressed && _previousMouseState.MiddleButton == ButtonState.Released;
    }

    /// <returns>
    /// Whether the mouse X1 button is pressed
    /// </returns>
    public static bool IsMouseX1Pressed()
    {
        return _mouseState.XButton1 == ButtonState.Pressed;
    }

    /// <returns>
    /// Whether the mouse X1 button is pressed this frame and wasn't the previous frame
    /// </returns>
    public static bool IsMouseX1JustPressed()
    {
        return _mouseState.XButton1 == ButtonState.Pressed && _previousMouseState.XButton1 == ButtonState.Released;
    }

    /// <returns>
    /// Whether the mouse X1 button is pressed
    /// </returns>
    public static bool IsMouseX2Pressed()
    {
        return _mouseState.XButton2 == ButtonState.Pressed;
    }

    /// <returns>
    /// Whether the mouse X1 button is pressed this frame and wasn't the previous frame
    /// </returns>
    public static bool IsMouseX2JustPressed()
    {
        return _mouseState.XButton2 == ButtonState.Pressed && _previousMouseState.XButton2 == ButtonState.Released;
    }

    /// <returns>
    /// Mouse position mapped to world coordinates
    /// </returns>
    public static Vector2 GetMousePos()
    {
        return _mouseState.Position.ToVector2() * (World.W / (float) World.WindowW);
    }

    /// <returns>
    /// Change in mouse scroll wheel value since last frame
    /// </returns>
    public static int GetMouseScroll()
    {
        return _mouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue;
    }

    /// <summary>
    /// Check for <c>Keybind</c> input across any keyboard or controller.
    /// Left/Right Shift/Ctrl/Alt are treated as the same
    /// </summary>
    public static bool Check(Keybind? keybind, bool allowHold = false, float holdDelayS = _DefaultHoldDelayS)
    {
        return _IsKeybindPressed(allowHold, holdDelayS, keybind);
    }

    /// <inheritdoc cref="Check(Keybind?, bool, float)" />
    public static bool Check(Keybind? keybind1, Keybind? keybind2, bool allowHold = false, float holdDelayS = _DefaultHoldDelayS)
    {
        return _IsKeybindPressed(allowHold, holdDelayS, keybind1) ||
        _IsKeybindPressed(allowHold, holdDelayS, keybind2);
    }

    /// <summary>
    /// <inheritdoc cref="Check(Keybind?, bool, float)" />
    /// <para>Does not account for held time or merged keybinds</para>
    /// </summary>
    public static bool CheckRaw(Keybind keybind)
    {
        if (_IsKeyDown(keybind.Key))
        {
            LastInputSource = InputDevice.Keyboard;
            return true;
        }

        if (_IsButtonDown(keybind.Button))
        {
            LastInputSource = _GetGamePadType();
            return true;
        }

        return false;
    }

    #endregion

    #region Internals

    private static GamePadState _GetMergedGamePadState()
    {
        Vector2 stickL = Vector2.Zero;
        Vector2 stickR = Vector2.Zero;

        float trigL = 0f;
        float trigR = 0f;

        Buttons buttons = Buttons.None;

        for (int i = 0; i < GamePad.MaximumGamePadCount; i++)
        {
            GamePadState state = GamePad.GetState((PlayerIndex) i);

            if (!state.IsConnected)
            {
                continue;
            }

            stickL += state.ThumbSticks.Left;
            stickR += state.ThumbSticks.Right;

            trigL += state.Triggers.Left;
            trigR += state.Triggers.Right;

            // tfw struct is just an existing type privately wrapped for no reason
            // u know the api is good when ur bit casting it away
            // safety: single-field struct that hasnt been updated since probably like 2006
            // todo: this will have a real public accessor when MG updates
            buttons |= Unsafe.BitCast<GamePadButtons, Buttons>(state.Buttons);

            // why are GamePadButtons and GamePadDPad separate
            // neither of these structs has an actual reason to exist it could have just been a Buttons
            // i hate this API
            GamePadDPad d = state.DPad;
            if (d.Up == ButtonState.Pressed)
            {
                buttons |= Buttons.DPadUp;
            }
            if (d.Down == ButtonState.Pressed)
            {
                buttons |= Buttons.DPadDown;
            }
            if (d.Left == ButtonState.Pressed)
            {
                buttons |= Buttons.DPadLeft;
            }
            if (d.Right == ButtonState.Pressed)
            {
                buttons |= Buttons.DPadRight;
            }
        }

        return new(
            stickL, stickR,
            trigL, trigR,
            buttons);
    }

    private static bool _IsKeybindPressed(bool allowHold, float holdDelayS, Keybind? keybind)
    {
        if (keybind is null)
        {
            return false;
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
        if (!CheckRaw(keybind))
        {
            _Held[(int) keybind.Id] = TimeSpan.Zero;
            return false;
        }

        // Held for 0t
        if (_Held[(int) keybind.Id] == TimeSpan.Zero && CheckRaw(keybind))
        {
            _Held[(int) keybind.Id] += _elapsedTime;
            return true;
        }

        // Held for long enough to re-tick
        if (allowHold && _Held[(int) keybind.Id] >= _HoldInitDelay && CheckRaw(keybind))
        {
            _Held[(int) keybind.Id] = _HoldInitDelay - (CheckRaw(Keybinds.Hotkey1)
                ? TimeSpan.FromSeconds(holdDelayS / 2)
                : TimeSpan.FromSeconds(holdDelayS));
            return true;
        }

        // Not long enough yet
        _Held[(int) keybind.Id] += _elapsedTime;
        return false;
    }

    private static bool _IsKeyDown(Keys key)
    {
        return key switch
        {
            Keys.LeftShift or Keys.RightShift => IsShiftPressed(),
            Keys.LeftControl or Keys.RightControl => IsCtrlPressed(),
            Keys.LeftAlt or Keys.RightAlt => IsAltPressed(),
            _ => KeyboardState.IsKeyDown(key),
        };
    }

    private static bool _IsButtonDown(Buttons button)
    {
        return button switch
        {
            Buttons.DPadLeft => _gamePadState.IsButtonDown(Buttons.DPadLeft)
                || _gamePadState.ThumbSticks.Left.X < -_MinAxisDist,
            Buttons.DPadRight => _gamePadState.IsButtonDown(Buttons.DPadRight)
                || _gamePadState.ThumbSticks.Left.X > _MinAxisDist,
            Buttons.DPadUp => _gamePadState.IsButtonDown(Buttons.DPadUp)
                || _gamePadState.ThumbSticks.Left.Y > _MinAxisDist,
            Buttons.DPadDown => _gamePadState.IsButtonDown(Buttons.DPadDown)
                || _gamePadState.ThumbSticks.Left.Y < -_MinAxisDist,
            Buttons.LeftTrigger => _gamePadState.Triggers.Left > _MinAxisDist,
            Buttons.RightTrigger => _gamePadState.Triggers.Right > _MinAxisDist,
            _ => _gamePadState.IsButtonDown(button)
        };
    }

    private static InputDevice _GetGamePadType()
    {
        GamePadCapabilities caps = GamePad.GetCapabilities(PlayerIndex.One);

        string name = caps.DisplayName.ToLowerInvariant();

        if (name.Contains("switch") || name.Contains("nintendo"))
        {
            return InputDevice.SwitchController;
        }

        if (name.Contains("ps") || name.Contains("playstation") || name.Contains("sony"))
        {
            return InputDevice.PlaystationController;
        }

        return InputDevice.XboxController;
    }

    #endregion
}
