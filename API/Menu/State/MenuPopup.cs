using Microsoft.Xna.Framework;

namespace API.Menu.State;

using static API.Input.InputPrompts;

public sealed class MenuPopup : IState {
    public void Update(GameTime gameTime) {

    }

    public void Draw(GameTime gameTime) {
        // todo
    }

    public string GetInputPrompt() => IState.GetInputPromptString(Close);
}
