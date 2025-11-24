using System.Text;
using API.Input;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

/// <summary>
/// A menu/scene that can be updated and drawn
/// </summary>
public interface IState {
    /// <summary>
    /// Called every frame when this <c>IState</c> is active, during the logic phase
    /// </summary>
    void Update(GameTime gameTime);

    /// <summary>
    /// Called every frame when this <c>IState</c> is active, during the drawing phase
    /// </summary>
    void Draw(GameTime gameTime);

    /// <summary>
    /// Called when this <c>IState</c> is first reached, to update the input prompt <c>Label</c> in the bottom-right corner
    /// </summary>
    string GetInputPrompt();

    protected static string GetInputPromptString(params InputPrompt[] inputPrompts) {
        StringBuilder inputs = new();

        for (int i = 0; i < inputPrompts.Length; i++) {
            inputs.Append(inputPrompts[i].GetText());
            if (i != (inputPrompts.Length - 1)) inputs.Append("  ");
        }

        return inputs.ToString();
    }
}
