using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace API.Input;

public class InputManager {
    /// <summary>
    /// Gets the state information of keyboard input.
    /// todo private?
    /// </summary>
    internal KeyboardState KeyboardState { get; private set; }

    /// <summary>
    /// Gets the state information of mouse input.
    /// todo use
    /// </summary>
    private MouseState MouseState { get; set; }

    /// <summary>
    /// Gets the state information of a gamepad.
    /// </summary>
    private GamePadState GamePadState { get; set; }

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

    /// <summary>
    /// Creates a new InputManager.
    /// </summary>
    public InputManager() {
        this.KeyboardState = new KeyboardState();
        this.MouseState = new MouseState();
        this.GamePadState = new GamePadState(); // todo
        /*for (int i = 0; i < 4; i++)
        {
            GamePads[i] = new GamePadState((PlayerIndex)i);
        }*/
    }

    /// <summary>
    /// Updates the state information for the keyboard, mouse, and gamepad inputs.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public void Update(GameTime gameTime) {
        this.KeyboardState = Keyboard.GetState();
        this.MouseState = Mouse.GetState();
        this.GamePadState = GamePad.GetState(PlayerIndex.One);

        this._elapsedTime = gameTime.ElapsedGameTime;
    }

    public bool CheckInput(bool allowHold, TimeSpan holdDelay, params Keybind[] keybinds) {
        foreach (Keybind keybind in keybinds) {
            if (this.IsKeybindPressed(allowHold, holdDelay, keybind)) {
                return true;
            }
        }

        return false;
    }

    public bool CheckInput(bool allowHold, params Keybind[] keybinds) {
        return this.CheckInput(allowHold, this._defaultHoldDelay, keybinds);
    }

    public bool CheckInput(params Keybind[] keybinds) {
        return this.CheckInput(false, this._defaultHoldDelay, keybinds);
    }

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
        return this.KeyboardState.IsKeyDown(keybind.Key) || this.IsButtonDown(keybind.Button);
    }

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