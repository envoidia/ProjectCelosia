using System;
using System.Collections.Generic;
using System.Text;
using API.Graphics;
using API.Input;
using API.Util;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace API.Menu.Widget;

/// <summary>
/// Handles single-line text input.
/// After construction, call <c>SubscribeToInput</c> exactly once, after construction of <c>Core</c>.
/// Receives text input from the OS. Should support all keyboards well.
/// Supports cursor movement, Home/End, BkSp/Del, and Hotkey2 for per-word actions.
/// Does not support tab, history, selection, clipboard, overwrite mode, or multiple lines
/// </summary>
// todo onscreen keyboard
public sealed class TextInputWidget : IInputWidget
{
    private readonly StringBuilder _Sb;

    /// <summary>
    /// Current text as a <c>string</c>
    /// </summary>
    public string Text = "";

    /// <summary>
    /// Text display
    /// </summary>
    public readonly Label Label;

    public const int UnlimitedLength = -1;

    /// <summary>
    /// Max length of text input
    /// </summary>
    // todo impl
    public int MaxLength = UnlimitedLength;

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

    public TextInputWidget(Label label, ARectangle cursor, Func<bool> onEnter, bool useUpDown = true, int maxLength = UnlimitedLength)
    {
        this.Label = label;
        this.Label.RichTextLayout.CalculateGlyphs = true;

        this.Cursor = cursor;
        this.Cursor.Size = new(2, label.Height - 8); // todo height init + relative width(?) (is that supposed to say height?)

        this.OnEnter = onEnter;
        this.UseUpDown = useUpDown;

        this.MaxLength = maxLength;
        this._Sb = new(maxLength == UnlimitedLength ? 128 : maxLength);

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
        this.Update();
        this.Index = this.OptCount;
    }

    public void Insert(char c)
    {
        this._Sb.Insert(this.Index, c);
        this.Update();
        this.Index++;
    }

    public void Clear()
    {
        this._Sb.Clear();
        this.Update();
        this.Index = 0;
    }

    public void SetText(string str)
    {
        this._Sb.Clear();
        this.Append(str);
    }

