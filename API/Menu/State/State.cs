using System;
using System.Text;
using API.Input;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

/// <summary>
/// A distinct menu or scene
/// </summary>
/// <param name="Name">Display name for this <c>State</c> (todo i18n)</param>
/// <param name="Update">Called every frame when this <c>State</c> is active, during the logic phase</param>
/// <param name="Draw">Called every frame when this <c>State</c> is active, during the drawing phase</param>
/// <param name="GetInputPrompt">Called when this <c>State</c> is first reached,
/// to update the input prompt <c>Label</c> in the bottom-right corner</param>
public record State(string Name, Action<GameTime> Update, Action<GameTime> Draw, Func<string> GetInputPrompt) {
    /// <summary>
    /// Called on <c>NavPath.Add</c>. Do not call elsewhere
    /// </summary>
    public Action Create { internal get; init; } = () => { };

    /// <summary>
    /// Called on <c>NavPath.Remove</c>. Do not call elsewhere
    /// </summary>
    public Action Destroy { internal get; init; } = () => { };

    public static string GetInputPromptString(params InputPrompt[] inputPrompts) {
        StringBuilder inputs = new();

        for (int i = 0; i < inputPrompts.Length; i++) {
            inputs.Append(inputPrompts[i].GetText());
            if (i != (inputPrompts.Length - 1)) inputs.Append("  ");
        }

        return inputs.ToString();
    }
}