using Microsoft.Xna.Framework.Input;

namespace API.Input;

public sealed class Keybind {
    public static readonly Keybind Confirm = new("Confirm", KeybindId.Confirm, Keys.Z, Buttons.A); // (Bottom)
    public static readonly Keybind Back = new("Back", KeybindId.Back, Keys.X, Buttons.B); // (Right)
    public static readonly Keybind Menu = new("Menu", KeybindId.Menu, Keys.C, Buttons.Y); // (Top) Also Open Full Log
    public static readonly Keybind Map = new("key.map", KeybindId.Map, Keys.V, Buttons.X); // (Left) Also Inspect
    public static readonly Keybind PageL1 = new("key.page_l1", KeybindId.PageL1, Keys.F, Buttons.LeftShoulder);
    public static readonly Keybind PageR1 = new("key.page_r1", KeybindId.PageR1, Keys.G, Buttons.RightShoulder);
    public static readonly Keybind PageL2 = new("key.page_l2", KeybindId.PageL2, Keys.S, Buttons.LeftTrigger);
    public static readonly Keybind PageR2 = new("key.page_r2", KeybindId.PageR2, Keys.D, Buttons.RightTrigger);
    public static readonly Keybind Left = new("key.left", KeybindId.Left, Keys.Left, Buttons.DPadLeft);
    public static readonly Keybind Right = new("key.right", KeybindId.Right, Keys.Right, Buttons.DPadRight);
    public static readonly Keybind Up = new("key.up", KeybindId.Up, Keys.Up, Buttons.DPadUp);
    public static readonly Keybind Down = new("key.down", KeybindId.Down, Keys.Down, Buttons.DPadDown);

    // Debug
    public static readonly Keybind DebugInfo = new("keybind.debug", KeybindId.DebugInfo, Keys.F1, Buttons.BigButton);

    public string Name { get; }
    public KeybindId Id { get; }
    public Keys Key { get; set; }
    public Buttons Button { get; set; }

    private Keybind(string name, KeybindId id, Keys key, Buttons button) {
        this.Name = name;
        this.Id = id;
        this.Key = key;
        this.Button = button;
    }
}