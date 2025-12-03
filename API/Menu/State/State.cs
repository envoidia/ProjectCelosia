using System;
using System.Text;
using API.Input;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

public class State(string name) {

    /// <summary>
    /// Display name for this <c>State</c> (todo i18n)
    /// </summary>
    public string Name { get; init; } = name;

    /// <summary>
    /// Called on <c>NavPath.Add</c>. Do not call elsewhere
    /// </summary>
    public Action Create { internal get; init; } = () => { };

    /// <summary>
    /// Called on <c>NavPath.Remove</c>. Do not call elsewhere
    /// </summary>
    public Action Destroy { internal get; init; } = () => { };

    /// <summary>
    /// Called every frame when this <c>State</c> is active, during the logic phase
    /// </summary>
    public required Action<GameTime> Update { get; init; }

    /// <summary>
    /// Called every frame when this <c>State</c> is active, during the drawing phase
    /// </summary>
    public required Action<GameTime> Draw { get; init; }

    /// <summary>
    /// Called when this <c>State</c> is first reached, to update the input prompt <c>Label</c> in the bottom-right corner
    /// </summary>
    public required Func<string> GetInputPrompt { get; init; }

    public static string GetInputPromptString(params InputPrompt[] inputPrompts) {
        StringBuilder inputs = new();

        for (int i = 0; i < inputPrompts.Length; i++) {
            inputs.Append(inputPrompts[i].GetText());
            if (i != (inputPrompts.Length - 1)) inputs.Append("  ");
        }

        return inputs.ToString();
    }
}