    /// <summary>
    /// Call after changing the text.
    /// Called automatically by <c>Append</c>, <c>Insert</c>, <c>SetText</c>, and <c>Clear</c>
    /// </summary>
    public void Update()
    {
        this.Text = this._Sb.ToString();
        this.OptCount = this._Sb.Length;
        this.OnChangeText?.Invoke();
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
                if (this.Index == 0)
                {
                    return;
                }

                if (InputLib.IsCtrlPressed())
                {
                    // Delete all left
                    if (InputLib.IsShiftPressed())
                    {
                        while (this.Index > 0)
                        {
                            this._Sb.Remove(this.Index - 1, 1);
                            this.Index--;
                        }

                        this.Update();

                        return;
                    }

                    // Word nav
                    while (this.Index > 0 && _IsWordSplitter(this._Sb[this.Index - 1]))
                    {
                        this._Sb.Remove(this.Index - 1, 1);
                        this.Index--;
                    }

                    while (this.Index > 0 && !_IsWordSplitter(this._Sb[this.Index - 1]))
                    {
                        this._Sb.Remove(this.Index - 1, 1);
                        this.Index--;
                    }

                    this.Update();

                    return;
                }

                // Word part nav
                if (InputLib.IsAltPressed())
                {
                    while (this.Index > 0 && _IsWordSplitter(this._Sb[this.Index - 1]))
                    {
                        this._Sb.Remove(this.Index - 1, 1);
                        this.Index--;
                    }

                    if (this.Index == 0)
                    {
                        this.Update();
                        return;
                    }

                    if (char.IsUpper(this._Sb[this.Index - 1]))
                    {
                        this._Sb.Remove(this.Index - 1, 1);
                        this.Update();
                        this.Index--;

                        return;
                    }

                    do
                    {
                        this._Sb.Remove(this.Index - 1, 1);
                        this.Index--;
                    }
                    while (this.Index > 0 && !_IsWordSplitter(this._Sb[this.Index - 1])
                        && !char.IsUpper(this._Sb[this.Index - 1]));

                    // Do not return
                }

                if (this.Index > 0)
                {
                    this._Sb.Remove(this.Index - 1, 1);
                    this.Update();
                    this.Index--;
                }

                return;

            case Keys.Delete:
                if (this.Index < this.OptCount)
                {
                    if (InputLib.IsCtrlPressed())
                    {
                        if (InputLib.IsShiftPressed())
                        {
                            while (this._Sb.Length > this.Index)
                            {
                                this._Sb.Remove(this.Index, 1);
                            }

                            this.Update();

                            return;
                        }

                        while (this._Sb.Length > this.Index && _IsWordSplitter(this._Sb[this.Index]))
                        {
                            this._Sb.Remove(this.Index, 1);
                        }

                        while (this._Sb.Length > this.Index && !_IsWordSplitter(this._Sb[this.Index]))
                        {
                            this._Sb.Remove(this.Index, 1);
                        }

                        this.Update();

                        return;
                    }

                    if (InputLib.IsAltPressed())
                    {
                        while (this._Sb.Length > this.Index && _IsWordSplitter(this._Sb[this.Index]))
                        {
                            this._Sb.Remove(this.Index, 1);
                        }

                        if (this._Sb.Length == 0)
                        {
                            this.Update();
                            return;
                        }

                        if (this._Sb.Length == this.Index + 1 || char.IsUpper(this._Sb[this.Index + 1]))
                        {
                            this._Sb.Remove(this.Index, 1);
                            this.Update();
                            return;
                        }

                        do
                        {
                            this._Sb.Remove(this.Index, 1);
                        }
                        while (this._Sb.Length > this.Index + 1 && !_IsWordSplitter(this._Sb[this.Index])
                            && !char.IsUpper(this._Sb[this.Index + 1]));

                        // Do not return
                    }

                    this._Sb.Remove(this.Index, 1);
                    this.Update();
                }

                return;

            // Replace open brackets with fullwidth counterparts. They look the same and FSS doesn't parse them
            // If shift is held to type {, the key is instead None) (todo: test on other OSes)
            // (todo use real brackets when logging)
            case Keys.OemOpenBrackets:
                this.Insert('［');
                return;

            default:
                this.Insert(args.Character);
                return;
        }
    }

    // OS doesn't handle nav input
    public void Input(GameTime gt)
    {
        bool ctrlPressed = InputLib.IsCtrlPressed();
        bool altPressed = InputLib.IsAltPressed();

        if (InputLib.Check(Keybinds.Left, true, _MoveDelay))
        {
            // Word jump
            if (ctrlPressed)
            {
                while (this.Index > 0 && _IsWordSplitter(this._Sb[this.Index - 1]))
                {
                    this.Index--;
                }

                while (this.Index > 0 && !_IsWordSplitter(this._Sb[this.Index - 1]))
                {
                    this.Index--;
                }

                return;
            }

            // Word part jump
            if (altPressed)
            {
                while (this.Index > 0 && _IsWordSplitter(this._Sb[this.Index - 1]))
                {
                    this.Index--;
                }

                if (this.Index == 0)
                {
                    return;
                }

                if (char.IsUpper(this._Sb[this.Index - 1]))
                {
                    this.Index--;
                    return;
                }

                do
                {
                    this.Index--;
                }
                while (this.Index > 0 && !_IsWordSplitter(this._Sb[this.Index - 1])
                    && !char.IsUpper(this._Sb[this.Index - 1]));

                // Do not return
            }

            if (this.Index > 0)
            {
                this.Index--;
            }

            return;
        }

        if (InputLib.Check(Keybinds.Right, true, _MoveDelay))
        {
            if (ctrlPressed)
            {
                while (this.Index < this.OptCount && _IsWordSplitter(this._Sb[this.Index]))
                {
                    this.Index++;
                }

                while (this.Index < this.OptCount && !_IsWordSplitter(this._Sb[this.Index]))
                {
                    this.Index++;
                }

                return;
            }

            if (altPressed)
            {
                while (this.Index < this.OptCount && _IsWordSplitter(this._Sb[this.Index]))
                {
                    this.Index++;
                }

                if (this.Index == this.OptCount)
                {
                    return;
                }

                if (char.IsUpper(this._Sb[this.Index + 1]))
                {
                    this.Index++;
                    return;
                }

                do
                {
                    this.Index++;
                }
                while (this.Index < this.OptCount - 1 && !_IsWordSplitter(this._Sb[this.Index])
                    && !char.IsUpper(this._Sb[this.Index + 1]));

                // Do not return
            }

            if (this.Index < this.OptCount)
            {
                this.Index++;
            }

            return;
        }

        if (InputLib.IsKeyJustPressed(Keys.Home) || (this.UseUpDown && InputLib.Check(Keybinds.Up)))
        {
            this.Index = 0;
            return;
        }

        if (InputLib.IsKeyJustPressed(Keys.End) || (this.UseUpDown && InputLib.Check(Keybinds.Down)))
        {
            this.Index = this.OptCount;
            return;
        }

        if (!ctrlPressed)
        {
            return;
        }

        if (InputLib.IsKeyJustPressed(Keys.K) && InputLib.IsCtrlPressed() && InputLib.IsShiftPressed())
        {
            this.Clear();
            return;
        }

#if !CONSOLE
        if (InputLib.IsKeyPressed(Keys.C))
        {
            Clipboard.Text = this.Text.Replace('［', '[');
            return;
        }

        if (InputLib.IsKeyJustPressed(Keys.X))
        {
            Clipboard.Text = this.Text;
            this.Clear();
            return;
        }

        if (InputLib.IsKeyJustPressed(Keys.V))
        {
            char[] cb = Clipboard.Text.ToCharArray();

            for (int i = 0; i < cb.Length; i++)
            {
                switch (cb[i])
                {
                    case '\n':
                        cb[i] = ' ';
                        continue;

                    case '[':
                        cb[i] = '［';
                        continue;
                }

                if (char.IsSurrogate(cb[i]))
                {
                    cb[i] = '�';
                }
            }

            this._Sb.Insert(this.Index, cb);
            this.Update();
            this.Index += cb.Length;
        }
#endif
    }

    internal void _UpdateCursor()
    {
        List<BaseChunk> chunks = this.Label.RichTextLayout.Lines[0].Chunks;
        List<TextChunkGlyph> glyphs = ((TextChunk) chunks[^1]).Glyphs;

        int i = chunks.Count == 1 ? this.Index + 1 : this.Index;
        if (i >= glyphs.Count)
        {
            return;
        }

        int x = glyphs[i].Bounds.X;

        this.Cursor.Position = new(this.Label.X + x - 2, this.Label.Y - this.Label.Height);
    }

    private static bool _IsWordSplitter(char c)
    {
        return char.IsWhiteSpace(c) || c == ':';
    }
}
