using System;
using System.Collections.Generic;
using System.Text;
using API.Debug;
using API.Graphics;
using API.Input;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TextCopy;

namespace API.Menu.Widget;

/// <summary>
/// Handles single-line text input.
/// After construction, call <c>SubscribeToInput</c> exactly once, after construction of <c>Core</c>.
/// Receives text input from the OS. Should support all keyboards well.
/// Supports cursor movement, Home/End, BkSp/Del, and Hotkey2 for per-word actions.
/// Does not support tab, history, selection, clipboard, overwrite mode, or multiple lines
/// </summary>
public sealed class TextInputWidget : IInputWidget
{
    public const string ClipboardError = "Failed to access clipboard! If on Linux, is xsel installed?";

    private readonly StringBuilder _Sb = new();

    /// <summary>
    /// Current text as a <c>string</c>. Allocates
    /// </summary>
    public string Text
    {
        get
        {
            return this._Sb.ToString();
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

    /// <summary>
    /// Whether up/down should be aliases for home/end
    /// </summary>
    public bool UseUpDown;

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

    internal const float _MoveDelay = 0.05f;

    public TextInputWidget(Label label, ARectangle cursor, Func<bool> onEnter, bool useUpDown = true)
    {
        this.Label = label;
        //this.Label.RichTextLayout.SupportsCommands = false; //todo fix
        this.Label.RichTextLayout.CalculateGlyphs = true;

        this.Cursor = cursor;
        this.Cursor.Size = new(1, label.Height - 8); // todo height init + relative width(?) (is that supposed to say height?)

        this.OnEnter = onEnter;
        this.UseUpDown = useUpDown;

        this._UpdateCursor();
    }

    /// <summary>
    /// Subscribes to OS text input. Call exactly once after construction
    /// </summary>
    public void SubscribeToInput()
    {
        Core.Instance.Window.TextInput += this._Input;
    }

    public void Append(string str)
    {
        this._Sb.Append(str);
        this.OptCount = this._Sb.Length;
        this.OnChangeText?.Invoke();
        this.Index = this.OptCount;
    }

    public void Clear()
    {
        this._Sb.Clear();
        this.OptCount = 0;
        this.OnChangeText?.Invoke();
        this.Index = 0;
    }

    public void SetText(string str)
    {
        this._Sb.Clear();
        this.Append(str);
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
                        while (this.Index > 0 && char.IsWhiteSpace(this._Sb[this.Index - 1]))
                        {
                            this._Sb.Remove(this.Index - 1, 1);
                            this.Index--;
                        }

                        while (this.Index > 0 && !char.IsWhiteSpace(this._Sb[this.Index - 1]))
                        {
                            this._Sb.Remove(this.Index - 1, 1);
                            this.Index--;
                        }

                        this.OptCount = this._Sb.Length;
                        this.OnChangeText?.Invoke();

                        return;
                    }

                    this._Sb.Remove(this.Index - 1, 1);
                    this.OptCount = this._Sb.Length;
                    this.OnChangeText?.Invoke();
                    this.Index--;
                }

                return;

            case Keys.Delete:
                if (this.Index < this.OptCount)
                {
                    if (InputLib.Check(Keybinds.Hotkey2))
                    {
                        while (this._Sb.Length > this.Index && char.IsWhiteSpace(this._Sb[this.Index]))
                        {
                            this._Sb.Remove(this.Index, 1);
                        }

                        while (this._Sb.Length > this.Index && !char.IsWhiteSpace(this._Sb[this.Index]))
                        {
                            this._Sb.Remove(this.Index, 1);
                        }

                        this.OptCount = this._Sb.Length;
                        this.OnChangeText?.Invoke();

                        return;
                    }

                    this._Sb.Remove(this.Index, 1);
                    this.OptCount = this._Sb.Length;
                    this.OnChangeText?.Invoke();
                }

                return;

            default:
                this._Sb.Insert(this.Index, args.Character);
                this.OptCount = this._Sb.Length;
                this.OnChangeText?.Invoke();
                this.Index++;

                return;
        }
    }

    // OS doesn't handle nav input
    public void Input(GameTime gt)
    {
        bool ctrlPressed = InputLib.IsKeyPressed(Keys.LeftControl) || InputLib.IsKeyPressed(Keys.RightControl);

        if (InputLib.Check(Keybinds.Left, true, _MoveDelay))
        {
            // Word jump
            if (ctrlPressed)
            {
                while (this.Index > 0 && char.IsWhiteSpace(this._Sb[this.Index - 1]))
                {
                    this.Index--;
                }

                while (this.Index > 0 && !char.IsWhiteSpace(this._Sb[this.Index - 1]))
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
        else if (InputLib.Check(Keybinds.Right, true, _MoveDelay))
        {
            if (ctrlPressed)
            {
                while (this.Index < this.OptCount && char.IsWhiteSpace(this._Sb[this.Index]))
                {
                    this.Index++;
                }

                while (this.Index < this.OptCount && !char.IsWhiteSpace(this._Sb[this.Index]))
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

        if (InputLib.IsKeyJustPressed(Keys.Home) || (this.UseUpDown && InputLib.Check(Keybinds.Up)))
        {
            this.Index = 0;
        }
        else if (InputLib.IsKeyJustPressed(Keys.End) || (this.UseUpDown && InputLib.Check(Keybinds.Down)))
        {
            this.Index = this.OptCount;
        }

        if (!ctrlPressed)
        {
            return;
        }

        if (InputLib.IsKeyPressed(Keys.C))
        {
            try
            {
                ClipboardService.SetText(this.Text);
            }
            catch (Exception e)
            {
                DebugConsole.Log(ClipboardError, nameof(TextInputWidget), DebugConsole.LogLevel.Error);
                Console.WriteLine(e);
            }
        }

        if (InputLib.IsKeyJustPressed(Keys.X))
        {
            try
            {
                ClipboardService.SetText(this.Text);
            }
            catch (Exception e)
            {
                DebugConsole.Log(ClipboardError, nameof(TextInputWidget), DebugConsole.LogLevel.Error);
                Console.WriteLine(e);
            }

            this.Clear();
        }

        if (InputLib.IsKeyJustPressed(Keys.V))
        {
            string? str;
            try
            {
                str = ClipboardService.GetText();
            }
            catch (Exception e)
            {
                DebugConsole.Log(ClipboardError, nameof(TextInputWidget), DebugConsole.LogLevel.Error);
                Console.WriteLine(e);

                return;
            }

            if (str is null)
            {
                return;
            }

            this._Sb.Insert(this.Index, str);
            this.OptCount += str.Length;
            this.OnChangeText?.Invoke();
            this.Index += str.Length;
        }
    }

    internal void _UpdateCursor()
    {
        List<TextChunkGlyph> glyphs = ((TextChunk) this.Label.RichTextLayout.Lines[0].Chunks[0]).Glyphs;

        int i = this.Index + 1;
        if (i >= glyphs.Count)
        {
            return;
        }

        int x = glyphs[i].Bounds.X;

        this.Cursor.Position = new(this.Label.X + 1 + x, this.Label.Y - this.Label.Height + 4);
    }
}
