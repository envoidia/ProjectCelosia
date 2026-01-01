using System;
using System.Linq;
using System.Text;
using API.Graphics;
using API.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace API.Menu;

/// <summary>
/// Handles single-line text input
/// </summary>
public sealed class TextInput : IInputWidget
{
    /// <summary>
    /// Current text as a <c>StringBuilder</c>
    /// </summary>
    public readonly StringBuilder Sb = new();

    /// <summary>
    /// Current text as a <c>string</c>
    /// </summary>
    public string Text => this.Sb.ToString();

    /// <summary>
    /// Text display
    /// </summary>
    public readonly Label Label;

    /// <summary>
    /// Current cursor position. 0 = very left
    /// </summary>
    public int CursorPos = 0;

    /// <summary>
    /// Invoked when enter is pressed. If it returns true, text is wiped
    /// </summary>
    public readonly Func<bool> OnEnter;

    /// <summary>
    /// Invoked when the text is changed
    /// </summary>
    public Action? OnChangeText;

    /// <summary>
    /// Whether the text has changed this frame
    /// </summary>
    public bool Changed;

    public bool CheckInput { get; set; }
    public int Index { get; set; }

    public int OptCount { get; }

    public Action<int>? OnChangeIndex { get; set; }

    public SelectionType PrefDir => SelectionType.TextInput;

    public SelectionType CurDir { get; set; } = SelectionType.TextInput;

    public TextInput(Label label, Func<bool> onEnter)
    {
        this.Label = label;
        this.Label.RichTextLayout.CalculateGlyphs = true;
        this.OnEnter = onEnter;
    }

    public void Input(GameTime gt)
    {
        Keys[] keys = [.. InputLib.KeyboardState.GetPressedKeys()
            .Except(InputLib.PreviousKeyboardState.GetPressedKeys())];

        if (keys.Length == 0)
        {
            return;
        }

        foreach (Keys key in keys)
        {
            switch (key)
            {
                case Keys.Enter:
                    if (this.Sb.Length > 0)
                    {
                        if (this.OnEnter())
                        {
                            this.Clear();
                        }
                        return;
                    }

                    break;


                case Keys.Back:
                    if (this.CursorPos > 0)
                    {
                        this.Changed = true;
                        this.Sb.Remove(this.CursorPos - 1, 1);
                        this.CursorPos--;
                    }

                    break;

                case Keys.Left:
                    if (this.CursorPos > 0)
                    {
                        this.CursorPos--;
                    }
                    break;

                case Keys.Right:
                    if (this.CursorPos < this.Sb.Length - 1)
                    {
                        this.CursorPos++;
                    }
                    break;

                default:
                    char? ch = key.Type();
                    if (ch is not null)
                    {
                        this.Changed = true;
                        this.Sb.Insert(this.CursorPos, (char) ch);
                        this.CursorPos++;
                    }

                    break;
            }
        }

        if (this.Changed)
        {
            this.OnChangeText?.Invoke();
        }
    }

    public void Clear()
    {
        this.Changed = true;
        this.Sb.Clear();
        this.OnChangeText?.Invoke();
        this.CursorPos = 0;
    }
}
