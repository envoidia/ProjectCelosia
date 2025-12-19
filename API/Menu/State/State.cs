using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API.Extensions;
using API.Input;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

/// <summary>
/// A distinct scene
/// </summary>
/// <param name="Name">Display name for this (todo i18n)</param>
/// <param name="OnUpdate">Called every frame when this is active, during the logic phase</param>
/// <param name="OnGetInputPrompt">Called when this is first reached to update the input prompt <c>Label</c>
/// in the bottom-right corner. Menus can override this</param>
// todo should states store List<IActor> and automatically add/remove them
public sealed record State(string Name, Action<GameTime>? OnUpdate, Func<string>? OnGetInputPrompt) {
    /// <summary>
    /// Called on <c>StateMachine.Add</c>. Do not call elsewhere
    /// </summary>
    public Action? OnCreate { get; init; }

    /// <summary>
    /// Called on <c>StateMachine.Remove</c>. Do not call elsewhere
    /// </summary>
    public Action? OnDestroy { get; init; }

    /// <summary>
    /// Current list of menus that have been traveled through in this
    /// </summary>
    // todo private
    internal List<Menu> _Menus { get; init; } = [];

    /// <inheritdoc cref="OnCreate" />
    public void Create() {
        this.OnCreate?.Invoke();
        if (this._Menus.Count > 0) this._Menus[0].Create();
    }

    /// <inheritdoc cref="OnDestroy" />
    public void Destroy() => this.OnDestroy?.Invoke();

    public void Update(GameTime gameTime) {
        if (this._Menus.Count > 0) this._Menus[^1].Update(gameTime);
        this.OnUpdate?.Invoke(gameTime);
    }

    /// <returns>
    /// Called when this is first reached and on menu change to update the input prompt <c>Label</c> in the bottom-right corner
    /// </returns>
    public string GetInputPrompt() {
        // Use Menu prompt
        if (this._Menus.Count > 0) {
            Func<string>? menuPrompt = this._Menus[^1].GetInputPrompt;
            if (menuPrompt is not null) {
                return menuPrompt();
            }
        }

        // Use State prompt
        return this.OnGetInputPrompt?.Invoke() ?? "";
    }

    /// <summary>
    /// Add and initialize a <c>Menu</c>
    /// </summary>
    public void AddMenu(Menu menu) {
        menu.Create();
        this._Menus.Add(menu);
        StateMachine.UpdateInputPrompt();
    }

    /// <summary>
    /// Remove and deinitialize the current <c>Menu</c>
    /// </summary>
    public void RemoveMenu() {
        this._Menus[^1].Destroy();
        this._Menus.RemoveLast();
        StateMachine.UpdateInputPrompt();
    }

    public string GetMenuString() =>
        string.Join(", ", [.. this._Menus.Select(static m => m.Name)]);

    public static string GetInputPromptString(params InputPrompt[] inputPrompts) {
        StringBuilder inputs = new();

        for (int i = 0; i < inputPrompts.Length; i++) {
            inputs.Append(inputPrompts[i].GetText());
            if (i != (inputPrompts.Length - 1)) inputs.Append("  ");
        }

        return inputs.ToString();
    }
}