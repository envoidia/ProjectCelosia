using System;
using API.Extensions;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

using static API.Input.InputPrompts;

public sealed class MenuPopup : IState {
    #region Impl

    public MenuPopup() {
        if (Core.MenuPopup is not null) {
            throw new InvalidOperationException(string.Format(Lang.MultipleInstance, nameof(MenuPopup)));
        }
    }

    public void Update(GameTime gameTime) {

    }

    public void Draw(GameTime gameTime) =>
        // Draw the previous IState underneath
        Core.NavPath.path[^2].Draw(gameTime);// Draw popup// todo

    public string GetInputPrompt() => IState.GetInputPromptString(Close);

    #endregion
}
