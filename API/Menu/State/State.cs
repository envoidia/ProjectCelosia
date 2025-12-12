using System;
using System.Collections.Generic;
using System.Text;
using API.Extensions;
using API.Input;
using Microsoft.Xna.Framework;

namespace API.Menu.State;

/// <summary>
/// A distinct menu or scene
/// </summary>
/// <param name="Name">Display name for this <c>State</c> (todo i18n)</param>
/// <param name="OnUpdate">Called every frame when this <c>State</c> is active, during the logic phase</param>
/// <param name="GetInputPrompt">Called when this <c>State</c> is first reached,
/// to update the input prompt <c>Label</c> in the bottom-right corner</param>
public sealed record State(string Name,
    Action<GameTime>? OnUpdate, Func<string>? GetInputPrompt) {
    /// <summary>
    /// Called on <c>NavPath.Add</c>. Do not call elsewhere
    /// </summary>
    public Action? OnCreate { get; init; }

    /// <summary>
    /// Called on <c>NavPath.Remove</c>. Do not call elsewhere
    /// </summary>
    public Action? OnDestroy { get; init; }

    /// <summary>
    /// Current list of menus that have been traveled through in this
    /// </summary>
    public List<Menu> Menus { get; init; } = [];

    public void Create() {
        this.OnCreate?.Invoke();
        if (this.Menus.Count > 0) this.Menus[0].Create();
    }

    public void Destroy() {
        this.OnDestroy?.Invoke();
    }

    public void Update(GameTime gameTime) {
        if (this.Menus.Count > 0) this.Menus[^1].Update();
        this.OnUpdate?.Invoke(gameTime);
    }

    public void RemoveMenu() {
        this.Menus[^1].Destroy();
        this.Menus.RemoveLast();
    }

    public static string GetInputPromptString(params InputPrompt[] inputPrompts) {
        StringBuilder inputs = new();

        for (int i = 0; i < inputPrompts.Length; i++) {
            inputs.Append(inputPrompts[i].GetText());
            if (i != (inputPrompts.Length - 1)) inputs.Append("  ");
        }

        return inputs.ToString();
    }
}