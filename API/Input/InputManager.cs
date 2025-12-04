using System;
using API.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace API.Input;

public sealed class InputManager {

    #region Fields

    // todo private?
    public KeyboardState PreviousKeyboardState { get; private set; }
    public KeyboardState KeyboardState { get; private set; }

    private GamePadState GamePadState { get; set; } // todo

    private InputDevice PreviousInputSource { get; set; } = InputDevice.Keyboard;
    public InputDevice LastInputSource { get; private set; } = InputDevice.Keyboard;
    public bool InputDeviceChanged { get; private set; } = true;

    /// <summary>
    /// How long each <c>Keybind</c> has been held down for
    /// </summary>
    private readonly TimeSpan[] _held = new TimeSpan[Keybinds.KeybindCount];

    /// <summary>
    /// Default time between triggers when holding <c>Keybind</c> down, in seconds
    /// </summary>
    private const float DefaultHoldDelay = 0.15f;

    /// <summary>
    /// Time from <c>Keybind</c> first becoming held to first trigger
    /// </summary>
    private readonly TimeSpan _holdInitDelay = TimeSpan.FromSeconds(0.3);

    /// <summary>
    /// Most recent <c>ElapsedGameTime</c>
    /// </summary>
    private TimeSpan _elapsedTime;

    /// <summary>
    /// Distance an axis must be moved for input to register
    /// </summary>
    private const float MinAxisDist = 0.4f;

    #endregion

    /*for (int i = 0; i < 4; i++)
        {
            GamePads[i] = new GamePadState((PlayerIndex)i);
        }*/

    public void Update(GameTime gameTime) {
        this.PreviousKeyboardState = this.KeyboardState;
        this.KeyboardState = Keyboard.GetState();
        this.GamePadState = GamePad.GetState(PlayerIndex.One);

        this._elapsedTime = gameTime.ElapsedGameTime;

        if (this.PreviousInputSource != this.LastInputSource) {
            this.PreviousInputSource = this.LastInputSource;
            this.InputDeviceChanged = true;
            return;
        }

        this.InputDeviceChanged = false;

    }

    /// <returns>
    /// Whether a <c>Keys</c> was pressed this frame and not the previous frame
    /// </returns>
    public bool IsKeyPressed(Keys key) => this.KeyboardState.IsKeyDown(key);

    /// <returns>
    /// Whether a <c>Keys</c> was pressed this frame and not the previous frame
    /// </returns>
    public bool IsKeyJustPressed(Keys key) =>
        this.KeyboardState.IsKeyDown(key) && this.PreviousKeyboardState.IsKeyUp(key);


    // Called multiple times per frame, so avoid params. Add more overloads if needed. Could add a params one at the end too
    #region CheckInput

    /// <summary>
    /// Check for inputs from 1 <c>Keybind</c>
    /// </summary>
    public bool CheckInput(Keybind keybind, bool allowHold = false, float holdDelayS = DefaultHoldDelay) =>
        this.IsKeybindPressed(allowHold, holdDelayS, keybind);

    /// <summary>
    /// Check for inputs from 2 <c>Keybind</c>s
    /// </summary>
    public bool CheckInput(Keybind keybind1, Keybind keybind2, bool allowHold = false, float holdDelayS = DefaultHoldDelay) =>
        this.IsKeybindPressed(allowHold, holdDelayS, keybind1) || this.IsKeybindPressed(allowHold, holdDelayS, keybind2);

    #endregion

    #region Internals

    private bool IsKeybindPressed(bool allowHold, float holdDelayS, Keybind keybind) {
        if (!this.CheckKeybind(keybind)) {
            this._held[(int) keybind.Id] = TimeSpan.Zero;
            return false;
        }

        if (this._held[(int) keybind.Id] == TimeSpan.Zero && this.CheckKeybind(keybind)) {
            this._held[(int) keybind.Id] += this._elapsedTime;
            return true;
        }

        if (allowHold && this._held[(int) keybind.Id] >= this._holdInitDelay && this.CheckKeybind(keybind)) {
            this._held[(int) keybind.Id] = this._holdInitDelay -
                TimeSpan.FromSeconds(holdDelayS * (this.CheckKeybind(Keybinds.ScrollFaster).ToInt() + 1));
            return true;
        }

        this._held[(int) keybind.Id] += this._elapsedTime;
        return false;
    }

    private bool CheckKeybind(Keybind keybind) {
        if (this.IsKeyDown(keybind)) {
            this.LastInputSource = InputDevice.Keyboard;
            return true;
        }

        if (this.IsButtonDown(keybind)) {
            this.LastInputSource = InputDevice.XboxController; // todo
            return true;
        }

        return false;
    }

    private bool IsKeyDown(Keybind keybind) => keybind.Id switch {
        KeybindId.LeftRight => this.KeyboardState.IsKeyDown(Keybinds.Left.Key) ||
                               this.KeyboardState.IsKeyDown(Keybinds.Right.Key),
        KeybindId.UpDown => this.KeyboardState.IsKeyDown(Keybinds.Up.Key) ||
                            this.KeyboardState.IsKeyDown(Keybinds.Down.Key),
        KeybindId.LeftRightUpDown => this.KeyboardState.IsKeyDown(Keybinds.Left.Key) ||
                                     this.KeyboardState.IsKeyDown(Keybinds.Right.Key) ||
                                     this.KeyboardState.IsKeyDown(Keybinds.Up.Key) ||
                                     this.KeyboardState.IsKeyDown(Keybinds.Down.Key),
        _ => this.KeyboardState.IsKeyDown(keybind.Key)
    };

    private bool IsButtonDown(Keybind keybind) => keybind.Id switch {
        KeybindId.LeftRight => this.IsButtonDown(Keybinds.Left.Button) || this.IsButtonDown(Keybinds.Right.Button),
        KeybindId.UpDown => this.IsButtonDown(Keybinds.Up.Button) || this.IsButtonDown(Keybinds.Down.Button),
        KeybindId.LeftRightUpDown => this.IsButtonDown(Keybinds.Left.Button) ||
                                     this.IsButtonDown(Keybinds.Right.Button) ||
                                     this.IsButtonDown(Keybinds.Up.Button) || this.IsButtonDown(Keybinds.Down.Button),
        _ => this.IsButtonDown(keybind.Button)
    };

    private bool IsButtonDown(Buttons button) => button switch {
        Buttons.DPadLeft => this.GamePadState.ThumbSticks.Left.X < -MinAxisDist,
        Buttons.DPadRight => this.GamePadState.ThumbSticks.Left.X > MinAxisDist,
        Buttons.DPadUp => this.GamePadState.ThumbSticks.Left.Y < -MinAxisDist,
        Buttons.DPadDown => this.GamePadState.ThumbSticks.Left.Y > MinAxisDist,
        Buttons.LeftTrigger => this.GamePadState.Triggers.Left > MinAxisDist,
        Buttons.RightTrigger => this.GamePadState.Triggers.Right > MinAxisDist,
        _ => this.GamePadState.IsButtonDown(button)
    };

    #endregion
}