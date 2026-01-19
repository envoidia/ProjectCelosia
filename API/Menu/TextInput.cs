using System;
using System.Text;
using API.Extensions;
using API.Graphics;
using API.Input;
using API.Util;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace API.Menu;

/// <summary>
/// Handles single-line text input. Cannot be constructed before <c>Core</c>
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
    public string Text
    {
        get
        {
            return this.Sb.ToString();
        }
    }

    /// <summary>
    /// Text display
    /// </summary>
    public readonly Label Label;

    /// <summary>
    /// Cursor display
    /// </summary>
    public readonly ARectangle Cursor;

    /// <summary>
    /// Invoked when enter is pressed. If it returns true, text is wiped
    /// </summary>
    public readonly Func<bool> OnEnter;

    /// <summary>
    /// Invoked when the text is changed
    /// </summary>
    public Action? OnChangeText;

    public bool CheckInput { get; set; }

    /// <summary>
    /// Current cursor position. 0 = very left
    /// </summary>
    public int Index
    {
        get;
        set
        {
            field = value;
            this._UpdateCursor();
        }
    }

    public int OptCount { get; set; }

    public Action<int>? OnChangeIndex { get; set; }

    public SelectionType PrefDir
    {
        get
        {
            return SelectionType.TextInput;
        }
    }

    public SelectionType CurDir { get; set; } = SelectionType.TextInput;

    private const float _MoveDelay = 0.05f;

    public TextInput(Label label, ARectangle cursor, Func<bool> onEnter)
    {
        this.Label = label;
        //this.Label.RichTextLayout.CalculateGlyphs = true;
        //this.Label.RichTextLayout.SupportsCommands = false; //todo fix

        this.Cursor = cursor;
        this.Cursor.Size = new(1, label.Height - 8); // todo height init + relative width

        this.Label.RichTextLayout.CalculateGlyphs = true;
        this.OnEnter = onEnter;

        Core.Instance.Window.TextInput += this._Input;

        this._UpdateCursor();
    }

    /// <summary>
    /// Ask OS to handle input
    /// </summary>
    private void _Input(object? sender, TextInputEventArgs args)
    {
        if (!this.CheckInput)
        {
            return;
        }

        switch (args.Key)
        {
            case Keys.Tab or Keys.Escape:
                return;

            case Keys.Enter:
                if (this.OptCount > 0)
                {
                    if (this.OnEnter())
                    {
                        this.Clear();
                    }

                    return;
                }

                return;

            case Keys.Back:
                if (this.Index > 0)
                {
                    if (InputLib.Check(Keybinds.Hotkey2))
                    {
                        while (this.Index > 0 && char.IsWhiteSpace(this.Sb[this.Index - 1]))
                        {
                            this.Sb.Remove(this.Index - 1, 1);
                            this.Index--;
                        }

                        while (this.Index > 0 && !char.IsWhiteSpace(this.Sb[this.Index - 1]))
                        {
                            this.Sb.Remove(this.Index - 1, 1);
                            this.Index--;
                        }

                        this.OptCount = this.Sb.Length;
                        this.OnChangeText?.Invoke();

                        return;
                    }

                    this.Sb.Remove(this.Index - 1, 1);
                    this.OptCount = this.Sb.Length;
                    this.OnChangeText?.Invoke();
                    this.Index--;
                }

                return;

            case Keys.Delete:
                if (this.Index < this.OptCount)
                {
                    if (InputLib.Check(Keybinds.Hotkey2))
                    {
                        while (this.Sb.Length > this.Index && char.IsWhiteSpace(this.Sb[this.Index]))
                        {
                            this.Sb.Remove(this.Index, 1);
                        }

                        while (this.Sb.Length > this.Index && !char.IsWhiteSpace(this.Sb[this.Index]))
                        {
                            this.Sb.Remove(this.Index, 1);
                        }

                        this.OptCount = this.Sb.Length;
                        this.OnChangeText?.Invoke();

                        return;
                    }

                    this.Sb.Remove(this.Index, 1);
                    this.OptCount = this.Sb.Length;
                    this.OnChangeText?.Invoke();
                }

                return;

            default:
                this.Sb.Insert(this.Index, args.Character);
                this.OptCount = this.Sb.Length;
                this.OnChangeText?.Invoke();
                this.Index++;
                return;
        }
    }

    // OS doesn't handle nav input
    public void Input(GameTime gt)
    {
        if (InputLib.IsKeyJustPressed(Keys.Insert))
        {
            Console.WriteLine($"{Index}/{OptCount}");
        }

        if (InputLib.Check(Keybinds.Left, true, _MoveDelay))
        {
            // Word jump
            if (InputLib.Check(Keybinds.Hotkey2))
            {
                while (this.Index > 0 && char.IsWhiteSpace(this.Sb[this.Index - 1]))
                {
                    this.Index--;
                }

                while (this.Index > 0 && !char.IsWhiteSpace(this.Sb[this.Index - 1]))
                {
                    this.Index--;
                }

                return;
            }

            if (this.Index > 0)
            {
                this.Index--;
            }
        }

        if (InputLib.Check(Keybinds.Right, true, _MoveDelay))
        {
            if (InputLib.Check(Keybinds.Hotkey2))
            {
                while (this.Index < this.OptCount && char.IsWhiteSpace(this.Sb[this.Index]))
                {
                    this.Index++;
                }

                while (this.Index < this.OptCount && !char.IsWhiteSpace(this.Sb[this.Index]))
                {
                    this.Index++;
                }

                return;
            }

            if (this.Index < this.OptCount)
            {
                this.Index++;
            }
        }

        if (InputLib.Check(Keybinds.Up) || InputLib.IsKeyJustPressed(Keys.Home))
        {
            this.Index = 0;
        }

        if (InputLib.Check(Keybinds.Down) || InputLib.IsKeyJustPressed(Keys.End))
        {
            this.Index = this.OptCount;
        }
    }

    private void _UpdateCursor()
    {
        int x = Index == OptCount
            ? this.Label.Width
            : ((TextChunk) this.Label.RichTextLayout.Lines[0].Chunks[0]).Glyphs[this.Index + 1].Bounds.X;

        this.Cursor.Position = new(this.Label.X + 1 + x, this.Label.Y - this.Label.Height + 4);
    }

    public void Clear()
    {
        this.Sb.Clear();
        this.OptCount = 0;
        this.OnChangeText?.Invoke();
        this.Index = 0;
    }
}
