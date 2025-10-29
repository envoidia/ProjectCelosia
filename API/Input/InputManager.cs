using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace API.Input;

public class InputManager {
    // todo private?
    internal KeyboardState KeyboardState { get; private set; }

    // todo use
    private MouseState MouseState { get; set; }

    private GamePadState GamePadState { get; set; } // todo

    private InputDevice PreviousInputSource { get; set; } = InputDevice.Keyboard;
    public InputDevice LastInputSource { get; private set; } = InputDevice.Keyboard;
    public bool InputDeviceChanged { get; private set; } = true;

    /// <summary>
    /// How long each keybind has been held down for
    /// </summary>
    private readonly TimeSpan[] _held = new TimeSpan[13];

    /// <summary>
    /// Default time between triggers when holding keybind down
    /// </summary>
    private readonly TimeSpan _defaultHoldDelay = TimeSpan.FromSeconds(0.1);

    /// <summary>
    /// Time from keybind first becoming held to first trigger
    /// </summary>
    private readonly TimeSpan _holdInitDelay = TimeSpan.FromSeconds(0.3);

    /// <summary>
    /// Most recent ElapsedGameTime
    /// </summary>
    private TimeSpan _elapsedTime;

    /// <summary>
    /// Distance an axis must be moved for input to register
    /// </summary>
    private const float MinAxisDist = 0.4f;

    /*for (int i = 0; i < 4; i++)
        {
            GamePads[i] = new GamePadState((PlayerIndex)i);
        }*/

    public void Update(GameTime gameTime) {
        this.KeyboardState = Keyboard.GetState();
        this.MouseState = Mouse.GetState();
        this.GamePadState = GamePad.GetState(PlayerIndex.One);

        this._elapsedTime = gameTime.ElapsedGameTime;

        if (this.PreviousInputSource != this.LastInputSource) {
            this.PreviousInputSource = this.LastInputSource;
            this.InputDeviceChanged = true;
        } else {
            this.InputDeviceChanged = false;
        }
    }

    /// <summary>
    /// Call to check for inputs from any number of Keybinds
    /// </summary>
    public bool CheckInput(bool allowHold, TimeSpan holdDelay, params Keybind[] keybinds) {
        foreach (Keybind keybind in keybinds) {
            if (this.IsKeybindPressed(allowHold, holdDelay, keybind)) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Call to check for inputs from any number of Keybinds
    /// </summary>
    public bool CheckInput(bool allowHold, params Keybind[] keybinds) =>
        this.CheckInput(allowHold, this._defaultHoldDelay, keybinds);

    /// <summary>
    /// Call to check for inputs from any number of Keybinds
    /// </summary>
    public bool CheckInput(params Keybind[] keybinds) => this.CheckInput(false, this._defaultHoldDelay, keybinds);

    private bool IsKeybindPressed(bool allowHold, TimeSpan holdDelay, Keybind keybind) {
        if (!this.CheckKeybind(keybind)) {
            this._held[(int) keybind.Id] = TimeSpan.Zero;
            return false;
        }

        if ((this._held[(int) keybind.Id] == TimeSpan.Zero) && this.CheckKeybind(keybind)) {
            this._held[(int) keybind.Id] += this._elapsedTime;
            return true;
        }

        if (allowHold && (this._held[(int) keybind.Id] >= this._holdInitDelay) && this.CheckKeybind(keybind)) {
            this._held[(int) keybind.Id] = this._holdInitDelay - holdDelay;
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
        KeybindId.LeftRight => this.KeyboardState.IsKeyDown(Keybind.Left.Key) ||
                               this.KeyboardState.IsKeyDown(Keybind.Right.Key),
        KeybindId.UpDown => this.KeyboardState.IsKeyDown(Keybind.Up.Key) ||
                            this.KeyboardState.IsKeyDown(Keybind.Down.Key),
        KeybindId.LeftRightUpDown => this.KeyboardState.IsKeyDown(Keybind.Left.Key) ||
                                     this.KeyboardState.IsKeyDown(Keybind.Right.Key) ||
                                     this.KeyboardState.IsKeyDown(Keybind.Up.Key) ||
                                     this.KeyboardState.IsKeyDown(Keybind.Down.Key),
        _ => this.KeyboardState.IsKeyDown(keybind.Key)
    };

    private bool IsButtonDown(Keybind keybind) => keybind.Id switch {
        KeybindId.LeftRight => this.IsButtonDown(Keybind.Left.Button) || this.IsButtonDown(Keybind.Right.Button),
        KeybindId.UpDown => this.IsButtonDown(Keybind.Up.Button) || this.IsButtonDown(Keybind.Down.Button),
        KeybindId.LeftRightUpDown => this.IsButtonDown(Keybind.Left.Button) ||
                                     this.IsButtonDown(Keybind.Right.Button) ||
                                     this.IsButtonDown(Keybind.Up.Button) || this.IsButtonDown(Keybind.Down.Button),
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
}