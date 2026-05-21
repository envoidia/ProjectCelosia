using System;
using API.Debug;
using API.Graphics;
using API.Input;
using Microsoft.Xna.Framework;

namespace API.Menu.Widget;

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
    /// Amount of selectable optithisons
    /// </summary>
    int OptCount { get; }

    /// <summary>
    /// Whether <c>this</c> should be considered to be "confirmed" separately from a press of the confirm key
    /// (such as by mouse input)
    /// </summary>
    bool ShouldConfirm { get; }

    /// <summary>
    /// Invoked immediately before Index changes
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

public static class InputWidgetExtensions
{
    extension(IInputWidget @this)
    {
        public int CheckInput()
        {
            // todo fix -1 list
            //            Assert.InRange(@this.Index, 0, @this.OptCount - 1);

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

        public void CheckScroll()
        {
            Assert.Is<IActor>(@this);
            IActor actor = (IActor) @this;

            if (actor.ContainsMouse())
            {
                int newIndex = Math.Clamp(@this.Index - (InputLib.GetMouseScroll() / InputLib.ScrollPerMouseWheelTick),
                    0, @this.OptCount - 1);
                if (@this.Index == newIndex)
                {
                    return;
                }

                @this.OnChangeIndex?.Invoke(newIndex);
                @this.Index = newIndex;
            }
        }
    }
}
