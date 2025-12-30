using System;
using System.Linq;
using System.Text;
using API.Graphics;
using API.Input;
using Microsoft.Xna.Framework.Input;

namespace API.Menu;

/// <summary>
/// Handles single-line text input. Does not add the given label to the <c>Stage</c>
/// </summary>
public sealed class TextInput(string dbgName, Label label, Func<bool> onEnter) : Menu(dbgName) {
    /// <summary>
    /// Current text as a <c>StringBuilder</c>
    /// </summary>
    public StringBuilder Sb { get; } = new();

    /// <summary>
    /// Current text as a <c>string</c>
    /// </summary>
    public string Text => this.Sb.ToString();

    /// <summary>
    /// Text display
    /// </summary>
    public Label Label => label;

    /// <summary>
    /// Current cursor position. 0 = very left
    /// </summary>
    public int CursorPos { get; set; } = 0;

    /// <summary>
    /// Invoked when enter is pressed. If it returns true, text is wiped
    /// </summary>
    public Func<bool> OnEnter => onEnter;

    /// <summary>
    /// Invoked when the text is changed
    /// </summary>
    public Action? OnChangeText { get; init; }

    /// <summary>
    /// Whether the text has changed this frame
    /// </summary>
    public bool Changed { get; set; }

    public void Input() {
        Keys[] keys = [.. InputLib.KeyboardState.GetPressedKeys()
            .Except(InputLib.PreviousKeyboardState.GetPressedKeys())];

        if (keys.Length == 0) return;

        foreach (Keys key in keys) {
            switch (key) {
                case Keys.Enter:
                    if (this.Sb.Length > 0) {
                        if (this.OnEnter()) this.Clear();
                        return;
                    }

                    break;


                case Keys.Back:
                    if (this.CursorPos > 0) {
                        this.Changed = true;
                        this.Sb.Remove(this.CursorPos - 1, 1);
                        this.CursorPos--;
                    }

                    break;

                default:
                    char? ch = key.Type();
                    if (ch is not null) {
                        this.Changed = true;
                        this.Sb.Insert(this.CursorPos, (char) ch);
                        this.CursorPos++;
                    }

                    break;
            }
        }

        if (this.Changed) this.OnChangeText?.Invoke();
    }

    public void Clear() {
        this.Changed = true;
        this.Sb.Clear();
        this.OnChangeText?.Invoke();
        this.CursorPos = 0;
    }
}
