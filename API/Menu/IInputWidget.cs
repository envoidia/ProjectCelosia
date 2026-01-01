using System;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Menu;

/// <summary>
/// A widget that can accept inputs
/// </summary>
public interface IInputWidget
{
    /// <summary>
    /// Whether to check for inputs
    /// </summary>
    bool CheckInput { get; set; }

    /// <summary>
    /// Currently selected option (0-indexed)
    /// </summary>
    int Index { get; set; }

    /// <summary>
    /// Amount of selectable options
    /// </summary>
    int OptCount { get; }

    /// <summary>
    /// Invoked when Index changes
    /// </summary>
    Action<int>? OnChangeIndex { get; set; }

    /// <inheritdoc cref="SelectionType" />
    SelectionType PrefDir { get; }

    /// <summary>
    /// The directions that this is currently using for input
    /// </summary>
    SelectionType CurDir { get; set; }

    /// <summary>
    /// Called every frame to check for input
    /// </summary>
    void Input(GameTime gt);

}

public static class InputAcceptorExtensions
{
    extension(IInputWidget @this)
    {
        public int CheckInput()
        {
            Assert.InRange(@this.Index, 0, @this.OptCount - 1);

            if (@this.CheckInput)
            {
                int newIndex = MenuLib.CheckMovement1D(@this.Index, @this.OptCount, @this.CurDir);
                if (@this.Index != newIndex)
                {
                    @this.OnChangeIndex?.Invoke(newIndex);
                    return newIndex;
                }
            }

            return @this.Index;
        }
    }
}