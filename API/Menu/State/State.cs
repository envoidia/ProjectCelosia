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
/// <param name="GetInputPrompt">Called when this is first reached,
/// to update the input prompt <c>Label</c> in the bottom-right corner</param>
public sealed record State(string Name, Action<GameTime>? OnUpdate, Func<string>? GetInputPrompt) {
    /// <summary>
    /// Called on <c>StateMachine.Add</c>. Do not call elsewhere
    /// </summary>
    public Action? OnCreate { get; init; }

    /// <summary>
    /// Called on <c>StateMachine.Remove</c>. Do not call elsewhere
    /// </summary>
    public Action? OnDestroy { get; init; }

    /// <summary>
    /// Current list of menus that have been traveled through in this. Use <c>RemoveMenu</c> rather than direct removal
    /// </summary>
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

    public void AddMenu(Menu menu) {
        menu.Create();
        this._Menus.Add(menu);
    }

    public void RemoveMenu() {
        this._Menus[^1].Destroy();
        this._Menus.RemoveLast();
    }

    public string GetMenuString() =>
        string.Join(", ", [.. this._Menus.Select(m => m.Name)]);

    public static string GetInputPromptString(params InputPrompt[] inputPrompts) {
        StringBuilder inputs = new();

        for (int i = 0; i < inputPrompts.Length; i++) {
            inputs.Append(inputPrompts[i].GetText());
            if (i != (inputPrompts.Length - 1)) inputs.Append("  ");
        }

        return inputs.ToString();
    }
}